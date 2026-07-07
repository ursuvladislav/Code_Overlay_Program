using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeOverlayRunner.Views
{
    public partial class SettingsView : UserControl
    {
        public event EventHandler? SaveClicked;
        public event EventHandler? BackClicked;

        public SettingsView ()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackClicked?.Invoke(this, EventArgs.Empty);
        }

        public string GetSelectedInterfaceLanguage()
        {
            return ((ComboBoxItem)InterfaceLanguageComboBox.SelectedItem).Content.ToString() ?? "English";
        }

        public string GetSelectedProgrammingLanguage()
        {
            return ((ComboBoxItem)ProgrammingLanguageComboBox.SelectedItem).Content.ToString() ?? "Auto";
        }

        public string GetSelectedTimeout()
        {
            return ((ComboBoxItem)TimeoutComboBox.SelectedItem).Content.ToString() ?? "2000 ms";
        }
    }
}