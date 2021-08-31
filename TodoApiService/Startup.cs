using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoApiService.Extensions;
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

            var jwtConfig = Configuration.GetSection("JWTAuthOptions");
            services.Configure<JWTAuthOptions>(jwtConfig);

            services.AddJWTAuthentication(jwtConfig.Get<JWTAuthOptions>());
            services.AddTransient<IAccountManager, AccountManager>();
            services.AddTransient<ITodoItemsRepository, EFTodoItemsRepository>();
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
