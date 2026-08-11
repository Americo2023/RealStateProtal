using Microsoft.AspNetCore.Authentication.JwtBearer;
using RealStatePortal.Application;
using RealStatePortal.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BrokerOrAdministrator", policy => policy.RequireRole("Broker", "Administrator"));
    options.AddPolicy("RegisteredUser", policy => policy.RequireAuthenticatedUser());
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
