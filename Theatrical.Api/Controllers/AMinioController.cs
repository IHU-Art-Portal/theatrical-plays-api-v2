using CommunityToolkit.HighPerformance;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Theatrical.Dto.ResponseWrapperFolder;
using Exception = System.Exception;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AMinioController : ControllerBase
{
    private readonly IMinioClient _minio;

    public AMinioController(IMinioClient minioClient)
    {
        _minio = minioClient;
    }
    
    
    [HttpDelete("DeleteFile")]
    public async Task<ActionResult<ApiResponse>> TestMinioDelete(string name)
    {
        //Remove a file from a bucket
        try
        {
            var bucketName = "api.testing";
            var objectName = name;
            

            var removeArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _minio.RemoveObjectAsync(removeArgs);

            return Ok($"Removed object {objectName} from bucket {bucketName} successfully");
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse<Exception>(e, ErrorCode.ServerError, "failed");
            return new ObjectResult(exceptionResponse);
        }
    }

    
}