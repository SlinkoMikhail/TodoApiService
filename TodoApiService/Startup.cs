using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoApiService.Models;
using TodoApiService.Models.Options;

namespace TodoApiService
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<TodoApiApplicationContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("TodoApiApplicationDb"));
            });

            services.Configure<JWTAuthOptions>(Configuration.GetSection("JWTAuth"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwt =>
                {
                    jwt.SaveToken = true;
                    JWTAuthOptions jwtAuthOptions = Configuration.Get<JWTAuthOptions>();
                    jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtAuthOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAuthOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = jwtAuthOptions.GetSymmetricSecurityKey(),
            
                        ValidateLifetime = true,
                        
                        ClockSkew = TimeSpan.Zero//jwtAuthOptions.ClockSkew
                    };
                });
            services.AddTransient<IAccountManager, AccountManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
