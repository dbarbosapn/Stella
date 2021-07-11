using Stella.API;
using Stella.Dialogs;
using Stella.Helper;
using Stella.Models;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Stella.Pages.Content
{
    /// <summary>
    /// Interaction logic for CustomersContent.xaml
    /// </summary>
    public partial class CustomersContent : Page
    {
        public CustomersContent()
        {
            InitializeComponent();
            LoadCustomers();
        }

        /// <summary>
        /// Loads the customers into the listview
        /// </summary>
        private void LoadCustomers()
        {
            WindowHelper.Instance.SetLoading(true, true);
            Client.Instance.GetCustomers((customers) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    WindowHelper.Instance.SetLoading(false, true);
                    CustomerList.ItemsSource = customers;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(CustomerList.ItemsSource);
                    view.Filter = CustomerFilter;
                });
            });
        }

        private void AddCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var w = new AddCustomerDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (w.ShowDialog() == true)
            {
                var r = w.Result;
                WindowHelper.Instance.SetLoading(true);
                Client.Instance.AddCustomer(r.Name, r.Card, r.Number, r.Address, r.Phone, r.Email, r.Gender, r.SerializedBirthdate, r.Notes, (added) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        WindowHelper.Instance.SetLoading(false);
                        if (added)
                        {
                            LoadCustomers();
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.customer_added);
                        }
                        else
                        {
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_adding_customer);
                        }
                    });
                });
            }
        }

        /// <summary>
        /// Listen to CustomerList selection changes
        /// </summary>
        private void CustomerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerList.SelectedItem != null)
            {
                RemoveCustomer.IsEnabled = true;
                EditCustomer.IsEnabled = true;
            }
            else
            {
                RemoveCustomer.IsEnabled = false;
                EditCustomer.IsEnabled = false;
            }
        }

        /// <summary>
        /// Edit customer routine
        /// </summary>
        private void EditCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var w = new AddCustomerDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            var c = CustomerList.SelectedItem as Customer;
            w.SetEdit(c);

            if (w.ShowDialog() == true)
            {
                var r = w.Result;
                WindowHelper.Instance.SetLoading(true);
                Client.Instance.EditCustomer(c.Id, r.Name, r.Number, r.Address, r.Phone, r.Email, r.Gender, r.SerializedBirthdate, r.Notes, (added) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        WindowHelper.Instance.SetLoading(false);
                        if (added)
                        {
                            LoadCustomers();
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.customer_edited);
                        }
                        else
                        {
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_editing_customer);
                        }
                    });
                });
            }
        }


        /// <summary>
        /// Remove customer routine
        /// </summary>
        private void RemoveCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var w = new ConfirmationDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            var s = CustomerList.SelectedItem as Customer;

            if (w.ShowDialog() == true)
            {
                WindowHelper.Instance.SetLoading(true);
                Client.Instance.RemoveCustomer(s, (removed) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        WindowHelper.Instance.SetLoading(false);
                        if (removed)
                        {
                            LoadCustomers();
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.customer_removed);
                        }
                        else
                        {
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_removing_customer);
                        }
                    });
                });
            }
        }

        private void CustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(CustomerList.ItemsSource).Refresh();
        }

        private bool CustomerFilter(object item)
        {
            if (string.IsNullOrEmpty(CustomerSearch.Text))
                return true;

            return (item as Customer).Name.IndexOf(CustomerSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
