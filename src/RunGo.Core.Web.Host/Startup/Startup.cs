using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.EntityFrameworkCore;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SignaIR;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace RunGo.Core.Web.Host.Startup
{
    public class Startup
    {

        private readonly IConfiguration _Configuration;

        public Startup(IConfiguration Configuration)
        {
            _Configuration = Configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // MVC
            services.AddMvc(
                options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
            ).AddWebApiConventions();
            //读取配置文件
            OptionConfigure(services);
            services.AddSignalR();
            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Core API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var commentsFileName = "RunGo.Core.Application.xml";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                options.IncludeXmlComments(commentsFile);
            });

            // Configure Abp and Dependency Injection
            return services.AddAbp<CoreWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IOptions<CorsOptions> corsOptions)
        {
            app.UseAbp(); // Initializes ABP framework.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("SignaIR/Bindex.html");
            app.UseDefaultFiles(defaultFilesOptions);

            app.UseStaticFiles();
            // 利用配置文件实现
            CorsOptions _corsOption = corsOptions.Value;
            // 分割成字符串数组
            string[] hosts = _corsOption.host.Split('|', StringSplitOptions.RemoveEmptyEntries);
            // 设置跨域
            app.UseCors(options =>
            {
                options.WithOrigins(hosts);
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowCredentials();
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "DefaultApi",
                    template: "api/{controller}/{id}");
            });

            #region SignalR
            app.UseSignalR(routes =>
            {
                //登录状态推送
                routes.MapHub<LoginHub>("/loginTestHubs");
            });
            #endregion
            //app.UseWebSockets();
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API v1");
                //options.InjectJavascript("RunGo.Web.wwwroot.swagger.ui.swagger_translator.js");
                
            }); // URL: /swagger
           
        }
        /// <summary>
        /// 读取允许的域
        /// </summary>
        /// <param name="services"></param>
        private void OptionConfigure(IServiceCollection services)
        {
            services.Configure<CorsOptions>(_Configuration.GetSection("AllowedHosts"));
        }
    }
}
