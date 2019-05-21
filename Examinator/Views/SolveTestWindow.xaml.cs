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
using Examinator.mvvm.models;
using Examinator.mvvm.models.subModels;

namespace Examinator.Views
{
    /// <summary>
    /// Interaction logic for SolveTestWindow.xaml
    /// </summary>
    public partial class SolveTestWindow : Window
    {
        public SolveTestWindow(TestModel testModel, PreloadedTestInfo preloadedInfo)
        {
            InitializeComponent();
            ((ExaminatorViewModel)DataContext).SetData(testModel, preloadedInfo);
        }
    }
}
