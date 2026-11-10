using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using ScheduleLab.Logging;
using ScheduleLab.Models;
using ScheduleLab.Saving;
using ScheduleLab.Services;
using ScheduleLab.Strategies;

namespace ScheduleLab.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IDialogService _dialogs;
    private readonly GoogleDriveService _drive;
    private readonly XmlValuesLoader _valuesLoader;
    private readonly HtmlTransformer _transformer;
    private readonly SearchContext _searchContext;

    private string? _xmlPath;
    private string? _xslPath;
    private List<Lesson>? _lastResults;

    public MainViewModel(IDialogService dialogs, GoogleDriveService drive,
        XmlValuesLoader valuesLoader, HtmlTransformer transformer,
        IEnumerable<ISearchStrategy> strategies)
    {
        _dialogs = dialogs;
        _drive = drive;
        _valuesLoader = valuesLoader;
        _transformer = transformer;

        Strategies = new ObservableCollection<ISearchStrategy>(strategies);
        _selectedStrategy = Strategies[0];
        _searchContext = new SearchContext(_selectedStrategy);

        OpenLocalXmlCommand = new Command(async () => await RunSafeAsync(OpenLocalXmlAsync));
        OpenDriveXmlCommand = new Command(async () => await RunSafeAsync(OpenDriveXmlAsync));
        OpenLocalXslCommand = new Command(async () => await RunSafeAsync(OpenLocalXslAsync));
        OpenDriveXslCommand = new Command(async () => await RunSafeAsync(OpenDriveXslAsync));
        SearchCommand = new Command(async () => await RunSafeAsync(SearchAsync));
        TransformCommand = new Command(async () => await RunSafeAsync(TransformAsync));
        SaveXmlToDriveCommand = new Command(async () => await RunSafeAsync(SaveXmlToDriveAsync));
        SaveHtmlToDriveCommand = new Command(async () => await RunSafeAsync(SaveHtmlToDriveAsync));
        ClearCommand = new Command(Clear);
        ExitCommand = new Command(async () => await AppExit.RequestAsync(_dialogs));
    }

    

    public ObservableCollection<ISearchStrategy> Strategies { get; }

    private ISearchStrategy _selectedStrategy;
    public ISearchStrategy SelectedStrategy
    {
        get => _selectedStrategy;
        set
        {
            if (value != null && SetProperty(ref _selectedStrategy, value))
                _searchContext.SetStrategy(value);
        }
    }

    public ObservableCollection<string> Days { get; } = new();
    public ObservableCollection<string> Types { get; } = new();
    public ObservableCollection<string> Subjects { get; } = new();
    public ObservableCollection<string> Teachers { get; } = new();
    public ObservableCollection<string> Departments { get; } = new();
    public ObservableCollection<string> Groups { get; } = new();

    private string? _selectedDay;
    public string? SelectedDay { get => _selectedDay; set => SetProperty(ref _selectedDay, value); }

    private string? _selectedType;
    public string? SelectedType { get => _selectedType; set => SetProperty(ref _selectedType, value); }

    private string? _selectedSubject;
    public string? SelectedSubject { get => _selectedSubject; set => SetProperty(ref _selectedSubject, value); }

    private string? _selectedTeacher;
    public string? SelectedTeacher { get => _selectedTeacher; set => SetProperty(ref _selectedTeacher, value); }

    private string? _selectedDepartment;
    public string? SelectedDepartment { get => _selectedDepartment; set => SetProperty(ref _selectedDepartment, value); }

    private string? _selectedGroup;
    public string? SelectedGroup { get => _selectedGroup; set => SetProperty(ref _selectedGroup, value); }

    private string _xmlFileName = "файл не вибрано";
    public string XmlFileName { get => _xmlFileName; set => SetProperty(ref _xmlFileName, value); }

    private string _xslFileName = "файл не вибрано";
    public string XslFileName { get => _xslFileName; set => SetProperty(ref _xslFileName, value); }

    private string _resultText = "";
    public string ResultText { get => _resultText; set => SetProperty(ref _resultText, value); }

    private HtmlWebViewSource _htmlPreview = new() { Html = "" };
    public HtmlWebViewSource HtmlPreview { get => _htmlPreview; set => SetProperty(ref _htmlPreview, value); }

    

    public ICommand OpenLocalXmlCommand { get; }
    public ICommand OpenDriveXmlCommand { get; }
    public ICommand OpenLocalXslCommand { get; }
    public ICommand OpenDriveXslCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand TransformCommand { get; }
    public ICommand SaveXmlToDriveCommand { get; }
    public ICommand SaveHtmlToDriveCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ExitCommand { get; }

    private async Task OpenLocalXmlAsync()
    {
        string? path = await _dialogs.PickFileAsync("Оберіть XML-файл", ".xml");
        if (path != null) LoadXml(path);
    }

    private async Task OpenDriveXmlAsync()
    {
        string? path = await DownloadFromDriveAsync(".xml");
        if (path != null) LoadXml(path);
    }

    private async Task OpenLocalXslAsync()
    {
        string? path = await _dialogs.PickFileAsync("Оберіть XSL-файл", ".xsl");
        if (path != null) SetXsl(path);
    }

    private async Task OpenDriveXslAsync()
    {
        string? path = await DownloadFromDriveAsync(".xsl");
        if (path != null) SetXsl(path);
    }

    private async Task<string?> DownloadFromDriveAsync(string extension)
    {
        var files = await _drive.ListFilesAsync(extension);
        if (files.Count == 0)
        {
            await _dialogs.AlertAsync("Google Drive", $"На Google Drive немає файлів {extension}.");
            return null;
        }

        string? chosen = await _dialogs.ChooseAsync($"Оберіть файл {extension}",
            files.Select(f => f.Name).ToArray());
        var file = files.FirstOrDefault(f => f.Name == chosen);
        if (file == null) return null;

        string localPath = Path.Combine(FileSystem.CacheDirectory, file.Name);
        await _drive.DownloadAsync(file.Id, localPath);
        return localPath;
    }

    private void LoadXml(string path)
    {
        
        ScheduleValues values = _valuesLoader.Load(path);
        _xmlPath = path;
        XmlFileName = Path.GetFileName(path);

        Clear();
        Refill(Days, values.Days);
        Refill(Types, values.Types);
        Refill(Subjects, values.Subjects);
        Refill(Teachers, values.Teachers);
        Refill(Departments, values.Departments);
        Refill(Groups, values.Groups);
    }

    private void SetXsl(string path)
    {
        _xslPath = path;
        XslFileName = Path.GetFileName(path);
    }

    private static void Refill(ObservableCollection<string> target, IEnumerable<string> values)
    {
        target.Clear();
        foreach (var v in values) target.Add(v);
    }



    private async Task SearchAsync()
    {
        if (!await EnsureXmlAsync()) return;

        var criteria = new SearchCriteria
        {
            Day = SelectedDay,
            Type = SelectedType,
            Subject = SelectedSubject,
            Teacher = SelectedTeacher,
            Department = SelectedDepartment,
            Group = SelectedGroup
        };

        _lastResults = _searchContext.ExecuteSearch(_xmlPath!, criteria);
        ResultText = FormatResults(_lastResults);

        Logger.Instance.Log(LogEventType.Filtering,
            $"Параметри: {criteria}. Спосіб: {SelectedStrategy.Name}. Знайдено: {_lastResults.Count}");
    }

    private string FormatResults(List<Lesson> lessons)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Спосіб аналізу: {SelectedStrategy.Name}");
        sb.AppendLine($"Знайдено занять: {lessons.Count}");
        sb.AppendLine(new string('-', 60));
        if (lessons.Count == 0)
            sb.AppendLine("За заданими параметрами нічого не знайдено.");
        foreach (var lesson in lessons)
            sb.AppendLine(lesson.ToString());
        return sb.ToString();
    }

    

    private async Task TransformAsync()
    {
        if (!await EnsureXmlAsync() || !await EnsureXslAsync()) return;

        var fragment = FilteredXmlBuilder.Build(_xmlPath!, _lastResults);
        string html = _transformer.TransformToString(fragment, _xslPath!);

        string fileName = $"{Path.GetFileNameWithoutExtension(_xmlPath)}_{DateTime.Now:yyyyMMdd_HHmmss}.html";
        string outPath = Path.Combine(AppPaths.OutputFolder, fileName);
        await File.WriteAllTextAsync(outPath, html, Encoding.UTF8);

        HtmlPreview = new HtmlWebViewSource { Html = html };
        Logger.Instance.Log(LogEventType.Transformation, $"Збережено у файл {fileName}");
        await _dialogs.AlertAsync("Трансформація", $"HTML збережено у файл:\n{outPath}");
    }

    

    private async Task SaveXmlToDriveAsync()
    {
        if (!await EnsureXmlAsync()) return;
        await SaveToDriveAsync(new XmlDriveSaver(_drive));
    }

    private async Task SaveHtmlToDriveAsync()
    {
        if (!await EnsureXmlAsync() || !await EnsureXslAsync()) return;
        await SaveToDriveAsync(new HtmlDriveSaver(_drive, _transformer, _xslPath!));
    }

    private async Task SaveToDriveAsync(FragmentSaver saver)
    {
        string? name = await _dialogs.PromptAsync("Збереження на Google Drive",
            $"Назва файлу ({saver.Extension}):", "schedule_filtered");
        if (string.IsNullOrWhiteSpace(name)) return;

        var fragment = FilteredXmlBuilder.Build(_xmlPath!, _lastResults);
        string savedName = await saver.SaveAsync(fragment, name.Trim());

        Logger.Instance.Log(LogEventType.Saving,
            $"Збережено відфільтрований фрагмент у файл {savedName} на Google Drive");
        await _dialogs.AlertAsync("Google Drive", $"Файл {savedName} збережено на Google Drive.");
    }

    

    private void Clear()
    {
        SelectedDay = null;
        SelectedType = null;
        SelectedSubject = null;
        SelectedTeacher = null;
        SelectedDepartment = null;
        SelectedGroup = null;
        ResultText = "";
        HtmlPreview = new HtmlWebViewSource { Html = "" };
        _lastResults = null;
    }

    private async Task<bool> EnsureXmlAsync()
    {
        if (_xmlPath != null) return true;
        await _dialogs.AlertAsync("Увага", "Спочатку відкрийте XML-файл.");
        return false;
    }

    private async Task<bool> EnsureXslAsync()
    {
        if (_xslPath != null) return true;
        await _dialogs.AlertAsync("Увага", "Спочатку відкрийте XSL-файл.");
        return false;
    }

    private async Task RunSafeAsync(Func<Task> action)
    {
        try { await action(); }
        catch (Exception ex) { await _dialogs.AlertAsync("Помилка", ex.Message); }
    }
}
