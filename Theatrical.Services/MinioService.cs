using CommunityToolkit.HighPerformance;
using Minio;
using Minio.DataModel.Args;

namespace Theatrical.Services;


public interface IMinioService
{
    Task<string> PostPdf(byte[] pdfBytes, int userId);
}

public class MinioService : IMinioService
{
    private string _bucketName = "api.testing";
    private readonly IMinioClient minioClient;

    public MinioService()
    {
        minioClient = new MinioClient()
            .WithEndpoint("play.min.io")
            .WithCredentials("HsIq0yLoY6AlqZdFMbZX", "0Jwvg6v4v6BxcTM9k2re5qV56RRjktCaodX7Dffa")
            .Build();
    }

    public async Task<string> PostPdf(byte[] pdfBytes, int userId)
    {

        var existArg = new BucketExistsArgs().WithBucket(_bucketName);
        var found = await minioClient.BucketExistsAsync(existArg);
        
        //Creates the bucket if not exists.
        if (!found)
        {
            await minioClient.MakeBucketAsync(
                new MakeBucketArgs()
                    .WithBucket(_bucketName)
            );
        }
        
        var objectName = $"IdentificationDocument{userId}.pdf";
        ReadOnlyMemory<byte> bs = new ReadOnlyMemory<byte>(pdfBytes);

        using var fileStream = bs.AsStream();

        var putArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType("application/pdf");
        
        await minioClient.PutObjectAsync(putArgs);

        return await GetDownloadLink(objectName);
    }

    //Get the download link a pdf object.
    private async Task<string> GetDownloadLink(string objectName)
    {
        
        var reqParams = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "response-content-type", "application/pdf" }
        };
        
        var presignedArgs = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithExpiry(1000)
            .WithHeaders(reqParams);
        
        var url = await minioClient.PresignedGetObjectAsync(presignedArgs);
        return url;
    }
    
}