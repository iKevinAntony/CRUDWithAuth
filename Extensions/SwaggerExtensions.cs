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
                    Description = "This is the first version (v1) of the backend APIs  in the \"CRUDWithAuth\" system. These APIs handle user authentication to ensure secure and efficient management of user permissions within the platform.",
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
