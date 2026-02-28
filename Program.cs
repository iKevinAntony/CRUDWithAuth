using CRUDWithAuth.Data;
using CRUDWithAuth.Extensions;
using CRUDWithAuth.Helpers;
using CRUDWithAuth.Helpers.StaticEnums;
using CRUDWithAuth.Services;
using CRUDWithAuth.Services.AuthRoles;
using CRUDWithAuth.Services.Expense;
using CRUDWithAuth.Services.IServices;
using CRUDWithAuth.Services.IServices.AuthRoles;
using CRUDWithAuth.Services.IServices.Expense;
using CRUDWithAuth.Services.IServices.Shared;
using CRUDWithAuth.Services.Shared;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBCon"));
});
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
var userJwtTokenConfig = builder.Configuration.GetSection("userJwtTokenConfig").Get<UserJwtTokenConfig>();
if (userJwtTokenConfig != null)
{
    builder.Services.AddSingleton(userJwtTokenConfig);
}
var serverUrls = builder.Configuration.GetSection("serverUrls").Get<ServerUrls>();
if (serverUrls != null)
{
    builder.Services.AddSingleton(serverUrls);
}
builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)  // Read settings from appsettings.json
            .ReadFrom.Services(services)                    // Allows DI for loggers with enriched properties
            .Enrich.FromLogContext()                        // Adds context properties to logs
            .WriteTo.Console()                              // Write logs to console
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)  // Write logs to a file with daily rotation
        );

#region Services

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
#endregion
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Use the custom Swagger setup
builder.Services.AddCustomSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    //options.AddPolicy("AllowAny",
    //    builder =>
    //    {
    //        builder.WithOrigins("https://localhost:7177/",
    //        "https://localhost:7177/")
    //        .AllowAnyMethod()
    //        .AllowAnyHeader().AllowCredentials();
    //    });
});
builder.AddAppAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizePolicies.UserOnly.ToString(), policy =>
        policy.RequireAuthenticatedUser()
              .AddAuthenticationSchemes(AuthenticationSchemes.UserScheme.ToString())
              .RequireClaim("UserType", ClaimUserTypes.CAUSER.ToString()));  // No role, using claim instead

});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    app.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/user/swagger.json", "User API");
        c.DocumentTitle = "CRUDWithAuth API";
    });
}
else
{
    app.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/user/swagger.json", "User API");
        c.DocumentTitle = "CRUDWithAuth API";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAny");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
