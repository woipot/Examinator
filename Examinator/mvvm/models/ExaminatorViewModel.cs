using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using Examinator.Extensions;
using Examinator.mvvm.models.subModels;
using Examinator.Views;

namespace Examinator.mvvm.models
{
    class ExaminatorViewModel : BindableBase
    {
        private PreloadedTestInfo _info;
        private DispatcherTimer Timer;
        public DelegateCommand<int> ChangeQuestionCommand { get; set; }
        public DelegateCommand EndTestCommand { get; set; }
        public DelegateCommand NextQuestionCommand { get; set; }
        public DelegateCommand PreviousQuestionCommand { get; set; }

        public int TimeLeft { get; set; }

        public String TimeLeftStr { get; set; }


        public QuestionModel SelectedQuestion { get; set; }

        public TestModel TestModel { get; set; }

        public ObservableCollection<QuestionModel> Questions { get; set; }

        public void SetData(TestModel testModel, PreloadedTestInfo preloadedInfo)
        {
            TestModel = testModel;
            _info = preloadedInfo;
            Questions = TestModel?.Questions;
            RandomizeQuestions();
            EndTestCommand = new DelegateCommand(EndTestByCommand);
            NextQuestionCommand = new DelegateCommand(NextQuestion);
            ChangeQuestionCommand = new DelegateCommand<int>(ChangeQuestion);
            PreviousQuestionCommand = new DelegateCommand(PreviousQuestion);
            Questions = new ObservableCollection<QuestionModel>(Questions.ToList()
                .GetRange(0, Math.Min(testModel.QuestionsInTest, testModel.Questions.Count)));
            SelectedQuestion = Questions.FirstOrDefault();
            SelectedQuestion.IsCurrent = true;
            RaisePropertiesChanged("Questions");


            TimeLeft = TestModel.Skipable
                ? TestModel.MinutsToTest * TestModel.QuestionsInTest
                : TestModel.MinutsToTest;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += timer_Tick;
            Timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            TimeLeft--;

            var timespan = TimeSpan.FromSeconds(TimeLeft);

            if (TimeLeft < 0 && TestModel.Skipable)
            {
                EndTest();
                return;
            }

            if (TimeLeft < 0 && !TestModel.Skipable)
            {
                SwitchQuestion(SelectedQuestion.Number + 1);
                if (SelectedQuestion.Number >= TestModel.QuestionsInTest)
                {
                    EndTest();
                    return;
                }

                TimeLeft = TestModel.MinutsToTest;
            }

            TimeLeftStr = timespan.ToString(@"mm\:ss");
        }

        private int CalculateResults(IEnumerable<QuestionModel> Questions)
        {
            var rightAnswers = 0;
            foreach (var question in Questions)
            {
                bool questionResult = true;
                foreach (var answer in question.Answers)
                {
                    if (answer.IsSelected != answer.IsRight)
                        questionResult = false;
                }

                if (questionResult)
                    rightAnswers++;
            }

            return rightAnswers;
        }

        public void RandomizeQuestions()
        {
            Questions.Shuffle();
            int num = 1;
            foreach (var question in Questions)
            {
                question.Number = num++;
                question.Answers.Shuffle();
            }
        }

        private void ChangeQuestion(int num)
        {
            SwitchQuestion(num);
        }

        private void SwitchQuestion(int num)
        {
            if (Questions != null && num > 0 && Questions.Count > num - 1)
            {
                SelectedQuestion.IsSolved = false;
                foreach (var answer in SelectedQuestion.Answers)
                {
                    if (answer.IsSelected)
                    {
                        SelectedQuestion.IsSolved = true;
                        break;
                    }
                }

                SelectedQuestion.IsCurrent = false;
                SelectedQuestion = Questions[num - 1];
                SelectedQuestion.IsCurrent = true;
                RaisePropertyChanged("Questions");
            }
        }

        private void EndTestByCommand()
        {
            var result =  MessageBox.Show("Завершить тест и получить результат?", "Вы уверены?", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                EndTest();
            }
        }

        private void EndTest()
        {
            Timer.Stop();
            var results = CalculateResults(Questions);
            MessageBox.Show($"{results}/{Math.Min(TestModel.QuestionsInTest, TestModel.Questions.Count)}");
            var windows = App.Current.Windows;
            foreach (var window in windows)
            {
                if (window is SolveTestWindow thisWindow)
                {
                    thisWindow.Close();
                }
            }
        }



        private void NextQuestion()
        {
            SwitchQuestion(SelectedQuestion.Number + 1);
            if (!TestModel.Skipable)
            {
                if (SelectedQuestion.Number >= TestModel.QuestionsInTest)
                {
                    EndTest();
                }
                else
                {
                    TimeLeft = TestModel.MinutsToTest;
                }
            }
        }

        private void PreviousQuestion()
        {
            SwitchQuestion(SelectedQuestion.Number - 1);
        }


    }
}