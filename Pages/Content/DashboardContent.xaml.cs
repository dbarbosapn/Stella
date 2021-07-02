using Stella.API;
using Stella.Dialogs;
using Stella.Helper;
using Stella.Windows;
using System;
using System.Windows.Controls;

namespace Stella.Pages.Content
{
    /// <summary>
    /// Interaction logic for DashboardContent.xaml
    /// </summary>
    public partial class DashboardContent : Page
    {
        public DashboardContent()
        {
            InitializeComponent();

            LoadStats();
        }

        private void LoadStats()
        {
            WindowHelper.Instance.SetLoading(true, true);
            Client.Instance.GetStats((stats) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    WindowHelper.Instance.SetLoading(false, true);
                    if(stats != null)
                    {
                        NumServices.Text = stats.NumServices;
                        NumCustomers.Text = stats.NumCustomers;
                        NumTransactions.Text = stats.NumTransactions;
                    }
                    else
                    {
                        WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_stats);
                    }
                });
            });
        }

        /// <summary>
        /// Adds service records to the customer
        /// </summary>
        private void LoadServices_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var cw = new SelectCustomerDialog()
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (cw.ShowDialog() == true)
            {
                string cid = cw.SelectedId;

                var sw = new LoadServicesDialog()
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };

                if (sw.ShowDialog() == true)
                {
                    string sname = sw.SelectedName;
                    int qt = sw.Quantity;

                    WindowHelper.Instance.SetLoading(true);
                    Client.Instance.AddRecords(cid, sname, qt, (added) =>
                    {
                        WindowHelper.Instance.RunOnUIThread(() =>
                        {
                            WindowHelper.Instance.SetLoading(false);
                            if (added)
                                WindowHelper.Instance.ShowSnackbar(Locale.Locale.services_loaded);
                            else
                                WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_loading_services);
                        });
                    });
                }
            }
        }

        private void ChargeService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var cw = new SelectCustomerDialog()
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (cw.ShowDialog() == true)
            {
                string cid = cw.SelectedId;

                var w = new ChargeServiceDialog(cid)
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };

                if(w.ShowDialog() == true)
                {
                    WindowHelper.Instance.SetLoading(true);
                    if (w.IsRecord)
                    {
                        Client.Instance.RemoveRecord(w.SelectedId, (removed) =>
                        {
                            if (removed)
                                FinishTransaction(cid, w.SelectedServiceName, w.SelectedDateTime, w.Notes);
                            else
                                WindowHelper.Instance.RunOnUIThread(() =>
                                {
                                    WindowHelper.Instance.SetLoading(false);
                                    WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_charging_service);
                                });
                        });
                    }
                    else FinishTransaction(cid, w.SelectedServiceName, w.SelectedDateTime, w.Notes);
                }
            }
        }

        /// <summary>
        /// Adds transaction to the database and shows snackbar
        /// </summary>
        private void FinishTransaction(string customerId, string serviceName, DateTime date, string notes)
        {
            Client.Instance.AddTransaction(customerId, serviceName, date, notes, (added) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    WindowHelper.Instance.SetLoading(false);
                    if(added)
                    {
                        WindowHelper.Instance.ShowSnackbar(Locale.Locale.service_charged);
                        LoadStats();
                    }
                    else
                        WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_charging_service);
                });
            });
        }

        /// <summary>
        /// Show customer details window
        /// </summary>
        private void CustomerDetails_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var cw = new SelectCustomerDialog()
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (cw.ShowDialog() == true)
            {
                string cid = cw.SelectedId;

                var w = new CustomerDetails(cid);
                w.Show();
            }
        }
    }
}
