using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eAuth.Utils;

namespace eAuth
{
    public class Startup
    {
        private static ILog logger = LogManager.GetLogger("Global");
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            this.InitLogger();
        }

        private void InitLogger()
        {
            //init elmah logger
            //ElmahLogger.Instance.Info("EDelivery WebPortal is starting...");
            //init log4net logger
            var path = LogHelper.LogConfiguration.LogConfigurationPath;
            if (!Path.IsPathRooted(path))
            {
                var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
                path = Path.Combine(assemblyPath, path);
            }

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
            LogManager.GetLogger("Global").Debug("Logging is initialized!");
        }

    }
}
