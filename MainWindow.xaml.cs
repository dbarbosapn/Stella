using Stella.API;
using Stella.Helper;
using Stella.Pages;
using System;
using System.Windows;

namespace Stella
{
    /// <summary>
    /// The MainWindow is where the landing dashboard and main functionality resides
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LocaleHelper.Instance.Load();

            WindowHelper.Instance.LoadingFrame = LoadingFrame;
            WindowHelper.Instance.MainFrame = MainFrame;
            WindowHelper.Instance.BottomSnackbar = Snackbar;

            LoadingFrame.Content = new LoadingPage();

            WindowHelper.Instance.SetLoading(true);
            Client.Instance.ValidateToken((result) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    if (result != null)
                    {
                        MainFrame.Content = new MenuPage(result);
                    }
                    else
                    {
                        MainFrame.Content = new AuthenticationPage();
                    }
                    WindowHelper.Instance.SetLoading(false);
                });
            });

            LoadReader();
        }

        private void LoadReader()
        {
            RFIDHelper.Instance.ScanReader(() =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    WindowHelper.Instance.ShowSnackbar(Locale.Locale.reader_found);
                });
            },
            () =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    WindowHelper.Instance.ShowSnackbar(Locale.Locale.reader_not_found);
                });
            });
        }

        /// <summary>
        /// Close the window
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Minimize the window
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Add drag/drop functionality to the window
        /// </summary>
        private void TopRow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
