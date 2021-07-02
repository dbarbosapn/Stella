using Stella.API;
using Stella.Helper;
using Stella.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Stella.Dialogs
{
    /// <summary>
    /// Interaction logic for ChargeServiceDialog.xaml
    /// </summary>
    public partial class ChargeServiceDialog : Window
    {
        public string SelectedServiceName = null;
        public bool IsRecord = true;
        public string SelectedId = null;
        public string Notes = null;
        public DateTime SelectedDateTime;

        private string _customerId;

        public ChargeServiceDialog(string customerId)
        {
            _customerId = customerId;

            InitializeComponent();
            LoadRecords();

            LoadPickers();
        }

        /// <summary>
        /// Loads the date and time pickers with default values
        /// </summary>
        private void LoadPickers()
        {
            SelectedDateTime = DateTime.Now;

            ChargeDatePicker.SelectedDate = SelectedDateTime;
            ChargeTimePicker.SelectedTime = SelectedDateTime;

            ChargeDatePicker.Language = XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag);
            ChargeTimePicker.Language = XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        /// <summary>
        /// Loads the services into the listview
        /// </summary>
        private void LoadRecords()
        {
            Client.Instance.GetRecords(_customerId, (services) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    WindowHelper.Instance.SetLoading(false, true);
                    RecordList.ItemsSource = services;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(RecordList.ItemsSource);
                    view.Filter = RecordFilter;

                    QuantityColumn.Width = double.NaN;
                });
            });
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

                    List<AccumulatedRecord> list = new List<AccumulatedRecord>();

                    foreach (var service in services)
                    {
                        var r = new AccumulatedRecord(_customerId, service.Name);
                        r.Ids.Add(service.Id);
                        list.Add(r);
                    }

                    RecordList.ItemsSource = list;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(RecordList.ItemsSource);
                    view.Filter = RecordFilter;

                    QuantityColumn.Width = 0;
                });
            });
        }

        private bool RecordFilter(object item)
        {
            if (string.IsNullOrEmpty(ServiceSearch.Text))
                return true;

            return (item as AccumulatedRecord).ServiceName.IndexOf(ServiceSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RecordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordList.SelectedItem != null)
                ConfirmButton.IsEnabled = true;
            else
                ConfirmButton.IsEnabled = false;
        }

        private void ServiceSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(RecordList.ItemsSource).Refresh();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            AccumulatedRecord r = RecordList.SelectedItem as AccumulatedRecord;
            SelectedServiceName = r.ServiceName;
            SelectedId = r.Ids[0];

            var date = ChargeDatePicker.SelectedDate;
            var time = ChargeTimePicker.SelectedTime;
            SelectedDateTime = DateTime.Parse($"{date.Value.Day}/{date.Value.Month}/{date.Value.Year} {time.Value.Hour}:{time.Value.Minute}:{time.Value.Second}");

            Notes = NotesBox.Text;

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Enable single charge mode (loads services)
        /// </summary>
        private void SingleCharge_Checked(object sender, RoutedEventArgs e)
        {
            if(SingleCharge.IsChecked.Value)
            {
                IsRecord = false;
                LoadServices();
            }
            else
            {
                IsRecord = true;
                LoadRecords();
            }
        }
    }
}
