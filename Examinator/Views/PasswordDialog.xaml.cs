using System.Windows;

namespace Examinator.Views
{
    /// <summary>
    /// Логика взаимодействия для PasswordDialog.xaml
    /// </summary>
    public partial class PasswordDialog : Window
    {
        public PasswordDialog()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get => ResponseTextBox.Password;
            set => ResponseTextBox.Password = value;
        }


        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}
