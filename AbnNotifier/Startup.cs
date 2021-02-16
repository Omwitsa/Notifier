using AbnNotifier.Controllers;
using AbnNotifier.Data.Notifier;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unisol.Web.Common.Utilities;

namespace AbnNotifier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
			var notifierDbConnection = DbSetting.ConnectionString(Configuration, "Portal");
			services.AddDbContext<AbnNotifierDbContext>(options => options.UseSqlServer(notifierDbConnection));
			services.AddHangfire(x => x.UseSqlServerStorage(notifierDbConnection));
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireServer();
            app.UseHangfireDashboard("/api/schedule");

            app.UseMvc();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AbnNotifierDbContext>();
                context.Database.Migrate();

                var values = new V1Controller(Configuration, context);
                RecurringJob.AddOrUpdate(() => values.SendNotification(), Cron.MinuteInterval(5));
            }
        }
    }
}
