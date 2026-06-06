namespace CodeOverlayRunner.Services
{
    public class AppSettings
    {
        public int TimeoutMs { get; set; } = 2000;
        public bool StartHidden { get; set; } = true;
        public bool ShowOverlay { get; set; } = true;
    }

    public static class SettingsService
    {
        public static AppSettings Current { get; private set; } = new AppSettings();

        public static void Load()
        {
            // позже загрузим settings.json
        }

        public static void Save()
        {
            // позже сохраним settings.json
        }
    }
}