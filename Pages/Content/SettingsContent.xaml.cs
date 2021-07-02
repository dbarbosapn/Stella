using Stella.API;
using Stella.Dialogs;
using Stella.Helper;
using System.Windows;
using System.Windows.Controls;

namespace Stella.Pages.Content
{
    /// <summary>
    /// Interaction logic for SettingsContent.xaml
    /// </summary>
    public partial class SettingsContent : Page
    {
        public SettingsContent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Logout after confirmation
        /// </summary>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new ConfirmationDialog()
            {
                Owner = Application.Current.MainWindow
            };
            if(w.ShowDialog() == true)
            {
                Client.Instance.SetToken("");
                WindowHelper.Instance.SetContent(new AuthenticationPage());
            }
        }
    }
}
