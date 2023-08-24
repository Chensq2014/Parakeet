using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Parakeet.Net.Localization;
using Parakeet.Net.MultiTenancy;
using Parakeet.Net.Permissions;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Account.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace Parakeet.Net.Web.Menus;

public class NetMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public NetMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var localization = context.GetLocalizer<NetResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                NetMenus.Home,
                localization["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        #region 添加菜单
        var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();
        //var localization = context.ServiceProvider.GetRequiredService<IStringLocalizer<NetResource>>();
        var currentUer = context.ServiceProvider.GetRequiredService<ICurrentUser>();
        if (currentUer.IsAuthenticated)
        {
            context.Menu.Items.Insert(0, new ApplicationMenuItem("Net.Home", localization["Menu:Home"], "/"));
        }
        try
        {
            if (await authorizationService.IsGrantedAsync(NeedPermissions.Needs.Default))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:Need", localization["Menu:Need"], "/api/parakeet/need/index")
                //.AddItem(new ApplicationMenuItem("Menu:Need:Index", localization["Menu:Need:Index"], url: "/api/parakeet/need/index"))
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        try
        {
            if (await authorizationService.IsGrantedAsync(NeedPermissions.Needs.Create))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:NeedCreate", localization["Menu:NeedCreate"], "/api/parakeet/need/create")
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        try
        {
            if (await authorizationService.IsGrantedAsync(NetPermissions.Net.Default))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:Author", localization["Menu:Author"], "/api/parakeet/author/index")
                        .AddItem(new ApplicationMenuItem("Menu:Author:Profile", localization["Menu:Author:Profile"], url: "/api/parakeet/author/profile"))
                        .AddItem(new ApplicationMenuItem("Menu:Author:Document", localization["Menu:Author:Document"], url: "/api/parakeet/author/document"))
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        try
        {
            if (await authorizationService.IsGrantedAsync(OrganizationPermissions.Organizations.Default))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:Organization", localization["Menu:Organization"], "/api/parakeet/organization/index")
                        .AddItem(new ApplicationMenuItem("Menu:Organization:Index", localization["Menu:Organization:Index"], url: "/api/parakeet/organization/index"))
                        .AddItem(new ApplicationMenuItem("Menu:Area:Index", localization["Menu:Area:Index"], url: "/api/parakeet/areaTenant/areaTreeListIndex"))
                        .AddItem(new ApplicationMenuItem("Menu:AreaTenant:Index", localization["Menu:AreaTenant:Index"], url: "/api/parakeet/areaTenant/index"))
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        try
        {
            if (await authorizationService.IsGrantedAsync(ProjectPermissions.Projects.Default))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:Project", localization["Menu:Project"], "/api/parakeet/project/index")
                        .AddItem(new ApplicationMenuItem("Menu:Project:Index", localization["Menu:Project:Index"], url: "/api/parakeet/project/index"))
                        .AddItem(new ApplicationMenuItem("Menu:Project:Map", localization["Menu:Project:Map"], url: "/api/parakeet/project/Map"))
                        .AddItem(new ApplicationMenuItem("Menu:Project:Tiku", localization["Menu:Project:Tiku"], url: "/api/parakeet/project/Tiku"))
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        #endregion
        
    }

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var localization = context.GetLocalizer<NetResource>();
        var accountStringLocalizer = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

        context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountStringLocalizer["MyAccount"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={_configuration["App:SelfUrl"]}", icon: "fa fa-cog", order: 1000, null, "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", localization["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000).RequireAuthenticated());

        await Task.CompletedTask;
    }
}
