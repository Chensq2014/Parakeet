using Common.Dtos;

namespace Parakeet.Net.ServiceGroup.Files.HttpModels
{
    public class GetFileInfosInput : PagedInputDto
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }
    }
}
