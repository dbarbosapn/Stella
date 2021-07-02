using Stella.API;
using Stella.Dialogs;
using Stella.Helper;
using Stella.Models;
using Stella.Pages;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Stella.Windows
{
    /// <summary>
    /// Interaction logic for CustomerDetails.xaml
    /// </summary>
    public partial class CustomerDetails : Window
    {
        private string _customerId;

        private bool[] loaded = new bool[3];

        public CustomerDetails(string customerId)
        {
            _customerId = customerId;
            InitializeComponent();
            Title = $"{Title} - {Locale.Locale.customer_details}";
            LoadingFrame.Content = new LoadingPage();

            TransactionDatePicker.Language = XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag);

            LoadCustomerInfo();
            LoadServicesAvailable();
            LoadHistory();
        }

        private void LoadHistory()
        {
            NotifyLoaded(2, false);
            Client.Instance.GetTransactions(_customerId, (records) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    NotifyLoaded(2, true);
                    TransactionList.ItemsSource = records;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(TransactionList.ItemsSource);
                    view.Filter = TransactionFilter;
                });
            });
        }

        private bool TransactionFilter(object item)
        {
            if (!TransactionDatePicker.SelectedDate.HasValue)
                return true;

            return (item as Transaction).Date.Value.Date == TransactionDatePicker.SelectedDate.Value.Date;
        }

        private void LoadServicesAvailable()
        {
            NotifyLoaded(1, false);
            Client.Instance.GetRecords(_customerId, (records) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    NotifyLoaded(1, true);
                    RecordList.ItemsSource = records;
                });
            });
        }

        private void LoadCustomerInfo()
        {
            NotifyLoaded(0, false);
            Client.Instance.GetCustomer(_customerId, (customer) =>
            {
                WindowHelper.Instance.RunOnUIThread(() =>
                {
                    NotifyLoaded(0, true);
                    if (customer != null)
                    {
                        NumberLabel.Text = customer.Number.ToString();
                        NameLabel.Text = customer.Name;
                        AddressLabel.Text = customer.Address;
                        PhoneLabel.Text = customer.Phone;
                        EmailLabel.Text = customer.Email;
                        GenderLabel.Text = customer.SerializedGender;
                        BirthdateLabel.Text = customer.SerializedBirthdate;
                        NotesLabel.Text = customer.Notes;
                    }
                    else
                    {
                        ShowSnackbar(Locale.Locale.error_loading_customer_info);
                    }
                });
            });
        }

        /// <summary>
        /// We only stop loading when all components have been loaded
        /// </summary>
        /// <param name="index">Index of the loaded component</param>
        private void NotifyLoaded(int index, bool loaded)
        {
            this.loaded[index] = loaded;

            bool setLoading = false;

            foreach (bool val in this.loaded)
            {
                if (!val)
                {
                    setLoading = true;
                    break;
                }
            }

            SetLoading(setLoading);
        }

        private void SetLoading(bool show)
        {
            LoadingFrame.Visibility = show ? Visibility.Visible : Visibility.Hidden;
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
        /// Shows a snackbar with given content
        /// </summary>
        /// <param name="content">Content to display</param>
        public void ShowSnackbar(string content)
        {
            Snackbar.MessageQueue.Enqueue(content);
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

        private void RecordList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RecordList.SelectedItem != null)
                RemoveService.IsEnabled = true;
            else
                RemoveService.IsEnabled = false;
        }

        private void RemoveService_Click(object sender, RoutedEventArgs e)
        {
            var w = new ConfirmationDialog();
            if(w.ShowDialog() == true)
            {
                SetLoading(true);
                var record = RecordList.SelectedItem as AccumulatedRecord;
                Client.Instance.RemoveRecord(record.Ids[0], (removed) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        SetLoading(false);

                        if (removed)
                            ShowSnackbar(Locale.Locale.service_removed);
                        else
                            ShowSnackbar(Locale.Locale.error_removing_service);

                        LoadServicesAvailable();
                    });
                });
            }
        }

        private void TransactionList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TransactionList.SelectedItem != null)
            {
                EditNotes.IsEnabled = true;
                DeleteRecord.IsEnabled = true;

                var t = TransactionList.SelectedItem as Transaction;
                ServiceNameLabel.Text = t.ServiceName;
                DateLabel.Text = t.SerializedDate;
                TNotesLabel.Text = t.Notes;
            }
            else
            {
                EditNotes.IsEnabled = false;
                DeleteRecord.IsEnabled = false;

                ServiceNameLabel.Text = "";
                DateLabel.Text = "";
                TNotesLabel.Text = "";
            }
        }

        private void TransactionDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TransactionDatePicker.SelectedDate != null)
                RemoveDate.Visibility = Visibility.Visible;
            else
                RemoveDate.Visibility = Visibility.Hidden;

            CollectionViewSource.GetDefaultView(TransactionList.ItemsSource).Refresh();
        }

        private void RemoveDate_Click(object sender, RoutedEventArgs e)
        {
            TransactionDatePicker.SelectedDate = null;
        }

        private void EditNotes_Click(object sender, RoutedEventArgs e)
        {
            var t = TransactionList.SelectedItem as Transaction;

            var w = new EditNotesDialog(t.Notes);
            if(w.ShowDialog() == true)
            {
                SetLoading(true);

                var newNotes = w.Result;
                Client.Instance.EditTransactionNotes(newNotes, t.Id, (edited) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        SetLoading(false);

                        if (edited)
                            ShowSnackbar(Locale.Locale.record_edited);
                        else
                            ShowSnackbar(Locale.Locale.error_editing_record);

                        LoadHistory();
                    });
                });
            }
        }

        private void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            var w = new ConfirmationDialog();
            if (w.ShowDialog() == true)
            {
                SetLoading(true);
                var transaction = TransactionList.SelectedItem as Transaction;
                Client.Instance.RemoveTransaction(transaction, (removed) =>
                {
                    WindowHelper.Instance.RunOnUIThread(() =>
                    {
                        SetLoading(false);

                        if (removed)
                            ShowSnackbar(Locale.Locale.record_removed);
                        else
                            ShowSnackbar(Locale.Locale.error_removing_record);

                        LoadHistory();
                    });
                });
            }
        }
    }
}
