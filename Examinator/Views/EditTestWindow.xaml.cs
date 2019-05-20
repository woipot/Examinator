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
    }
}
