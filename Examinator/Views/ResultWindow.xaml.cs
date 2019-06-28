using System.Windows;
using Examinator.mvvm.models;
using Examinator.mvvm.viewmodels;

namespace Examinator.Views
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow(ResultModel resultmodel)
        {
            InitializeComponent();

            ((ResultsViewModel) DataContext).ResultModel = resultmodel;
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            ((ResultsViewModel)DataContext).LoadResults();
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
