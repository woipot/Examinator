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

        public long TimeLeft { get; set; }

        public String TimeLeftStr { get; set; }
        

        public QuestionModel SelectedQuestion { get; set; }

        public TestModel TestModel { get; set; }

        public ObservableCollection<QuestionModel> Questions => TestModel?.Questions;

        public void SetData(TestModel testModel, PreloadedTestInfo preloadedInfo)
        {
            TestModel = testModel;
            _info = preloadedInfo;
            RandomizeQuestions();
            SelectedQuestion = Questions.FirstOrDefault();
            ChangeQuestionCommand= new DelegateCommand<int>(ChangeQuestion);
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
            if (Questions != null && Questions.Count > num - 1)
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

       

    }
}
