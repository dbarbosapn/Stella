using Stella.API;
using Stella.Helper;
using Stella.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Stella.Dialogs
{
    /// <summary>
    /// Interaction logic for LoadServicesDialog.xaml
    /// </summary>
    public partial class LoadServicesDialog : Window
    {
        public string SelectedName = null;
        public int Quantity = 1;

        public LoadServicesDialog()
        {
            InitializeComponent();
            LoadServices();
        }

        /// <summary>
        /// Loads the services into the listview
        /// </summary>
        private void LoadServices()
        {
            Client.Instance.GetServices((services) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    WindowHelper.Instance.SetLoading(false, true);
                    ServiceList.ItemsSource = services;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ServiceList.ItemsSource);
                    view.Filter = ServiceFilter;
                });
            });
        }

        private bool ServiceFilter(object item)
        {
            if (string.IsNullOrEmpty(ServiceSearch.Text))
                return true;

            return (item as Service).Name.IndexOf(ServiceSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void ServiceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceList.SelectedItem != null)
                ConfirmButton.IsEnabled = true;
            else
                ConfirmButton.IsEnabled = false;
        }

        private void ServiceSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ServiceList.ItemsSource).Refresh();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedName = (ServiceList.SelectedItem as Service).Name;

            DialogResult = true;
            Close();
        }

        private void QtLeft_Click(object sender, RoutedEventArgs e)
        {
            if (Quantity > 1)
                Quantity--;

            QuantityLabel.Text = Quantity.ToString();
        }

        private void QtRight_Click(object sender, RoutedEventArgs e)
        {
            Quantity++;

            QuantityLabel.Text = Quantity.ToString();
        }
    }
}
