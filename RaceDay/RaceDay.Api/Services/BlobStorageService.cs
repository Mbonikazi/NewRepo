using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _container;

    public BlobStorageService(IConfiguration configuration)
    {
        // Read the connection string and container name from appsettings.json
        string connectionString = configuration["AzureBlobStorage:ConnectionString"]
            ?? throw new InvalidOperationException("Blob connection string is missing from appsettings.");

        string containerName = configuration["AzureBlobStorage:ContainerName"] ?? "raceday-images";

        // Initialize the Azure client
        _container = new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string> UploadAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file was supplied.");

        // Generate a unique filename so we don't overwrite existing images
        string extension = Path.GetExtension(file.FileName);
        string blobName = $"{folderName}/{Guid.NewGuid()}{extension}";

        // Get a reference to where the blob will be saved
        BlobClient blob = _container.GetBlobClient(blobName);

        // Open the file stream and upload it
        await using Stream stream = file.OpenReadStream();
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

        // Return the public URL of the uploaded image
        return blob.Uri.ToString();
    }
}