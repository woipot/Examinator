using System.Windows;

namespace Examinator.Views
{
    /// <summary>
    /// Логика взаимодействия для StudentInputDialog.xaml
    /// </summary>
    public partial class StudentInputDialog : Window
    {
        public string StudentName { get; set; } 

        public string Group { get; set; }

        public StudentInputDialog()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            StudentName = NameTextBox.Text;
            Group = GroupTextBox.Text;

            if (string.IsNullOrEmpty(StudentName) || string.IsNullOrEmpty(Group))
            {
                MessageBox.Show("Вы должны заполнить информацию", "Ошибка");
            }
            else
            {
                StudentName = StudentName.Trim().ToLower();
                Group = Group.Trim().ToLower();
                DialogResult = true;
            }
        }
    }
}
