using Stella.API;
using Stella.Helper;
using Stella.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Stella.Dialogs
{
    /// <summary>
    /// Interaction logic for GetCustomerDialog.xaml
    /// </summary>
    public partial class SelectCustomerDialog : Window
    {
        public string SelectedId = null;

        public SelectCustomerDialog()
        {
            InitializeComponent();
            LoadCustomers();

            RFIDHelper.Instance.OnReaderDataReceived += OnReaderValue;
        }

        /// <summary>
        /// Loads the customers into the listview
        /// </summary>
        private void LoadCustomers()
        {
            Client.Instance.GetCustomers((customers) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    CustomerList.ItemsSource = customers;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(CustomerList.ItemsSource);
                    view.Filter = CustomerFilter;
                });
            });
        }

        private bool CustomerFilter(object item)
        {
            if (string.IsNullOrEmpty(CustomerSearch.Text))
                return true;

            return (item as Customer).Name.IndexOf(CustomerSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void CustomerList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CustomerList.SelectedItem != null)
                SelectButton.IsEnabled = true;
            else
                SelectButton.IsEnabled = false;
        }

        private void CustomerSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(CustomerList.ItemsSource).Refresh();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedId = (CustomerList.SelectedItem as Customer).Id;
            DialogResult = true;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            RFIDHelper.Instance.OnReaderDataReceived -= OnReaderValue;
        }

        private void OnReaderValue(string value)
        {
            Client.Instance.GetCustomerCard(value, (customer) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    if (customer != null)
                    {
                        RFIDHelper.Instance.PositiveSignal();
                        SelectedId = customer.Id;
                        DialogResult = true;
                        Close();
                    }
                    else RFIDHelper.Instance.NegativeSignal();
                });
            });
        }
    }
}
