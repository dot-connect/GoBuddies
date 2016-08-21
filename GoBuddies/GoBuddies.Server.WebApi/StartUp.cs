using GoBuddies.Server.Data;
using GoBuddies.Server.Data.DbModels;
using GoBuddies.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using System;
using System.IO;

namespace GoBuddies.Server.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                //builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            ////services.AddApplicationInsightsTelemetry(Configuration);

            // add ef
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase());

            // add swagger
           services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Geo Search API",
                    Description = "A simple api to search using geo location in Elasticsearch",
                    TermsOfService = "None"
                });

                string pathToDoc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GoBuddies.Server.WebApi.XML");
                options.IncludeXmlComments(pathToDoc);
                options.DescribeAllEnumsAsStrings();
            });

            // config identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Cookies.ApplicationCookie.AuthenticationScheme = "ApplicationCookie";
                options.Cookies.ApplicationCookie.CookieName = "Interop";
                options.Cookies.ApplicationCookie.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"C:\Identities"));
            })
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            ////app.UseApplicationInsightsRequestTelemetry();

            ////app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });

            app.UseIdentity();

            app.UseOData("odata");

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
