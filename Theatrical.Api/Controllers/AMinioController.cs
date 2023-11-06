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

    public AMinioController()
    {
        _minio = new MinioClient()
            .WithEndpoint("play.min.io")
            .WithCredentials("Q3AM3UQ867SPQQA43P2F", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG")
            .Build();
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