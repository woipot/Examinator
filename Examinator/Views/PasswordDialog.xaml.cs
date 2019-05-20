using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
