using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using Examinator.Extensions;
using Examinator.mvvm.models;
using Examinator.mvvm.models.subModels;
using Examinator.Views;

namespace Examinator.mvvm.viewmodels
{
    class ExaminatorViewModel : BindableBase
    {
        private DispatcherTimer _timer;
        private DateTime _startTime;
        public DelegateCommand<int> ChangeQuestionCommand { get; set; }
        public DelegateCommand EndTestCommand { get; set; }
        public DelegateCommand NextQuestionCommand { get; set; }
        public DelegateCommand PreviousQuestionCommand { get; set; }

        public int TimeLeft { get; set; }
        private int _answered;
        public String TimeLeftStr { get; set; }

        private int _questionsCount; 

        public QuestionModel SelectedQuestion { get; set; }

        public TestModel TestModel { get; set; }

        public ObservableCollection<QuestionModel> Questions { get; set; }

        private string _studentName;
        private string _group;


        public void SetData(TestModel testModel, string studentName, string group)
        {
            _studentName = studentName;
            _group = group;

            TestModel = testModel;
            Questions = TestModel?.Questions;
            RandomizeQuestions();

            _questionsCount = Math.Min(testModel.QuestionsInTest, testModel.Questions.Count);

            EndTestCommand = new DelegateCommand(EndTestByCommand);
            NextQuestionCommand = new DelegateCommand(NextQuestion);
            ChangeQuestionCommand = new DelegateCommand<int>(ChangeQuestion);
            PreviousQuestionCommand = new DelegateCommand(PreviousQuestion);

            Questions = new ObservableCollection<QuestionModel>(Questions.ToList()
                .GetRange(0, _questionsCount));

            SelectedQuestion = Questions.FirstOrDefault();
            if (SelectedQuestion != null) SelectedQuestion.IsCurrent = true;
            RaisePropertiesChanged("Questions");

            _startTime=DateTime.Now;
            TimeLeft = TestModel.Skipable
                ? TestModel.MinutsToTest * _questionsCount
                : TestModel.MinutsToTest;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;
            _timer.Start();
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
                if (SelectedQuestion.Number >= _questionsCount)
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
            _answered = 0;
            foreach (var question in Questions)
            {
                var questionResult = true;
                var isAnswered = false;
                foreach (var answer in question.Answers)
                {
                    if (answer.IsSelected)
                        isAnswered = true;
                    if (answer.IsSelected != answer.IsRight)
                        questionResult = false;
                }

                if (isAnswered)
                    _answered++;
                if (questionResult)
                    rightAnswers++;
            }

            return rightAnswers;
        }

        public void RandomizeQuestions()
        {
            Questions.Shuffle();
            var num = 1;
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
            var result = MessageBox.Show("Завершить тест и получить результат?", "Вы уверены?",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                EndTest();
            }
        }

        private void EndTest()
        {

            _timer.Stop();

            
            double results = CalculateResults(Questions);

            var res = results==0? 0: results/ _questionsCount * 100;

            if (res >= 90)
            {
                res = 5;
            }else if (res>=75)
            {
                res = 4;
            }
            else if (res >= 60)
            {
                res = 3;
            }
            else
            {
                res = 2;
            }


            var resultWindow = new ResultWindow(new ResultModel()
            {
                Mark = (int) res, QuestionsCount = _questionsCount,
                CorrectAnswersCount = (int) results,
                StudentName = _studentName,
                Group = _group,
                TestName = TestModel.TestName,
                StartTime = _startTime,
                FinishTime = DateTime.Now,
                TestAuthor = TestModel.Author,
                TestDate = TestModel.CreatedDate,
                TotalAnswers = _answered
            });
            resultWindow.ShowDialog();

            try
            {
                var windows = Application.Current.Windows;
                foreach (var window in windows)
                {
                    if (window is SolveTestWindow thisWindow)
                    {
                        thisWindow.Close();
                    }
                }
            }
            catch (Exception e)
            {
            }

        }

        


        private void NextQuestion()
        {
            if (SelectedQuestion.Number == _questionsCount && !TestModel.Skipable)
            {
                EndTest();
            }
                SwitchQuestion(SelectedQuestion.Number + 1);
            if (!TestModel.Skipable)
            {
                if (SelectedQuestion.Number > _questionsCount)
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