using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using DevExpress.Mvvm;
using Examinator.mvvm.models.subModels;
using Examinator.Views;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;

namespace Examinator.mvvm.models
{
    public class RedactorModel : BindableBase
    {
        private TestModel _testModel;
        private PreloadedTestInfo _info;

        public ObservableCollection<QuestionModel> Questions => _testModel?.Questions;

        public RedactorModel()
        {
            CloseWindowCommand = new DelegateCommand<Window>(CloseWindow);
        }


        public TestModel TestModel => _testModel;

        public string FullTime
        {
            get
            {
                if (_testModel == null)
                    return "0 мин";
                var total =  _testModel.MinutsToTest * Math.Min(_testModel.QuestionsInTest, _testModel.Questions.Count);
                int seconds = total % 60;
                int minutes = total / 60;

                return $"{minutes} мин :  {seconds} сек";
            }
        }

        public int MinutToTest
        {
            get
            {
                return _testModel != null ? _testModel.MinutsToTest : 0; }

            set
            {
                if (_testModel != null)
                {
                    _testModel.MinutsToTest = value;
                    RaisePropertiesChanged("FullTime");
                }
            }
        }

        public int QuestionsInTestCount
        {
            get
            {
                return _testModel != null ? _testModel.QuestionsInTest : 0;
            }

            set
            {
                if (_testModel != null)
                {
                    _testModel.QuestionsInTest = value;
                    RaisePropertiesChanged("FullTime");
                }
            }
        }

        public void SetData(TestModel testModel, PreloadedTestInfo preloadedInfo)
        {
            _testModel = testModel;
            _info = preloadedInfo;
            RaisePropertiesChanged("Questions");
            RaisePropertiesChanged("TestModel");
            RaisePropertiesChanged("FullTime");
            RaisePropertiesChanged("MinutToTest");
            RaisePropertiesChanged("QuestionsInTestCount");
        }



        public DelegateCommand<Window> CloseWindowCommand { get; }

        public void CloseWindow(Window window)
        {
            window?.Close();
        }

    }
}
