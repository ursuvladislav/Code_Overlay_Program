namespace CodeOverlayRunner.Services
{
    public enum InterfaceLanguage
    {
        English,
        Russian,
        Romanian
    }

    public enum ProgrammingLanguageMode
    {
        Auto,
        Cpp,
        Python,
        CSharp,
        JavaScript
    }

    public class AppSettings
    {
        public InterfaceLanguage InterfaceLanguage { get; set; } = InterfaceLanguage.English;
        public ProgrammingLanguageMode LanguageMode { get; set; } = ProgrammingLanguageMode.Cpp;

        public int ExecutionTimeoutMs { get; set; } = 2000;

        public string CompileHotkey { get; set; } = "Ctrl + Shift + Space";
        public string OpenWindowHotkey { get; set; } = "Ctrl + Shift + Alt + O";
        
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