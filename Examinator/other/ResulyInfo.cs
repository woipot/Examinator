using System;
using DevExpress.Mvvm;

namespace Examinator.other
{
    public class ResulyInfo
    {
        public string StudentName { get; set; } = "";

        public int RightAnswersCount { get; set; } = 0;

        public int QuestionsCount { get; set; } = 0;

        public int ExamBall { get; set; } = 0;

        public string Date { get; set; } = DateTime.Now.ToString("g");
    }
}
