using System;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Localization;
using Parakeet.Net.Permissions;
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
    //private readonly IAuthorizationService _authorizationService;
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
        var l = context.GetLocalizer<NetResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                NetMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        if (CommonConsts.MultiTenancyEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        await AddMenuItems(context);
    }

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<NetResource>();
        var accountStringLocalizer = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

        context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountStringLocalizer["MyAccount"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={_configuration["App:SelfUrl"]}", icon: "fa fa-cog", order: 1000, null, "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", l["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000).RequireAuthenticated());

        //await AddMenuItems(context);
        await Task.CompletedTask;
    }

    private async Task AddMenuItems(MenuConfigurationContext context)
    {
        var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();
        var l = context.GetLocalizer<NetResource>();
        //var currentUer = context.ServiceProvider.GetRequiredService<ICurrentUser>();
        //if (currentUer.IsAuthenticated)
        //{
        //    //context.Menu.Items.Insert(0, new ApplicationMenuItem("Net.Home", l["Menu:Home"], "/"));
        //}
        #region 添加菜单

        try
        {
            if (await authorizationService.IsGrantedAsync(NeedPermissions.Needs.Default))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:Need", l["Menu:Need"], "/api/parakeet/need/index")
                //.AddItem(new ApplicationMenuItem("Menu:Need:Index", l["Menu:Need:Index"], url: "/api/parakeet/need/index"))
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
                    new ApplicationMenuItem("Menu:NeedCreate", l["Menu:NeedCreate"], "/api/parakeet/need/create")
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
                    new ApplicationMenuItem("Menu:Author", l["Menu:Author"], "/api/parakeet/author/index")
                        .AddItem(new ApplicationMenuItem("Menu:Author:Profile", l["Menu:Author:Profile"], url: "/api/parakeet/author/profile"))
                        .AddItem(new ApplicationMenuItem("Menu:Author:Document", l["Menu:Author:Document"], url: "/api/parakeet/author/document"))
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
                    new ApplicationMenuItem("Menu:Organization", l["Menu:Organization"], "/api/parakeet/organization/index")
                        .AddItem(new ApplicationMenuItem("Menu:Organization:Index", l["Menu:Organization:Index"], url: "/api/parakeet/organization/index"))
                        .AddItem(new ApplicationMenuItem("Menu:Area:Index", l["Menu:Area:Index"], url: "/api/parakeet/areaTenant/areaTreeListIndex"))
                        .AddItem(new ApplicationMenuItem("Menu:AreaTenant:Index", l["Menu:AreaTenant:Index"], url: "/api/parakeet/areaTenant/index"))
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
                    new ApplicationMenuItem("Menu:Project", l["Menu:Project"], "/api/parakeet/project/index")
                        .AddItem(new ApplicationMenuItem("Menu:Project:Index", l["Menu:Project:Index"], url: "/api/parakeet/project/index"))
                        .AddItem(new ApplicationMenuItem("Menu:Project:Map", l["Menu:Project:Map"], url: "/api/parakeet/project/Map"))
                        .AddItem(new ApplicationMenuItem("Menu:Project:Tiku", l["Menu:Project:Tiku"], url: "/api/parakeet/project/Tiku"))
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        try
        {
            if (await authorizationService.IsGrantedAsync(SectionPermissions.Sections.Default))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:Section", l["Menu:Section"], "/api/parakeet/section/workerIndex")
                        .AddItem(new ApplicationMenuItem("Menu:Section:WorkerIndex", l["Menu:Section:WorkerIndex"], url: "/api/parakeet/section/workerIndex"))
                        .AddItem(new ApplicationMenuItem("Menu:Section:WorkerDetailIndex", l["Menu:Section:WorkerDetailIndex"], url: "/api/parakeet/section/WorkerDetailIndex"))
                        .AddItem(new ApplicationMenuItem("Menu:Section:Index", l["Menu:Section:Index"], url: "/api/parakeet/section/index"))
                        .AddItem(new ApplicationMenuItem("Menu:Section:ProductIndex", l["Menu:Section:ProductIndex"], url: "/api/parakeet/section/ProductIndex"))
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        try
        {
            if (await authorizationService.IsGrantedAsync(WorkerTypePermissions.WorkerType.Default))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:WorkerType", l["Menu:WorkerType"], "/api/parakeet/section/worker")
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        try
        {
            if (await authorizationService.IsGrantedAsync(WorkerPermissions.Worker.Default))
            {
                context.Menu.AddItem(
                    new ApplicationMenuItem("Menu:Worker", l["Menu:Worker"], "/api/parakeet/section/workerType")
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        #endregion
    }

}
