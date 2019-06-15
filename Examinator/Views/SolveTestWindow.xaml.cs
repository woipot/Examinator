using System.ComponentModel;
using System.Windows;
using Examinator.mvvm.models.subModels;
using Examinator.mvvm.viewmodels;

namespace Examinator.Views
{
    /// <summary>
    /// Interaction logic for SolveTestWindow.xaml
    /// </summary>
    public partial class SolveTestWindow : Window
    {
        public SolveTestWindow(TestModel testModel, PreloadedTestInfo preloadedInfo, string studentName, string group)
        {
            InitializeComponent();
            ((ExaminatorViewModel)DataContext).SetData(testModel, studentName, group);

        }

        private void SolveTestWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (((ExaminatorViewModel) DataContext).IsSolved)
                return;

            var res = ExaminatorViewModel.EndTestDialog();
            if(res)
                ((ExaminatorViewModel)DataContext).EndTest();
            
        }
    }
}
