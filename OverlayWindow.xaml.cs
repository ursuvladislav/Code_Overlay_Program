using System;
using System.Windows;
using System.Windows.Threading;

namespace CodeOverlayRunner
{
    public partial class OverlayWindow : Window
    {
        private readonly DispatcherTimer _timer;

        public OverlayWindow()
        {
            InitializeComponent();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            _timer.Tick += (_, __) =>
            {
                _timer.Stop();
                Hide();
            };
        }

        public void ShowMessage(string message)
        {
            TextOut.Text = message;

            // Позиция: правый нижний угол
            var area = SystemParameters.WorkArea;
            Left = area.Right - Width - 16;
            Top  = area.Bottom - Height - 16;

            Show();
            Activate();

            _timer.Stop();
            _timer.Start();
        }
    }
}