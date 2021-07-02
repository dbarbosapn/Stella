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
    /// Interaction logic for ServicesContent.xaml
    /// </summary>
    public partial class ServicesContent : Page
    {
        public ServicesContent()
        {
            InitializeComponent();
            LoadServices();
        }

        /// <summary>
        /// Loads the services into the listview
        /// </summary>
        private void LoadServices()
        {
            WindowHelper.Instance.SetLoading(true, true);
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

        /// <summary>
        /// Opens Add Service dialog and, if confirmed, adds the service using the client
        /// </summary>
        private void AddService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var w = new AddServiceDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (w.ShowDialog() == true)
            {
                var r = w.Result;
                WindowHelper.Instance.SetLoading(true);
                Client.Instance.AddService(r.Name, r.Notes, (added) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        WindowHelper.Instance.SetLoading(false);
                        if (added)
                        {
                            LoadServices();
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.service_added);
                        }
                        else
                        {
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_adding_service);
                        }
                    });
                });
            }
        }

        /// <summary>
        /// Listen to ServiceList selection changes
        /// </summary>
        private void ServiceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ServiceList.SelectedItem != null)
            {
                RemoveService.IsEnabled = true;
                EditService.IsEnabled = true;
            }
            else
            {
                RemoveService.IsEnabled = false;
                EditService.IsEnabled = false;
            }
        }

        /// <summary>
        /// Remove service routine
        /// </summary>
        private void RemoveService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var w = new ConfirmationDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            
            var s = ServiceList.SelectedItem as Service;

            if (w.ShowDialog() == true)
            {
                WindowHelper.Instance.SetLoading(true);
                Client.Instance.RemoveService(s, (removed) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        WindowHelper.Instance.SetLoading(false);
                        if (removed)
                        {
                            LoadServices();
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.service_removed);
                        }
                        else
                        {
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_removing_service);
                        }
                    });
                });
            }
        }

        /// <summary>
        /// Opens dialog for editing
        /// </summary>
        private void EditService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var w = new AddServiceDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            var s = ServiceList.SelectedItem as Service;
            w.SetEdit(s);

            if (w.ShowDialog() == true)
            {
                var r = w.Result;
                WindowHelper.Instance.SetLoading(true);
                Client.Instance.EditService(s.Id, r.Name, r.Notes, (added) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        WindowHelper.Instance.SetLoading(false);
                        if (added)
                        {
                            LoadServices();
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.service_edited);
                        }
                        else
                        {
                            WindowHelper.Instance.ShowSnackbar(Locale.Locale.error_editing_service);
                        }
                    });
                });
            }
        }

        /// <summary>
        /// Refresh the filters when the search box has changed
        /// </summary>
        private void ServiceSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ServiceList.ItemsSource).Refresh();
        }

        private bool ServiceFilter(object item)
        {
            if (string.IsNullOrEmpty(ServiceSearch.Text))
                return true;

            return (item as Service).Name.IndexOf(ServiceSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
