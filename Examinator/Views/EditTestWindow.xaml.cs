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
            var result = MessageBox.Show("Убедитесь, что сохранили изменения, иначе они будут утеряны", "Вы уверены?", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;

        }
    }
}
