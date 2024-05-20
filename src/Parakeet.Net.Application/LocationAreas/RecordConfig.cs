using Common.Dtos;
using Common.Storage;
using Serilog;
using System.Collections.Generic;

namespace Parakeet.Net.LocationAreas
{
    public class RecordConfig
    {
        public RecordConfig()
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RecordConfig)}");
        }
        public List<LocationAreaDto> LocationAreas { get; set; } = new List<LocationAreaDto>();
    }
}
