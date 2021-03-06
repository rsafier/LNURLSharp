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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.OpenApi.Models;

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

            services.AddDbContext<LNURLContext>();

            services.AddSingleton(myLndNode);
            services.AddControllers();
            services.AddHttpContextAccessor();

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Libre.Lightning", Version = "v1" });
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "ApiKey must appear in header",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "ApiKey",
                    In = ParameterLocation.Header
                });
                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                {
                   { key, new List<string>() }
                };
                c.AddSecurityRequirement(requirement);
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LNURLContext dbContext, LNDNodeConnection nodeConnection, ILogger<Startup> logger)
        {
            dbContext.Database.Migrate(); //autocreate DB or apply new migrations
            
            if (!dbContext.LNDServers.Any(t => t.Pubkey == nodeConnection.LocalNodePubKey))
            {
                var lnd = dbContext.LNDServers.Add(new LNDServer
                {
                    Pubkey = nodeConnection.LocalNodePubKey
                });
                dbContext.SaveChanges();
                logger.LogInformation("Node {Pubkey} added to db.",nodeConnection.LocalNodePubKey);
            }
            else
            {
                logger.LogInformation("Node {Pubkey} exists in db.", nodeConnection.LocalNodePubKey);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Libre.Lightning v1"));
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
