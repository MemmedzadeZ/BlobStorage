namespace BlobStorage32205.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName);
        Task<Stream> DownloadAsync(string fileName);
        Task<List<string>> ListFilesAsync();
        Task DeleteAsync(string fileName);

    }
}