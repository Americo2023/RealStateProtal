using RealStatePortal.Api.Extensions;
using RealStatePortal.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProjectServices(builder.Configuration);

var auth0Domain = builder.Configuration["Auth0:Domain"];
var auth0Audience = builder.Configuration["Auth0:Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (!string.IsNullOrWhiteSpace(auth0Domain))
        {
            options.Authority = $"https://{auth0Domain.TrimEnd('/')}/";
        }

        options.Audience = auth0Audience;
        options.RequireHttpsMetadata = builder.Configuration.GetValue("Auth0:RequireHttpsMetadata", true);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "sub",
            RoleClaimType = "realstateportal_role"
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RegisteredUser", policy => policy.RequireClaim("realstateportal_role", "RegisteredUser", "Broker", "Administrator"))
    .AddPolicy("Broker", policy => policy.RequireClaim("realstateportal_role", "Broker", "Administrator"))
    .AddPolicy("Administrator", policy => policy.RequireClaim("realstateportal_role", "Administrator"));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
