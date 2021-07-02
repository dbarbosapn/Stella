using Stella.Helper;
using Stella.Pages.Content;
using System.Windows.Controls;
using System.Windows.Media;

namespace Stella.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private string _name = "";
        private Button selectedButton;

        public MenuPage(string name)
        {
            _name = name;
            InitializeComponent();

            WindowHelper.Instance.LoadingContentFrame = LoadingContentFrame;
            LoadingContentFrame.Content = new LoadingPage();
            WindowHelper.Instance.SetLoading(false, true);

            ContentFrame.Content = new DashboardContent();
        }

        /// <summary>
        /// Load basic functionality
        /// </summary>
        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            HelloText.Text = $"{Locale.Locale.hello}, {_name}";
            selectedButton = DashboardButton;
        }

        /// <summary>
        /// Loads the language selector with the current locale
        /// </summary>
        private void LanguageSelector_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            string l = LocaleHelper.Instance.GetCurrentLocale();
            switch (l)
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
            if (LocaleHelper.Instance.SetLocale(value))
            {
                WindowHelper.Instance.SetContent(new MenuPage(_name));
            }
        }

        /// <summary>
        /// Menu button click listener
        /// </summary>
        private void Menu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(sender != selectedButton)
            {
                selectedButton.Foreground = new SolidColorBrush(Colors.Black);
                selectedButton = (Button)sender;
                selectedButton.Foreground = new SolidColorBrush(Colors.LightPink);
            }

            switch(selectedButton.Tag)
            {
                case "Services":
                    ContentFrame.Content = new ServicesContent();
                    break;

                case "Customers":
                    ContentFrame.Content = new CustomersContent();
                    break;

                case "Settings":
                    ContentFrame.Content = new SettingsContent();
                    break;

                case "Dashboard":
                    ContentFrame.Content = new DashboardContent();
                    break;
            }
        }
    }
}
