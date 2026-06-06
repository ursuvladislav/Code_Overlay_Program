using System;
using System.Threading.Tasks;
using System.Windows;
using CodeOverlayRunner.Services;
using System.Diagnostics;

namespace CodeOverlayRunner
{
    public partial class MainWindow : Window
    {
        private readonly HotkeyService _hotkeyService;
        private readonly OverlayWindow _overlay;

        private bool _isRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            _overlay = new OverlayWindow();
            _hotkeyService = new HotkeyService(this);

            _hotkeyService.OpenMainWindowPressed += (_, _) =>
            {
                Dispatcher.Invoke(() =>
                {
                    _isRunning = false;

                    _hotkeyService.Unregister();

                    Show();
                    WindowState = WindowState.Normal;
                    Activate();

                    _overlay.ShowMessage("CodeOverlay stopped.");
                });
            };

            _hotkeyService.IncludesOnlyPressed += async (_, _) =>
            {
                await ExecuteAsync(BuildMode.IncludesOnly);
            };

            _hotkeyService.IncludesAndMainPressed += async (_, _) =>
            {
                await ExecuteAsync(BuildMode.IncludesAndMain);
            };
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
                return;

            _hotkeyService.Register();
            _isRunning = true;

            _overlay.ShowMessage("CodeOverlay started.");

            Hide();
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = "https://github.com/ursuvladislav/Code_Overlay_Program",
                    UseShellExecute = true
                });
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _hotkeyService.Dispose();
            Application.Current.Shutdown();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _hotkeyService.Dispose();
            base.OnClosed(e);
        }

        private async Task ExecuteAsync(BuildMode mode)
        {
            try
            {
                string? code = ClipboardService.GetText();

                if (string.IsNullOrWhiteSpace(code))
                {
                    _overlay.ShowMessage("Буфер пуст.");
                    return;
                }

                _overlay.ShowMessage("Компилирую...");

                string result = await CompilerService.CompileAndRunCppAsync(code, mode);

                _overlay.ShowMessage(result);
            }
            catch (Exception ex)
            {
                _overlay.ShowMessage("Ошибка:\n" + ex.Message);
            }
        }
    }
}