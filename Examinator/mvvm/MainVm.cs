﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            _loader = new Loader();

            if (_loader.LoadExceptions.Any())
            {
                MessageBox.Show("Фаилы повреждены : \n" + string.Join("\n", _loader.LoadExceptions));
            }

            SwitchModeCommand = new DelegateCommand(SwitchMode);
            ViewTestCommand = new DelegateCommand<object>(OpenViewWindow);
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

        public void OpenViewWindow(object param)
        {
            var preloadedInfo = param as PreloadedTestInfo;

            if (preloadedInfo == null)
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

    }
}