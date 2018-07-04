using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace MicroService.Api1
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            #region Swagger
            services.AddSwaggerGen(s =>
                     {
                         s.SwaggerDoc(Configuration["Service:DocName"], new Info
                         {
                             Title = Configuration["Service:Title"],
                             Version = Configuration["Service:Version"],
                             Description = Configuration["Service:Description"],
                             Contact = new Contact
                             {
                                 Name = Configuration["Service:Contact:Name"],
                                 Email = Configuration["Service:Contact:Email"]
                             }
                         });

                //var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //var xmlPath = Path.Combine(basePath, Configuration["Service:XmlFile"]);
                //s.IncludeXmlComments(xmlPath);
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            #region Consul

            String ip = Configuration["ip"];//部署到不同服务器的时候不能写成127.0.0.1或者0.0.0.0，因为这是让服务消费者调用的地址
            int port = int.Parse(Configuration["port"]);//获取服务端口
            var client = new ConsulClient(ConfigurationOverview); //回调获取
            string serverId = "MicroService.Api1";

            var result = client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = serverId,//服务编号保证不重复
                Name = "MsgServer",//服务的名称
                Address = ip,//服务ip地址
                Port = port,//服务端口
                Check = new AgentServiceCheck //健康检查
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后反注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔（定时检查服务是否健康）
                    HTTP = $"http://{ip}:{port}/api/Health",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)//服务的注册时间
                }
            });

            lifetime.ApplicationStopping.Register(() =>
            {
                Console.WriteLine("注销方法");
                client.Agent.ServiceDeregister(serverId).Wait();//服务停止时取消注册
            });
            #endregion
           
            #region swagger
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "doc/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint($"/doc/{Configuration["Service:DocName"]}/swagger.json",
                    $"{Configuration["Service:Name"]} {Configuration["Service:Version"]}");
            });
            #endregion
          
        }
        private static void ConfigurationOverview(ConsulClientConfiguration obj)
        {
            //consul的地址
            obj.Address = new Uri("http://127.0.0.1:8500");
            //数据中心命名
            obj.Datacenter = "dc1";
        }
    }

}
