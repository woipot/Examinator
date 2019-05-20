using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using Examinator.mvvm.models;
using Examinator.mvvm.models.subModels;
using Examinator.other;
using Examinator.Views;

namespace Examinator.mvvm
{
    public class MainVm : BindableBase
    {
        private readonly Loader _loader;

        public bool TeacherMode { get; set; }

        public ObservableCollection<PreloadedTestInfo> PreloadedTests => _loader.PreloadedTests;

        public MainVm()
        {
            #if DEBUG
                TeacherMode = true;
            #endif

            _loader = new Loader();

            if (_loader.LoadExceptions.Any())
            {
                MessageBox.Show("Фаилы повреждены : \n" + string.Join("\n", _loader.LoadExceptions));
            }

            SwitchModeCommand = new DelegateCommand(SwitchMode);
            ViewTestCommand = new DelegateCommand<object>(OpenViewWindow);
            EditTestCommand = new DelegateCommand<object>(OpenEditWindow);
        }


        public DelegateCommand SwitchModeCommand { get; }

        private void SwitchMode()
        {
            if (TeacherMode)
            {
                TeacherMode = false;
                RaisePropertiesChanged("TeacherMode");
                return;
            }

            var dialog = new PasswordDialog();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.ResponseText.GetHashCode().ToString() == "-1359372755")
                    TeacherMode = true;
                else
                    MessageBox.Show("Пароль неверен, проверьте язык ввода и capslock, и повторите попытку");
            }
            RaisePropertiesChanged("TeacherMode");

        }

        public DelegateCommand<object> ViewTestCommand { get; }

        private static void OpenViewWindow(object param)
        {
            if (!(param is PreloadedTestInfo preloadedInfo))
            {
                MessageBox.Show("Внутреняя ошибка: Невозможно открыть данный тест");
                return;
            }

            try
            {
                var testModel = Loader.LoadTest(preloadedInfo.AssociatedPath);
                var viewWindow = new TestViewWindow(testModel);

                viewWindow.Show();
            }
            catch (TestException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить файл");
            }
        }


        public DelegateCommand<object> EditTestCommand { get; }

        private static void OpenEditWindow(object param)
        {
            if (!(param is PreloadedTestInfo preloadedInfo))
            {
                MessageBox.Show("Внутреняя ошибка: Невозможно открыть данный тест");
                return;
            }

            try
            {
                var testModel = Loader.LoadTest(preloadedInfo.AssociatedPath);

                var editWindow = new EditTestWindow(testModel, preloadedInfo);
                editWindow.Show();
                
            }
            catch (TestException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить файл");
            }
        }

    }
}
