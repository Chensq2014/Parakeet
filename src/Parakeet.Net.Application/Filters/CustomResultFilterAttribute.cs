using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// Result的Filter
    /// </summary>
    public class CustomResultFilterAttribute : ResultFilterAttribute //Attribute, IResultFilter
    {
        private readonly ILogger<CustomResultFilterAttribute> _logger;

        public CustomResultFilterAttribute(ILogger<CustomResultFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogDebug($"{nameof(CustomResultFilterAttribute)} OnResultExecuted Executed!");
            //context.HttpContext.RequestServices.GetService<ILogger<CustomActionFilterAttribute>>()
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            _logger.LogDebug($"{nameof(CustomResultFilterAttribute)} OnResultExecuting Executing!");
        }
    }
}
