using System.ComponentModel;
using System.Windows;
using Examinator.mvvm.models.subModels;
using Examinator.mvvm.viewmodels;
using Examinator.other;

namespace Examinator.Views
{
    /// <summary>
    /// Interaction logic for SolveTestWindow.xaml
    /// </summary>
    public partial class SolveTestWindow : Window
    {
        public SolveTestWindow(TestModel testModel, PreloadedTestInfo preloadedInfo, string studentName, string group, MarkClass marks)
        {
            InitializeComponent();
            ((ExaminatorViewModel)DataContext).SetData(testModel, studentName, group, marks);

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
