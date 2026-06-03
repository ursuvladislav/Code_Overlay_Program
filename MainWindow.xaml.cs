using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace CodeOverlayRunner
{
    public partial class MainWindow : Window
    {
        private readonly OverlayWindow _overlay;

        private const int HOTKEY_ID_INCLUDES_ONLY = 1;
        private const int HOTKEY_ID_INCLUDES_AND_MAIN = 2;

        // Модификаторы
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;

        // Клавиши
        private const uint VK_SPACE = 0x20;   // Space
        private const uint VK_OEM_2 = 0xBF;   // / ? на стандартной раскладке

        public MainWindow()
        {
            InitializeComponent();
            _overlay = new OverlayWindow();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var helper = new WindowInteropHelper(this);
            var source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(WndProc);

            // Ctrl + Shift + /
            RegisterHotKey(helper.Handle, HOTKEY_ID_INCLUDES_ONLY, MOD_CONTROL | MOD_SHIFT, VK_OEM_2);

            // Ctrl + Shift + Space
            RegisterHotKey(helper.Handle, HOTKEY_ID_INCLUDES_AND_MAIN, MOD_CONTROL | MOD_SHIFT, VK_SPACE);

            // Главное окно не показываем
            Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            var helper = new WindowInteropHelper(this);

            UnregisterHotKey(helper.Handle, HOTKEY_ID_INCLUDES_ONLY);
            UnregisterHotKey(helper.Handle, HOTKEY_ID_INCLUDES_AND_MAIN);

            base.OnClosed(e);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY)
            {
                int hotkeyId = wParam.ToInt32();
                handled = true;

                switch (hotkeyId)
                {
                    case HOTKEY_ID_INCLUDES_ONLY:
                        _ = CompileFromClipboardAsync(BuildMode.IncludesOnly);
                        return IntPtr.Zero;

                    case HOTKEY_ID_INCLUDES_AND_MAIN:
                        _ = CompileFromClipboardAsync(BuildMode.IncludesAndMain);
                        return IntPtr.Zero;
                }
            }

            return IntPtr.Zero;
        }

        private async Task CompileFromClipboardAsync(BuildMode mode)
        {
            try
            {
                if (!Clipboard.ContainsText())
                {
                    _overlay.ShowMessage("Буфер пуст. Скопируй C++ код (Ctrl+C).");
                    return;
                }

                string code = Clipboard.GetText();

                if (string.IsNullOrWhiteSpace(code))
                {
                    _overlay.ShowMessage("Буфер пуст.");
                    return;
                }

                string modeText = mode switch
                {
                    BuildMode.IncludesOnly => "Компилирую C++...",
                    BuildMode.IncludesAndMain => "Компилирую C++...",
                    _ => "Компилирую C++..."
                };

                _overlay.ShowMessage(modeText);

                string result = await CompilerService.CompileAndRunCppAsync(code, mode);

                _overlay.ShowMessage(result);
            }
            catch (Exception ex)
            {
                _overlay.ShowMessage("Ошибка:\n" + ex.Message);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}