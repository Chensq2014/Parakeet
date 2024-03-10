using System;
using System.ComponentModel.DataAnnotations;
using Common;
using Common.Enums;
using Common.Storage;
using Serilog;
using Volo.Abp.Localization;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Parakeet.Net;

public static class NetModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            Log.Debug($"{{0}}", $"{CacheKeys.LogCount++}、NetModuleExtensionConfigurator OneTimeRunner 委托....");
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        /* You can change max lengths for properties of the
         * entities defined in the modules used by your application.
         *
         * Example: Change user and role name max lengths

           AbpUserConsts.MaxNameLength = 99;
           IdentityRoleConsts.MaxNameLength = 99;

         * Notice: It is not suggested to change property lengths
         * unless you really need it. Go with the standard values wherever possible.
         *
         * If you are using EF Core, you will need to run the add-migration command after your changes.
         */
    }

    private static void ConfigureExtraProperties()
    {
        /* You can configure extra properties for the
         * entities defined in the modules used by your application.
         *
         * This class can be used to define these extra properties
         * with a high level, easy to use API.
         *
         * Example: Add a new property to the user entity of the identity module

           ObjectExtensionManager.Instance.Modules()
              .ConfigureIdentity(identity =>
              {
                  identity.ConfigureUser(user =>
                  {
                      user.AddOrUpdateProperty<string>( //property type: string
                          "SocialSecurityNumber", //property name
                          property =>
                          {
                              //validation rules
                              property.Attributes.Add(new RequiredAttribute());
                              property.Attributes.Add(new StringLengthAttribute(64) {MinimumLength = 4});
                              
                              property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                              //...other configurations for this property
                          }
                      );
                  });
              });

         * See the documentation for more:
         * https://docs.abp.io/en/abp/latest/Module-Entity-Extensions
         */


        #region User表字段映射
        
        ObjectExtensionManager.Instance.Modules()
            .ConfigureIdentity(identity =>
            {
                identity.ConfigureUser(user =>
                {
                    user.AddOrUpdateProperty<string>( //property type: string
                        "IdCardNo", //property name
                        property =>
                        {
                            property.DisplayName = new FixedLocalizableString($"身份证号");
                            //property.DisplayName = LocalizableString.Create<NetResource>("IdCardNo");
                            //property.Attributes.Add(new RequiredAttribute());
                            property.Attributes.Add(new StringLengthAttribute(CommonConsts.MaxLength18) { MinimumLength = 15 });

                            property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                        }
                    )
                    .AddOrUpdateProperty<UserStatus?>( //property type: string
                    "UserStatus", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"用户状态");
                        //property.DisplayName = LocalizableString.Create<NetResource>("UserStatus");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<UserType?>( //property type: string
                    "UserType", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"用户类型");
                        //property.DisplayName = LocalizableString.Create<NetResource>("UserType");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<Sex?>( //property type: string
                    "Sex", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"性别");
                        //property.DisplayName = LocalizableString.Create<NetResource>("Sex");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<TreeNodeLevel?>( //property type: string
                    "Level", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"等级");
                        //property.DisplayName = LocalizableString.Create<NetResource>("Level");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<DateTime?>( //property type: string
                    "LastLoginTime", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"最近登陆时间");
                        //property.DisplayName = LocalizableString.Create<NetResource>("LastLoginTime");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<DateTime?>( //property type: string
                    "BirthDay", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"生日");
                        //property.DisplayName = LocalizableString.Create<NetResource>("BirthDay");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<string>( //property type: string
                    "SignAccountId", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"签章的SignAccountId值");
                        property.Attributes.Add(new StringLengthAttribute(CommonConsts.MaxLength255));
                        //property.DisplayName = LocalizableString.Create<NetResource>("SignAccountId");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<string>( //property type: string
                    "Motto", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"个性签名");
                        property.Attributes.Add(new StringLengthAttribute(CommonConsts.MaxLength255));
                        //property.DisplayName = LocalizableString.Create<NetResource>("Motto");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<string>( //property type: string
                    "Signature", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"个人签章照片-Base64");
                        property.Attributes.Add(new StringLengthAttribute(CommonConsts.MaxLength8192));
                        //property.DisplayName = LocalizableString.Create<NetResource>("Motto");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<string>( //property type: string
                    "HeadPortraitKey", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"头像图片Key或Base64String");
                        property.Attributes.Add(new StringLengthAttribute(CommonConsts.MaxLength8192));
                        //property.DisplayName = LocalizableString.Create<NetResource>("HeadPortraitKey");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<byte[]>( //property type: string
                    "HeadPortraitImage", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"头像图片二进制");
                        //property.DisplayName = LocalizableString.Create<NetResource>("HeadPortraitImage");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                    })
                    .AddOrUpdateProperty<bool?>( //property type: string
                    "IsRealName", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"是否完成实名认证");
                        //property.DisplayName = LocalizableString.Create<NetResource>("IsRealName");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    })
                    .AddOrUpdateProperty<bool?>( //property type: string
                    "IsCompleteGuide", //property name
                    property =>
                    {
                        property.DisplayName = new FixedLocalizableString($"是否完成新手引导");
                        //property.DisplayName = LocalizableString.Create<NetResource>("IsCompleteGuide");
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                    });
                });
            });
        
        #endregion
    }
}
