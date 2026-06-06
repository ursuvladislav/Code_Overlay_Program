using System.Windows;

namespace CodeOverlayRunner.Services
{
    public static class ClipboardService
    {
        public static string? GetText()
        {
            if (!Clipboard.ContainsText())
                return null;

            string text = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(text))
                return null;

            return text;
        }
    }
}