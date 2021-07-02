using Stella.Models;
using System.Windows;

namespace Stella.Dialogs
{
    /// <summary>
    /// Interaction logic for AddServiceDialog.xaml
    /// </summary>
    public partial class AddServiceDialog : Window
    {
        public Service Result = null;

        public AddServiceDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets data. Used for editing
        /// </summary>
        public void SetEdit(Service s)
        {
            Title.Text = Locale.Locale.edit_service;
            NameBox.Text = s.Name;
            NotesBox.Text = s.Notes;
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Updates the result field and closes the window with dialog result
        /// </summary>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Result = new Service("", NameBox.Text, NotesBox.Text);
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Enables or disables the confirmation button based on input
        /// </summary>
        private void Confirm_Validation(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            bool enabled = true;

            if (string.IsNullOrEmpty(NameBox.Text))
                enabled = false;

            ConfirmButton.IsEnabled = enabled;
        }
    }
}
