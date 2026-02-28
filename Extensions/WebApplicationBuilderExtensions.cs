using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CRUDWithAuth.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {


            var userJWT = builder.Configuration.GetSection("userJwtTokenConfig");
           

            var user_secret = userJWT.GetValue<string>("secret") ?? "";
            var user_issuer = userJWT.GetValue<string>("issuer");
            var user_audience = userJWT.GetValue<string>("audience");



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("UserScheme", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = user_issuer,  // Replace with your issuer
                    ValidAudience = user_audience,  // Replace with your audience
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(user_secret)) // Replace with your secret key
                };

            });



            return builder;
        }
    }
}
