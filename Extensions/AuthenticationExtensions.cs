using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookStoreApi.Extensions
{
    public static class AuthenticationExtensions
    {
            public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                    options.DefaultChallengeScheme =
                    options.DefaultForbidScheme =
                    options.DefaultScheme =
                    options.DefaultSignInScheme =
                    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"])
                        ),
                        //// You might also want to validate lifetime and set a clock skew
                        //ValidateLifetime = true,
                        //ClockSkew = TimeSpan.Zero // Recommended for no clock skew tolerance
                    };
                });

                services.AddAuthorization(); // Authorization typically goes with authentication
                                             // This enables the [Authorize] attribute
                return services;
            }
    }
}
