using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace RunGo.Core.Web.Host.Startup
{
    [DependsOn(
        typeof(CoreApplicationModule),
        typeof(AbpAspNetCoreModule)
        )]
    public class CoreWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public CoreWebHostModule(IHostingEnvironment env)
        {
        }
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(CoreApplicationModule).GetAssembly()
                );
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoreWebHostModule).GetAssembly());
            //_IDynamicApiControllerBuilder
            //.ForAll<IApplicationService>(typeof(CoreApplicationModule).Assembly, "v1")
            //.Build();
        }
    }
}
