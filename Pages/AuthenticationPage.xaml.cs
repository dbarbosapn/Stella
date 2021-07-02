using Stella.API;
using Stella.Helper;
using System.Windows.Controls;

namespace Stella.Pages
{
    /// <summary>
    /// Interaction logic for Authentication.xaml
    /// </summary>
    public partial class AuthenticationPage : Page
    {
        public AuthenticationPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Validate the new token and procceed to the next page
        /// </summary>
        private void ConfirmButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(TokenInput.Text != "")
            {
                WindowHelper.Instance.SetLoading(true);
                Client.Instance.SetToken(TokenInput.Text);
                Client.Instance.ValidateToken((result) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        if (result != null)
                        {
                            WindowHelper.Instance.SetContent(new MenuPage(result));
                        }
                        else
                        {
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.token_error);
                        }
                        WindowHelper.Instance.SetLoading(false);
                    });
                });
            }
        }

        /// <summary>
        /// Loads the language selector with the current locale
        /// </summary>
        private void LanguageSelector_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            string l = LocaleHelper.Instance.GetCurrentLocale();
            switch(l)
            {
                case "pt":
                    LanguageSelector.SelectedIndex = 1;
                    break;
                default:
                    LanguageSelector.SelectedIndex = 0;
                    break;
            }
        }

        /// <summary>
        /// Listens to locale changes
        /// </summary>
        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string value = ((ComboBoxItem)LanguageSelector.SelectedItem).Tag.ToString();
            if(LocaleHelper.Instance.SetLocale(value))
            {
                WindowHelper.Instance.SetContent(new AuthenticationPage());
            }
        }

        /// <summary>
        /// Enables or disables the confirmation button based on input
        /// </summary>
        private void TokenInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TokenInput.Text))
                ConfirmButton.IsEnabled = false;
            else
                ConfirmButton.IsEnabled = true;
        }
    }
}
