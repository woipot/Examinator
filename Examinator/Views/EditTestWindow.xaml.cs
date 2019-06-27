using System.ComponentModel;
using System.Windows;
using Examinator.mvvm.models;
using Examinator.mvvm.models.subModels;

namespace Examinator.Views
{
    /// <summary>
    /// Логика взаимодействия для EditTestWindow.xaml
    /// </summary>
    public partial class EditTestWindow : Window
    {
        public EditTestWindow(TestModel testModel, PreloadedTestInfo preloadedInfo)
        {
            InitializeComponent();

            ((RedactorModel) DataContext).SetData(testModel, preloadedInfo);
        }

        private void EditTestWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Убедитесь, что Вы нажали на кнопку 'Сохранить изменения', иначе все изменения в тесте не будут сохранены.", "Вы уверены, что хотите выйти?", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);


            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;

            if (((RedactorModel) DataContext).Info.TestName != null)
                DialogResult = true;
        }
    }
}
