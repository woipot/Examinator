﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using Examinator.mvvm.models;
using Examinator.mvvm.models.subModels;
using Examinator.other;
using Examinator.Views;
using Microsoft.Win32;

namespace Examinator.mvvm.viewmodels
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
            SolveTestCommand = new DelegateCommand<object>(OpenSolveWindow);
            CreateNewTestCommand = new DelegateCommand(CreateNewTest);
            DeleteCommand = new DelegateCommand<PreloadedTestInfo>(Delete);
            ImportCommand = new DelegateCommand(ImportTest);
            ShowInstructionCommand = new DelegateCommand(ShowInstruction);
            ShowResultsCommand = new DelegateCommand(ShowResults);
            ShowAboutCommand = new DelegateCommand(ShowAbout);
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
                if (dialog.ResponseText.GetHashCode().ToString().Equals("-891927694"))
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

                viewWindow.ShowDialog();
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
                editWindow.ShowDialog();

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


        public DelegateCommand<object> SolveTestCommand { get; }
        
        private void OpenSolveWindow(object param)
        {
            if (!(param is PreloadedTestInfo preloadedInfo))
            {
                MessageBox.Show("Внутреняя ошибка: Невозможно открыть данный тест");
                return;
            }

            var studentInfoWindow = new StudentInputDialog();
            if (studentInfoWindow.ShowDialog() == true)
            {

                try
                {
                    var testModel = Loader.LoadTest(preloadedInfo.AssociatedPath);
                    MarkClass marks = new MarkClass();
                    try
                    {
                        marks = Loader.LoadMark(_loader.PathToMarkFile);
                    } catch (TestException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    var solveWindow = new SolveTestWindow(testModel, preloadedInfo, studentInfoWindow.StudentName, studentInfoWindow.Group, marks);
                    solveWindow.ShowDialog();

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


        public DelegateCommand CreateNewTestCommand { get; }

        private void CreateNewTest()
        {
            try
            {
                var testModel = new TestModel();

                var editWindow = new EditTestWindow(testModel, new PreloadedTestInfo(null, _loader.PathToTests));
                
                if (editWindow.ShowDialog() == true)
                {
                    _loader.PreloadedTests.Insert(0, ((RedactorModel)editWindow.DataContext).Info);
                }

            }
            catch (TestException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить/сохранить файл");
            }
        }


        public DelegateCommand<PreloadedTestInfo> DeleteCommand { get; }

        private void Delete(PreloadedTestInfo info)
        {
            var result = MessageBox.Show("Востановить тест будет невозможно", "Вы уверены?", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if(result == MessageBoxResult.Cancel)
                return;

            try
            {
                Loader.DeleteTest(info.AssociatedPath);
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: Возможно, файл уже удален");
            }

            _loader.PreloadedTests.Remove(info);
        }


        public DelegateCommand ImportCommand { get; }

        private void ImportTest()
        {
            var openFileDialog =
                new OpenFileDialog
                {
                    Filter = "Текстовые файлы (*.txt)|*.txt"
                };

            if (openFileDialog.ShowDialog() == false)
                return;

            var p  = openFileDialog.FileName;

            try
            {
                var test = Loader.LoadFromFile(p);

                var realPath = Loader.SaveTest(test, _loader.PathToTests);

                _loader.PreloadedTests.Insert(0, new PreloadedTestInfo(test.TestName, realPath));
            }
            catch (TestException e)
            {
                var errorWindow = new ErrorWindow(e.Message + "\n" + e.AdditionalErrorInfo);
                errorWindow.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show("Что-то пошло не так: невозможно загрузить/сохранить файл");
            }
        }


        public DelegateCommand ShowInstructionCommand { get; }

        private void ShowInstruction()
        {
            var window = new HelpWindow();
            window.Show();
        }


        public DelegateCommand ShowResultsCommand { get; }

        private void ShowResults()
        {
            var resultsWindow = new ResultTableWindow();
            resultsWindow.ShowDialog();
        }


        public DelegateCommand ShowAboutCommand { get; }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Создатели:\nСтульников Кирилл, Удалов Никита,\nКирдяшкин Игорь, Павлов Александр\nСпециально для кафедры военной подготовки\nПо всем вопросам: forandwoicorp@gmail.com", "Контакты", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    
    }
}
