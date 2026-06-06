using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace CodeOverlayRunner.Services
{
    public sealed class HotkeyService : IDisposable
    {
        private readonly Window _window;

        private HwndSource? _source;

        private const int WM_HOTKEY = 0x0312;

        private const int HOTKEY_ID_INCLUDES_ONLY = 1;
        private const int HOTKEY_ID_INCLUDES_AND_MAIN = 2;
        private const int HOTKEY_ID_OPEN_MAIN_WINDOW = 3;
        
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;

        private const uint VK_SPACE = 0x20;
        private const uint VK_OEM_2 = 0xBF;
        private const uint VK_O = 0x4F;

        public event EventHandler? IncludesOnlyPressed;
        public event EventHandler? IncludesAndMainPressed;
        public event EventHandler? OpenMainWindowPressed;

        public HotkeyService(Window window)
        {
            _window = window;
        }

        public void Register()
        {
            var helper = new WindowInteropHelper(_window);

            _source = HwndSource.FromHwnd(helper.Handle);

            _source?.AddHook(WndProc);

            RegisterHotKey(
                helper.Handle,
                HOTKEY_ID_OPEN_MAIN_WINDOW,
                MOD_CONTROL | MOD_SHIFT | MOD_ALT,
                VK_O);
                
            RegisterHotKey(
                helper.Handle,
                HOTKEY_ID_INCLUDES_ONLY,
                MOD_CONTROL | MOD_SHIFT | MOD_ALT,
                VK_OEM_2);

            RegisterHotKey(
                helper.Handle,
                HOTKEY_ID_INCLUDES_AND_MAIN,
                MOD_CONTROL | MOD_SHIFT,
                VK_SPACE);
        }

        public void Unregister()
        {
            var helper = new WindowInteropHelper(_window);

            UnregisterHotKey(helper.Handle, HOTKEY_ID_OPEN_MAIN_WINDOW);
            UnregisterHotKey(helper.Handle, HOTKEY_ID_INCLUDES_ONLY);
            UnregisterHotKey(helper.Handle, HOTKEY_ID_INCLUDES_AND_MAIN);

            _source?.RemoveHook(WndProc);
        }

        private IntPtr WndProc(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            if (msg != WM_HOTKEY)
                return IntPtr.Zero;

            handled = true;

            int id = wParam.ToInt32();

            switch (id)
            {
                case HOTKEY_ID_INCLUDES_ONLY:
                    IncludesOnlyPressed?.Invoke(this, EventArgs.Empty);
                    break;

                case HOTKEY_ID_INCLUDES_AND_MAIN:
                    IncludesAndMainPressed?.Invoke(this, EventArgs.Empty);
                    break;
                
                case HOTKEY_ID_OPEN_MAIN_WINDOW:
                    OpenMainWindowPressed?.Invoke(this, EventArgs.Empty);
                    break;
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            Unregister();
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            uint fsModifiers,
            uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(
            IntPtr hWnd,
            int id);
    }
}