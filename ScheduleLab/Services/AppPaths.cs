namespace ScheduleLab.Services;

public static class AppPaths
{
   
    public static string OutputFolder { get; } = CreateFolder();

    private static string CreateFolder()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ScheduleLab");
        Directory.CreateDirectory(dir);
        return dir;
    }
}
