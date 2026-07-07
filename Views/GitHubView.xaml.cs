using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeOverlayRunner.Views
{
    public partial class GitHubView  : UserControl
    {
        public event EventHandler? OpenGitHubClicked;
        public event EventHandler? BackClicked;

        public GitHubView()
        {
            InitializeComponent();
        }

        private void OpenGitHubButton_Click(object sender, RoutedEventArgs e)
        {
            OpenGitHubClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}