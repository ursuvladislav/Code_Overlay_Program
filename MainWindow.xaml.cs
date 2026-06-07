using System;
using System.Threading.Tasks;
using System.Windows;
using CodeOverlayRunner.Services;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Controls;
using CodeOverlayRunner.Views;

namespace CodeOverlayRunner
{
    public partial class MainWindow : Window
    {
        private readonly HotkeyService _hotkeyService;
        private readonly OverlayWindow _overlay;

        private bool _isRunning = false;
        private bool _isExiting = false;

        public MainWindow()
        {
            InitializeComponent();

            RightContent.Content = new CodeRainView();

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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            RightContent.Content = new TextBlock
            {
                Text = "Settings page\n\nLanguage: Coming soon\nHotkeys: Coming soon\nCompiler: Coming soon",
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            RightContent.Content = new TextBlock
            {
                Text = "GitHub repository will open in browser.",
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            Process.Start(
                new ProcessStartInfo
                {
                    FileName = "https://github.com/ursuvladislav/Code_Overlay_Program",
                    UseShellExecute = true
                });
        }

        private void ExitApplication()
        {
            if (_isExiting)
                return;

            _isExiting = true;

            _hotkeyService.Dispose();
            Application.Current.Shutdown();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _hotkeyService.Dispose();
            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ExitApplication();
            base.OnClosing(e);
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