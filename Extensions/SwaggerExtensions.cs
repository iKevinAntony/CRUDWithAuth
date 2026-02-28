using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace CRUDWithAuth.Extensions
{
    /// <summary>
    /// Defines extension methods for configuring Swagger services in the application.
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Adds Swagger configuration to the service collection, including documentation and customization options.
        /// </summary>
        /// <param name="services"></param>
        /// <returns>Swagger Service</returns>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";

            services.AddSwaggerGen(option =>
            {
                option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                option.SwaggerDoc("user", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "User APIs",
                    Description = "This is the first version (v1) of the backend APIs in the “CRUDWithAuth” system, designed to provide secure user authentication using JWT along with refresh token support, and to enable full CRUD operations for managing expenses with optional file uploads. The system follows a single-role authorization model, where all authenticated users have the same level of access to protected resources, ensuring simplicity while maintaining strong security. Users authenticate through the login endpoint to receive a short-lived access token and a long-lived refresh token, which are used to securely access and renew sessions for protected APIs. Once authenticated, users can create, view, update, and delete expense records, attach supporting files such as receipts or bills, and retrieve data using filtering and pagination features. These APIs follow RESTful principles and are secured using JWT Bearer authentication to ensure reliable, consistent, and secure expense management across the platform.",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Kevin Antony",
                        Email = "kevinantony.mail@gmail.com",
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "Built By Kevin",
                        Url = new Uri("https://www.linkedin.com/in/ikevinantony"),
                    }
                });

                option.SwaggerDoc("crud", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "CRUD APIs",
                    Description = "This collection of APIs provides functionality related to CRUD operations within the \"CRUDWithAuth\" platform.",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Kevin Antony",
                        Email = "kevinantony.mail@gmail.com",
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "Built By Kevin",
                        Url = new Uri("https://www.linkedin.com/in/ikevinantony"),
                    }
                });

              

                option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Token, eg:`Bearer access-token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        }, new string[]{}
                    }
                });
            });
            return services;
        }
    }
}
