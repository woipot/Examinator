using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using Examinator.mvvm.models;
using Examinator.mvvm.models.subModels;

namespace Examinator.mvvm
{
    public class MainVm : BindableBase
    {
        private Loader _loader;

        public ObservableCollection<PreloadedTestInfo> PreloadedTests => _loader.PreloadedTests;

        public MainVm()
        {
            _loader = new Loader();

            if (_loader.LoadExceptions.Any())
            {
                MessageBox.Show("Фаилы повреждены : \n" + string.Join("\n", _loader.LoadExceptions));
            }
        }

    }
}
