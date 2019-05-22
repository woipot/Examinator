using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Mvvm;
using Examinator.Extensions;
using Examinator.mvvm.models.subModels;

namespace Examinator.mvvm.models
{
    class ExaminatorViewModel : BindableBase
    {

        private PreloadedTestInfo _info;

        public DelegateCommand<int> ChangeQuestionCommand { get; set; }
        public DelegateCommand EndTestCommand { get; set; }
        public DelegateCommand NextQuestionCommand { get; set; }
        public DelegateCommand PreviousQuestionCommand { get; set; }

        public long TimeLeft { get; set; }

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
            SelectedQuestion = Questions.FirstOrDefault();
            EndTestCommand = new DelegateCommand(EndTest);
            NextQuestionCommand = new DelegateCommand(NextQuestion);
            ChangeQuestionCommand = new DelegateCommand<int>(ChangeQuestion);
            PreviousQuestionCommand = new DelegateCommand(PreviousQuestion);
            Questions = new ObservableCollection<QuestionModel>(Questions.ToList().GetRange(0, testModel.QuestionsInTest));
            RaisePropertiesChanged("Questions");

            TimeLeft = testModel.MinutsToTest * 60;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            TimeLeft--;

            var timespan = TimeSpan.FromSeconds(TimeLeft);

            TimeLeftStr = timespan.ToString(@"mm\:ss");
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

        private void EndTest()
        {

        }


        private void NextQuestion()
        {
            SwitchQuestion(SelectedQuestion.Number + 1);
        }




        private void PreviousQuestion()
        {
            SwitchQuestion(SelectedQuestion.Number - 1);
        }


    }
}