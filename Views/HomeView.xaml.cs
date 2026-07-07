using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeOverlayRunner.Views
{
    public partial class HomeView : UserControl
    {
        public event EventHandler? StartClicked;
        public event EventHandler? SettingsClicked;
        public event EventHandler? GitHubClicked;
        public event EventHandler? ExitClicked;
        
        public HomeView()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartClicked?.Invoke(this, EventArgs.Empty);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            GitHubClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}