using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RequestLocalization;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;

namespace Parakeet.Net.MultiTenancy
{
    /// <summary>
    /// 重写MultiTenancyMiddleware
    /// </summary>
    public class CustomMultiTenancyMiddleware : IMiddleware, ITransientDependency
    {
        private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

        private readonly ICurrentTenant _currentTenant;

        private readonly AbpAspNetCoreMultiTenancyOptions _options;

        private readonly ITenantResolveResultAccessor _tenantResolveResultAccessor;

        public CustomMultiTenancyMiddleware(ITenantConfigurationProvider tenantConfigurationProvider, ICurrentTenant currentTenant, IOptions<AbpAspNetCoreMultiTenancyOptions> options, ITenantResolveResultAccessor tenantResolveResultAccessor)
        {
            _tenantConfigurationProvider = tenantConfigurationProvider;
            _currentTenant = currentTenant;
            _tenantResolveResultAccessor = tenantResolveResultAccessor;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            TenantConfiguration tenant = null;
            try
            {
                tenant = await _tenantConfigurationProvider.GetAsync(saveResolveResult: true).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception arg)
            {
                if (await _options.MultiTenancyMiddlewareErrorPageBuilder(context, arg).ConfigureAwait(continueOnCapturedContext: false))
                {
                    return;
                }
            }

            if (tenant?.Id != _currentTenant.Id)
            {
                using (_currentTenant.Change(tenant?.Id, tenant?.Name))
                {
                    if (_tenantResolveResultAccessor.Result != null && _tenantResolveResultAccessor.Result.AppliedResolvers.Contains("QueryString"))
                    {
                        AbpMultiTenancyCookieHelper.SetTenantCookie(context, _currentTenant.Id, _options.TenantKey);
                    }

                    RequestCulture requestCulture = await TryGetRequestCultureAsync(context).ConfigureAwait(continueOnCapturedContext: false);
                    if (requestCulture != null)
                    {
                        CultureInfo.CurrentCulture = requestCulture.Culture;
                        CultureInfo.CurrentUICulture = requestCulture.UICulture;
                        AbpRequestCultureCookieHelper.SetCultureCookie(context, requestCulture);
                        context.Items["__AbpSetCultureCookie"] = true;
                    }

                    await next(context).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            else
            {
                await next(context).ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        private async Task<RequestCulture> TryGetRequestCultureAsync(HttpContext httpContext)
        {
            IRequestCultureFeature requestCultureFeature = httpContext.Features.Get<IRequestCultureFeature>();
            if (requestCultureFeature == null)
            {
                return null;
            }

            if (requestCultureFeature.Provider != null)
            {
                return null;
            }

            ISettingProvider requiredService = httpContext.RequestServices.GetRequiredService<ISettingProvider>();
            string text = await requiredService.GetOrNullAsync("Abp.Localization.DefaultLanguage").ConfigureAwait(continueOnCapturedContext: false);
            if (text.IsNullOrWhiteSpace())
            {
                return null;
            }

            string culture;
            string uiCulture;
            if (text.Contains(';'))
            {
                string[] array = text.Split(';');
                culture = array[0];
                uiCulture = array[1];
            }
            else
            {
                culture = text;
                uiCulture = text;
            }

            return new RequestCulture(culture, uiCulture);
        }
    }
}
