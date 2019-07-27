using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AutoMapper;
using System;
using System.Linq;

namespace RunGo.Core
{
    [DependsOn(
        typeof(CoreCoreModule), 
        typeof(AbpAspNetCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class CoreApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(CoreApplicationModule).Assembly, moduleName: "app", useConventionalHttpVerbs: true);

        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoreApplicationModule).GetAssembly());
            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => { cfg.AddProfiles(typeof(CoreApplicationModule).GetAssembly());
                    //没错就是这句话(如果你不想自动映射到这个属性的话就加上[IgnoreMapAttribute]特性吧，搞了我一个小时，简直傻屌)
                    cfg.ForAllMaps((a, b) => b.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => !opt.DestinationMember.CustomAttributes.Any(t => t.AttributeType.FullName == typeof(IgnoreMapAttribute).FullName))));}
                );
        }
    }
}
