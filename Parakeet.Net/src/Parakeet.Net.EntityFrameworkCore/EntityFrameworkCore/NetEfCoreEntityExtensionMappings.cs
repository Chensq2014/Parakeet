using System;
using Microsoft.EntityFrameworkCore;
using Parakeet.Net.Enums;
using Parakeet.Net.Users;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Parakeet.Net.EntityFrameworkCore;

public static class NetEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        NetGlobalFeatureConfigurator.Configure();
        NetModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            /* You can configure extra properties for the
             * entities defined in the modules used by your application.
             *
             * This class can be used to map these extra properties to table fields in the database.
             *
             * USE THIS CLASS ONLY TO CONFIGURE EF CORE RELATED MAPPING.
             * USE NetModuleExtensionConfigurator CLASS (in the Domain.Shared project)
             * FOR A HIGH LEVEL API TO DEFINE EXTRA PROPERTIES TO ENTITIES OF THE USED MODULES
             *
             * Example: Map a property to a table field:

                 ObjectExtensionManager.Instance
                     .MapEfCoreProperty<IdentityUser, string>(
                         "MyProperty",
                         (entityBuilder, propertyBuilder) =>
                         {
                             propertyBuilder.HasMaxLength(128);
                         }
                     );

             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Customizing-Application-Modules-Extending-Entities
             */
            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string>(
                    nameof(AppUser.IdCardNo),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(18);
                        propertyBuilder.HasComment("身份证号");
                    }
                )
                .MapEfCoreProperty<IdentityUser, UserStatus?>(
                    nameof(AppUser.UserStatus),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("用户状态");
                    }
                )
                .MapEfCoreProperty<IdentityUser, UserType?>(
                    nameof(AppUser.UserType),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("用户类型");
                    }
                )
                .MapEfCoreProperty<IdentityUser, Sex?>(
                    nameof(AppUser.Sex),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("性别");
                    }
                )
                .MapEfCoreProperty<IdentityUser, TreeNodeLevel?>(
                    nameof(AppUser.Level),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("等级");
                    }
                )
                .MapEfCoreProperty<IdentityUser, DateTime?>(
                    nameof(AppUser.LastLoginTime),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("最近登陆时间");
                    }
                )
                .MapEfCoreProperty<IdentityUser, DateTime?>(
                    nameof(AppUser.BirthDay),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("生日");
                    }
                )
                .MapEfCoreProperty<IdentityUser, string>(
                    nameof(AppUser.SignAccountId),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CustomerConsts.MaxLength255).HasComment("签章的SignAccountId值");
                    }
                )
                .MapEfCoreProperty<IdentityUser, string>(
                    nameof(AppUser.Motto),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CustomerConsts.MaxLength255).HasComment("个性签名");
                    }
                )
                .MapEfCoreProperty<IdentityUser, string>(
                    nameof(AppUser.Signature),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CustomerConsts.MaxLength8192).HasComment("个人签章照片-Base64");
                    }
                )
                .MapEfCoreProperty<IdentityUser, string>(
                    nameof(AppUser.HeadPortraitKey),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CustomerConsts.MaxLength8192).HasComment("头像图片Key或Base64String");
                    }
                )
                .MapEfCoreProperty<IdentityUser, byte[]>(
                    nameof(AppUser.HeadPortraitImage),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("头像图片二进制");
                    }
                )
                .MapEfCoreProperty<IdentityUser, bool?>(
                    nameof(AppUser.IsRealName),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("是否完成实名认证");
                    }
                )
                .MapEfCoreProperty<IdentityUser, bool?>(
                    nameof(AppUser.IsCompleteGuide),
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("是否完成新手引导");
                    }
                );
        });
    }
}
