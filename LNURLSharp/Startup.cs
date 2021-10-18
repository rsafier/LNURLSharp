using LNDroneController.LND;
using LNURLSharp.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
            LNURLSettings lnurlSettings = Configuration.GetSection("LNURLSettings").Get<LNURLSettings>();

            var myLndNode = new LNDNodeConnection(lnurlSettings.LNDNodes.First());
            services.AddSingleton(myLndNode);
            services.AddControllers();
            services.AddHttpContextAccessor();

            services.AddDbContext<LNURLContext>();
            if (lnurlSettings.EnableTorEndpoint)
            {
                throw new NotImplementedException("I'll get to it.");
            }

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardLimit = 1;
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            services.Configure<HttpsRedirectionOptions>(options =>
            {
                options.HttpsPort = 443;
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LNURLContext dbContext)
        {
            dbContext.Database.Migrate(); //autocreate DB or apply new migrations

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
            //app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
