using Common;
using Common.Enums;
using Microsoft.EntityFrameworkCore;
using System;
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
                    "IdCardNo",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CommonConsts.MaxLength18);
                    }
                ).MapEfCoreProperty<IdentityUser, UserStatus?>(
                    "UserStatus",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("用户状态");
                    }
                ).MapEfCoreProperty<IdentityUser, UserType?>(
                    "UserStatus",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("用户类型");
                    }
                ).MapEfCoreProperty<IdentityUser, UserType?>(
                    "UserType",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("用户类型");
                    }
                ).MapEfCoreProperty<IdentityUser, UserType?>(
                    "UserType",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("用户类型");
                    }
                ).MapEfCoreProperty<IdentityUser, Sex?>(
                    "Sex",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("性别");
                    }
                ).MapEfCoreProperty<IdentityUser, TreeNodeLevel?>(
                    "Level",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("等级");
                    }
                ).MapEfCoreProperty<IdentityUser, DateTime?>(
                    "LastLoginTime",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("最近登陆时间");
                    }
                ).MapEfCoreProperty<IdentityUser, DateTime?>(
                    "BirthDay",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("生日");
                    }
                ).MapEfCoreProperty<IdentityUser, string>(
                    "SignAccountId",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CommonConsts.MaxLength255);
                        propertyBuilder.HasComment("签章的SignAccountId值");
                    }
                ).MapEfCoreProperty<IdentityUser, string>(
                    "Motto",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CommonConsts.MaxLength255);
                        propertyBuilder.HasComment("个性签名");
                    }
                ).MapEfCoreProperty<IdentityUser, string>(
                    "Signature",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CommonConsts.MaxLength8192);
                        propertyBuilder.HasComment("个人签章照片-Base64");
                    }
                ).MapEfCoreProperty<IdentityUser, string>(
                    "HeadPortraitKey",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(CommonConsts.MaxLength8192);
                        propertyBuilder.HasComment("头像图片Key或Base64String");
                    }
                ).MapEfCoreProperty<IdentityUser, byte[]>(
                    "HeadPortraitImage",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("头像图片二进制");
                    }
                ).MapEfCoreProperty<IdentityUser, bool?>(
                    "IsRealName",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("是否完成实名认证");
                    }
                ).MapEfCoreProperty<IdentityUser, bool?>(
                    "IsCompleteGuide",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasComment("是否完成新手引导");
                    }
                );
        });
    }
}
