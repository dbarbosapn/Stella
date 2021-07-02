using System.Windows;

namespace Stella.Dialogs
{
    /// <summary>
    /// Interaction logic for EditNotesDialog.xaml
    /// </summary>
    public partial class EditNotesDialog : Window
    {
        public string Result = "";

        public EditNotesDialog(string original)
        {
            InitializeComponent();
            NotesBox.Text = original;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Result = NotesBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
