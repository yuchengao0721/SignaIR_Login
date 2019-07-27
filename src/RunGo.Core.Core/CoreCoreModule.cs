using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace RunGo.Core
{

    public class CoreCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoreCoreModule).GetAssembly());
        }
    }
}
