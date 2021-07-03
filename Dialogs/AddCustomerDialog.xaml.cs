using Stella.Models;
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
        }

        /// <summary>
        /// Close the window
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Set result and close
        /// </summary>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            var date = BirthdatePicker.SelectedDate;

            int? number = null;
            int tmp;
            if (int.TryParse(NumberBox.Text, out tmp))
                number = tmp;

            Result = new Customer("", NameBox.Text, number, AddressBox.Text, PhoneBox.Text, EmailBox.Text, ((ComboBoxItem)GenderSelector.SelectedItem).Tag.ToString(), date, NotesBox.Text);

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

            if (NumberBox.Text != "" && !Regex.IsMatch(PhoneBox.Text, "^[+]?[0-9]*?$"))

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
            NumberBox.Text = c.Number.ToString();
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
