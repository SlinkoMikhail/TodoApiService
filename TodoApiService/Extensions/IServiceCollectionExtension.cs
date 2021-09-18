using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using TodoApiService.Models.Options;

namespace TodoApiService.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static void AddJWTAuthentication(this IServiceCollection services, JWTAuthOptions jwtAuthOptions)
        {   
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwt =>
                {
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtAuthOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAuthOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = jwtAuthOptions.GetSymmetricSecurityKey(),
            
                        ValidateLifetime = true,
                        
                        ClockSkew = jwtAuthOptions.ClockSkew
                    };
                });
        }
    }
}