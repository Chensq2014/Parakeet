using System;
using System.Collections.Generic;
using System.Text;
using Parakeet.Net.Localization;
using Volo.Abp.Application.Services;

namespace Parakeet.Net;

/* Inherit your application services from this class.
 */
public abstract class NetAppService : ApplicationService
{
    protected NetAppService()
    {
        LocalizationResource = typeof(NetResource);
    }
}
