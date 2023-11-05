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
            .WithEndpoint("127.0.0.1:9000")
            .WithCredentials("toXclgHWaUaYgQhgG8gr", "E21eMXPu9vPWwcdybP4xQp51iNKuiG8lesBrPATa")
            .Build();
    }

    [HttpGet("Test-Minio")]
    public async Task<ActionResult<ApiResponse>> TestMinio()
    {
        try
        {
            var bucketName = "test.net.core";

            //Find if a bucket exists.
            var existArg = new BucketExistsArgs().WithBucket(bucketName);
            var found = await _minio.BucketExistsAsync(existArg);
            //Create the bucket if it does not exist.
            if (!found)
            {
                await _minio.MakeBucketAsync(
                    new MakeBucketArgs()
                        .WithBucket(bucketName)
                    )
                    .ConfigureAwait(false);
            }

            //Creates a test.text file and uploads it to the bucket.
            var filePath = "sample.pdf";
            var objectName = "sample.pdf"; //Change to have the name of the user.
            ReadOnlyMemory<byte> bs = await System.IO.File.ReadAllBytesAsync(filePath);
            using var filestream = bs.AsStream();

            var putArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(filestream)
                .WithObjectSize(filestream.Length)
                .WithContentType("application/pdf");

            await _minio.PutObjectAsync(putArgs);

            Console.WriteLine($"Uploaded object {objectName} to bucket {bucketName}");

            //

            return Ok(bucketName);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse<Exception>(e, ErrorCode.ServerError, "failed");
            return new ObjectResult(exceptionResponse);
        }
    }
    
    
    [HttpDelete("DeleteFile")]
    public async Task<ActionResult<ApiResponse>> TestMinioDelete(string name)
    {
        //Remove a file from a bucket
        try
        {
            var bucketName = "dev";
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