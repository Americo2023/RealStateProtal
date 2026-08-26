using System.Security.Claims;
using DotNetEnv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using RealStatePortal.Api.Authorization;
using RealStatePortal.Application;
using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Enums;
using RealStatePortal.Infrastructure;

var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
    Env.Load(envFile);
}

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"))
    && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")))
{
    Environment.SetEnvironmentVariable(
        "ConnectionStrings__DefaultConnection",
        "Server=localhost,1433;Database=RealStatePortal;User Id=sa;Password="
        + Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")
        + ";TrustServerCertificate=True;");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "smart";
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddPolicyScheme("smart", "Bearer or cookie", options =>
    {
        options.ForwardDefaultSelector = context =>
            context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? JwtBearerDefaults.AuthenticationScheme
                : CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "realstateportal.session";
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    })
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.ClientId = builder.Configuration["Auth0:ClientId"];
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
        options.ResponseType = "code";
        options.UsePkce = true;
        options.SaveTokens = false;
        options.CallbackPath = "/auth/callback";
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Events.OnTokenValidated = async context =>
        {
            var principal = context.Principal;
            var externalId = principal?.FindFirst("sub")?.Value
                ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal?.FindFirst(ClaimTypes.Email)?.Value ?? principal?.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(externalId) || string.IsNullOrWhiteSpace(email))
            {
                context.Fail("The identity provider did not return the required user claims.");
                return;
            }

            var validatedPrincipal = principal!;
            var firstName = validatedPrincipal.FindFirst(ClaimTypes.GivenName)?.Value
                ?? validatedPrincipal.FindFirst("given_name")?.Value
                ?? "User";
            var lastName = validatedPrincipal.FindFirst(ClaimTypes.Surname)?.Value
                ?? validatedPrincipal.FindFirst("family_name")?.Value
                ?? "User";
            var roles = validatedPrincipal.Claims
                .Where(claim => claim.Type == ClaimTypes.Role
                    || claim.Type == "roles"
                    || claim.Type.EndsWith("/roles", StringComparison.OrdinalIgnoreCase))
                .Select(claim => claim.Value switch
                {
                    "Registered User" => UserRole.RegisteredUser,
                    "RegisteredUser" => UserRole.RegisteredUser,
                    "Broker" => UserRole.Broker,
                    "Administrator" => UserRole.Administrator,
                    "Visitor" => UserRole.Visitor,
                    _ => (UserRole?)null
                })
                .Where(role => role.HasValue)
                .Select(role => role!.Value)
                .Distinct()
                .DefaultIfEmpty(UserRole.RegisteredUser)
                .ToArray();

            try
            {
                var provisioning = context.HttpContext.RequestServices.GetRequiredService<IIdentityProvisioningService>();
                var user = await provisioning.ProvisionAsync(
                    new ExternalUserProfile(externalId, email, firstName, lastName, roles),
                    context.HttpContext.RequestAborted);
                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var localRoles = await userRepository.GetRolesAsync(user.Id, context.HttpContext.RequestAborted);

                if (validatedPrincipal.Identity is ClaimsIdentity identity)
                {
                    foreach (var claim in validatedPrincipal.FindAll(ClaimTypes.NameIdentifier).ToArray())
                    {
                        identity.RemoveClaim(claim);
                    }

                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                    foreach (var role in localRoles)
                    {
                        var roleName = role switch
                        {
                            UserRole.RegisteredUser => "Registered User",
                            UserRole.Broker => "Broker",
                            UserRole.Administrator => "Administrator",
                            UserRole.Visitor => "Visitor",
                            _ => role.ToString()
                        };

                        if (!identity.Claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == roleName))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                        }
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                context.Fail(exception.Message);
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BrokerOrAdministrator", policy =>
    {
        policy.Requirements.Add(new LocalRoleRequirement("Broker", "Administrator"));
    });
    options.AddPolicy("Administrator", policy =>
    {
        policy.Requirements.Add(new LocalRoleRequirement("Administrator"));
    });
    options.AddPolicy("RegisteredUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});
builder.Services.AddScoped<IAuthorizationHandler, LocalRoleAuthorizationHandler>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    name = "RealStatePortal API",
    status = "running",
    authentication = "/api/auth/me"
}));

app.Run();
