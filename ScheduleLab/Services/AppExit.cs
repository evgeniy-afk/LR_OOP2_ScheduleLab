namespace ScheduleLab.Services;

public static class AppExit
{
    public static bool Confirmed { get; private set; }

    public static async Task RequestAsync(IDialogService dialogs)
    {
        bool yes = await dialogs.ConfirmAsync(
            "Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
        if (!yes) return;

        Confirmed = true;
        Application.Current?.Quit();
    }
}
