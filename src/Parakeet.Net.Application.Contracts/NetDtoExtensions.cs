using System;
using Common.Enums;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Parakeet.Net;

public static class NetDtoExtensions
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            /* You can add extension properties to DTOs
             * defined in the depended modules.
             *
             * Example:
             *
             * ObjectExtensionManager.Instance
             *   .AddOrUpdateProperty<IdentityRoleDto, string>("Title");
             *
             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Object-Extensions
             */

            ObjectExtensionManager.Instance
                .AddOrUpdateProperty<IdentityUserDto, string>("IdCardNo")
                .AddOrUpdateProperty<IdentityUserDto, UserStatus?>("UserStatus")
                .AddOrUpdateProperty<IdentityUserDto, UserType?>("UserType")
                .AddOrUpdateProperty<IdentityUserDto, Sex?>("Sex")
                .AddOrUpdateProperty<IdentityUserDto, TreeNodeLevel?>("Level")
                .AddOrUpdateProperty<IdentityUserDto, DateTime?>("LastLoginTime")
                .AddOrUpdateProperty<IdentityUserDto, DateTime?>("BirthDay")
                .AddOrUpdateProperty<IdentityUserDto, string>("SignAccountId")
                .AddOrUpdateProperty<IdentityUserDto, string>("Signature")
                .AddOrUpdateProperty<IdentityUserDto, string>("Motto")
                .AddOrUpdateProperty<IdentityUserDto, string>("HeadPortraitKey")
                .AddOrUpdateProperty<IdentityUserDto, byte[]>("HeadPortraitImage")
                .AddOrUpdateProperty<IdentityUserDto, bool?>("IsRealName")
                .AddOrUpdateProperty<IdentityUserDto, bool?>("IsCompleteGuide");
        });
    }
}
