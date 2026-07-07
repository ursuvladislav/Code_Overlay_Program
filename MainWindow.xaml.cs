using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using CodeOverlayRunner.Services;
using CodeOverlayRunner.Views;
using System.ComponentModel;

namespace CodeOverlayRunner
{
    public partial class MainWindow : Window
    {
        private readonly HotkeyService _hotkeyService;
        private readonly OverlayWindow _overlay;

        private bool _isRunning = false;
        private bool _isExiting = false;
        private const string GithubUrl =
            "https://github.com/ursuvladislav/Code_Overlay_Program";

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

            ShowHomeView();
        }

        private void ShowHomeView()
        {
            var view = new HomeView();

            view.StartClicked += (_, _) => StartOverlayMode();
            view.SettingsClicked += (_, _) => ShowSettingsView();
            view.GitHubClicked += (_, _) => ShowGitHubView();
            view.ExitClicked += (_, _) => ExitApplication();

            MainContent.Content = view;
        }

        private void ShowSettingsView()
        {
            var view = new SettingsView();

            view.SaveClicked += (_, _) =>
            {
                string interfaceLanguage = view.GetSelectedInterfaceLanguage();
                string programmingLanguage = view.GetSelectedProgrammingLanguage();
                string timeout = view.GetSelectedTimeout();

                _overlay.ShowMessage(
                    $"Settings saved:\nLanguage: {interfaceLanguage}\nProgramming: {programmingLanguage}\nTimeout: {timeout}");
            };

            view.BackClicked += (_, _) => ShowHomeView();

            MainContent.Content = view;
        }

        private void ShowGitHubView()
        {
            var view = new GitHubView();

            view.OpenGitHubClicked += (_, _) => OpenGitHub();
            view.BackClicked += (_, _) => ShowHomeView();

            MainContent.Content = view;
        }

        private void StartOverlayMode()
        {
            if (_isRunning)
                return;

            _hotkeyService.Register();
            _isRunning = true;

            _overlay.ShowMessage("CodeOverlay started.");

            Hide();
        }

        private void OpenGitHub()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = GithubUrl,
                UseShellExecute = true
            });
        }

        private void ExitApplication()
        {
            if (_isExiting)
                return;

            _isExiting = true;

            try
            {
                _hotkeyService.Dispose();
            }
            catch
            {
                // Игнорируем ошибки при закрытии
            }

            try
            {
                _overlay.Close();
            }
            catch
            {
                // Игнорируем ошибки при закрытии overlay
            }
            
            Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                ExitApplication();
                return;
            }

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