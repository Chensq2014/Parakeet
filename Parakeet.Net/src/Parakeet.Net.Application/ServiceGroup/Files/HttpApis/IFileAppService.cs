using System.Net.Http;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Parakeet.Net.ServiceGroup.Files.HttpApis
{
    //[TraceFilter]
    public interface IFileAppService : IHttpApi
    {
        //[HttpPost("api/dfs-file/UploadFile")]
        //Task<string> AddFileInfo([JsonContent]AddFileInfoInput input);//UploadFile

        //[HttpGet("api/dfs-file/GetFileInfo")]
        //Task<FileInfoDto> GetFileInfo(string fileId);

        //[HttpGet("api/dfs-file/GetFileInfos")]
        //Task<IPagedResult<FileInfoDto>> GetFileInfos([JsonContent]GetFileInfosInput input);

        [HttpDelete("api/dfs/file/fileInfo/{fileId}")]
        ITask<HttpResponseMessage> DeleteFileAsync(string fileId);

        //[HttpGet("api/dfs-file/CheckFileAccess")]
        //void CheckFileAccess([JsonContent]FileInfoDto fileInfo);
    }
}
