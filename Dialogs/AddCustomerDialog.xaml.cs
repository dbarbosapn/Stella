using Stella.Helper;
using Stella.Models;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Stella.Dialogs
{
    /// <summary>
    /// Interaction logic for AddCustomerDialog.xaml
    /// </summary>
    public partial class AddCustomerDialog : Window
    {
        public Customer Result;

        public AddCustomerDialog()
        {
            InitializeComponent();

            BirthdatePicker.Language = XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag);

            RFIDHelper.Instance.OnReaderDataReceived += OnReaderValue;
        }

        private void OnReaderValue(string value)
        {
            WindowHelper.Instance.RunOnUIThread(() =>
            {
                if (value != "")
                {
                    RFIDHelper.Instance.PositiveSignal();
                    CardLabel.Text = value;
                }
                else RFIDHelper.Instance.NegativeSignal();
            });
        }

        /// <summary>
        /// Close the window
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            RFIDHelper.Instance.OnReaderDataReceived -= OnReaderValue;
        }

        /// <summary>
        /// Set result and close
        /// </summary>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            var date = BirthdatePicker.SelectedDate;
            var pin = string.IsNullOrEmpty(PinBox.Text) ? null : PinBox.Text.Trim();

            Result = new Customer("", CardLabel.Text, NameBox.Text, null, pin, AddressBox.Text, PhoneBox.Text, EmailBox.Text, ((ComboBoxItem)GenderSelector.SelectedItem).Tag.ToString(), date, NotesBox.Text);

            Close();
        }

        /// <summary>
        /// Enables or disables the confirmation button based on input
        /// </summary>
        private void Confirm_Validation(object sender, TextChangedEventArgs e)
        {
            ConfirmButton.IsEnabled = ShouldEnableConfirm();
        }

        private void GenderSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfirmButton.IsEnabled = ShouldEnableConfirm();
        }

        private bool ShouldEnableConfirm()
        {
            bool enabled = true;

            if (PinBox.Text != "" && !Regex.IsMatch(PinBox.Text, "^[0-9]+$"))
                enabled = false;

            if (PhoneBox.Text != "" && !Regex.IsMatch(PhoneBox.Text, "^[0-9]+$"))
                enabled = false;

            if (string.IsNullOrEmpty(NameBox.Text))
                enabled = false;

            if (GenderSelector.SelectedItem == null)
                enabled = false;

            return enabled;
        }

        /// <summary>
        /// Sets fields for editing
        /// </summary>
        /// <param name="c">Customer to edit</param>
        public void SetEdit(Customer c)
        {
            NameBox.Text = c.Name;
            CardLabel.Text = c.Card != null ? $" {c.Card}" : "";
            NumberLabel.Text = c.Number != null ? $" {c.Number}" : "";
            PinBox.Text = c.Pin;
            AddressBox.Text = c.Address;
            PhoneBox.Text = c.Phone;
            EmailBox.Text = c.Email;
            GenderSelector.SelectedIndex = GetGenderIndex(c.Gender);
            BirthdatePicker.SelectedDate = c.Birthdate;
            NotesBox.Text = c.Notes;
        }

        /// <summary>
        /// Get gender index for the selector
        /// </summary>
        private int GetGenderIndex(string gender)
        {
            switch(gender)
            {
                case "male":
                    return 0;
                case "female":
                    return 1;
                default:
                    return 2;
            }
        }
    }
}
