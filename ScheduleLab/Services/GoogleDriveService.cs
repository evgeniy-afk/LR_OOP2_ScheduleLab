using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace ScheduleLab.Services;

public record DriveFileInfo(string Id, string Name);

public class GoogleDriveService
{
    private DriveService? _service;

    private async Task<DriveService> GetServiceAsync()
    {
        if (_service != null) return _service;

        Stream stream;
        try
        {
            stream = await FileSystem.OpenAppPackageFileAsync("credentials.json");
        }
        catch (Exception)
        {
            throw new FileNotFoundException(
                "Не знайдено credentials.json. Покладіть його в папку Resources/Raw проєкту " +
                "і перезапустіть програму (див. README.txt).");
        }

        UserCredential credential;
        using (stream)
        {
            var secrets = GoogleClientSecrets.FromStream(stream).Secrets;
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None,
                new FileDataStore(Path.Combine(FileSystem.AppDataDirectory, "google-token"), true));
        }

        _service = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "ScheduleLab"
        });
        return _service;
    }

    public async Task<List<DriveFileInfo>> ListFilesAsync(string extension)
    {
        var service = await GetServiceAsync();
        var request = service.Files.List();
        request.Q = "trashed = false and mimeType != 'application/vnd.google-apps.folder'";
        request.Fields = "files(id, name)";
        request.PageSize = 200;

        var response = await request.ExecuteAsync();
        return response.Files
            .Where(f => f.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            .Select(f => new DriveFileInfo(f.Id, f.Name))
            .ToList();
    }

    public async Task DownloadAsync(string fileId, string localPath)
    {
        var service = await GetServiceAsync();
        await using var file = File.Create(localPath);
        var progress = await service.Files.Get(fileId).DownloadAsync(file);
        if (progress.Status != DownloadStatus.Completed)
            throw new IOException("Не вдалося завантажити файл з Google Drive.", progress.Exception);
    }

    public async Task UploadAsync(string fileName, Stream content, string mimeType)
    {
        var service = await GetServiceAsync();
        var metadata = new DriveFile { Name = fileName };
        var request = service.Files.Create(metadata, content, mimeType);
        request.Fields = "id";

        var progress = await request.UploadAsync();
        if (progress.Status != UploadStatus.Completed)
            throw new IOException("Не вдалося зберегти файл на Google Drive.", progress.Exception);
    }
}
