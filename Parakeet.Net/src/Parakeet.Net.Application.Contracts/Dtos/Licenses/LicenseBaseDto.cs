using System;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos.Licenses
{
    public class LicenseBaseDto : EntityDto<Guid>
    {
        public string AppId { get; set; }

        public string AppKey { get; set; }

        public string AppSecret { get; set; }

        public string Token { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public string Name { get; set; }
    }
}