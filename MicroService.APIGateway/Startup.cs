using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Swashbuckle.AspNetCore.Swagger;

namespace MicroService.APIGateway
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
            services.AddSwaggerGen(options1 =>
            {
                options1.SwaggerDoc("ApiGateway", new Info { Title = "网关服务", Version = "v1" });
            });
            #region IdentityService
            Action<IdentityServerAuthenticationOptions> options = o =>
              {
                //IdentityService认证服务的地址
                o.Authority = "http://localhost:9500";
                //IdentityService项目中Config类中定义的ApiName
                o.ApiName = "s2api"; //
                o.RequireHttpsMetadata = false;
                  o.SupportedTokens = SupportedTokens.Both;
                //IdentityService项目中Config类中定义的Secret
                o.ApiSecret = "secret";
              };
            // Ocelot
         //   services.AddOcelot(Configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOcelot()
                .AddAdministration("/admin", options); 
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }  
            loggerFactory.AddNLog();//添加NLog
            var apis = new List<string> { "MicroService.Api1", "MicroService.Api2" };
            app.UseMvc()
               .UseSwagger()
               .UseSwaggerUI(options =>
               {
                   apis.ForEach(m =>
                   {
                       options.SwaggerEndpoint($"/doc/{m}/swagger.json", m);
                   });
               });
            app.UseOcelot().Wait();
        }
    }
}
