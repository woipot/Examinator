using System.Windows;
using Examinator.other;

namespace Examinator.Views
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow(ResulyInfo result)
        {
            InitializeComponent();

            ResultLabel.Content = $"";
        }
    }
}
