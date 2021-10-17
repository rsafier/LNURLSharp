using LNDroneController.LND;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LNURLSharp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            LNDSettings lndSettings = new LNDSettings();
            LNURLSettings lnurlSettings = new LNURLSettings();

            Configuration.GetSection("LNDSettings").Bind(lndSettings);
            Configuration.GetSection("LNURLSettings").Bind(lnurlSettings);

            var myLndNode = new LNDNodeConnection(lndSettings);
            services.AddControllers(options =>
            {
                var jsonInputFormatter = options.InputFormatters
                .OfType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>()
                .Single();
                jsonInputFormatter.SupportedMediaTypes.Add("*/*");
            });
            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddSingleton(myLndNode);

            if (lnurlSettings.EnableTorEndpoint)
            {
                throw new NotImplementedException("I'll get to it.");
            }

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardLimit = 1;
                //  options.AllowedHosts = new List<string> { "127.0.0.1", "localhost", "::1" };

                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit 
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            services.Configure<HttpsRedirectionOptions>(options =>
            {
                options.HttpsPort = 443;
            });

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
           //     app.UseHsts();
            }
            app.UseForwardedHeaders();

            app.UseHttpsRedirection();
         //   app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
