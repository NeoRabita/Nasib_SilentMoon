using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SilentMoon.WebApi.Extensions
{
    public static class AuthExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["APIAppSettings:JWTSettings:Key"];
            var issuer = configuration["APIAppSettings:JWTSettings:Issuer"];
            var audience = configuration["APIAppSettings:JWTSettings:Audience"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; 
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                };

           
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        if (context.Response.HasStarted) return;

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/problem+json";

                        var problem = JsonSerializer.Serialize(new
                        {
                            type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                            title = "Unauthorized",
                            status = 401,
                            detail = "You are not authorized to access this resource.",
                            errorCode = "Auth.Unauthorized"
                        });

                        await context.Response.WriteAsync(problem);
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/problem+json";

                        var problem = JsonSerializer.Serialize(new
                        {
                            type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                            title = "Forbidden",
                            status = 403,
                            detail = "You do not have permission to access this resource.",
                            errorCode = "Auth.Forbidden"
                        });

                        await context.Response.WriteAsync(problem);
                    }
                };
            });

            services.AddAuthorization();
        }
    }
}
