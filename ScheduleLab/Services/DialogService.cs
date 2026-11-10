namespace ScheduleLab.Services;

public interface IDialogService
{
    Task AlertAsync(string title, string message);
    Task<bool> ConfirmAsync(string title, string message, string accept, string cancel);
    Task<string?> ChooseAsync(string title, string[] options);
    Task<string?> PromptAsync(string title, string message, string initialValue);
    Task<string?> PickFileAsync(string title, string extension);
}

public class DialogService : IDialogService
{
    private const string CancelText = "Скасувати";

    private static Page CurrentPage => Application.Current!.Windows[0].Page!;

    public Task AlertAsync(string title, string message) =>
        CurrentPage.DisplayAlert(title, message, "OK");

    public Task<bool> ConfirmAsync(string title, string message, string accept, string cancel) =>
        CurrentPage.DisplayAlert(title, message, accept, cancel);

    public async Task<string?> ChooseAsync(string title, string[] options)
    {
        string? result = await CurrentPage.DisplayActionSheet(title, CancelText, null, options);
        return result == null || result == CancelText ? null : result;
    }

    public Task<string?> PromptAsync(string title, string message, string initialValue) =>
        CurrentPage.DisplayPromptAsync(title, message, "OK", CancelText, initialValue: initialValue);

    public async Task<string?> PickFileAsync(string title, string extension)
    {
        var types = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { extension } },
            { DevicePlatform.MacCatalyst, new[] { extension.TrimStart('.') } }
        });

        FileResult? result = await FilePicker.Default.PickAsync(
            new PickOptions { PickerTitle = title, FileTypes = types });

        return result?.FullPath;
    }
}
