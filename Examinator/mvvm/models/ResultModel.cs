using System;

namespace Examinator.mvvm.models
{
    [Serializable]
    public class ResultModel
    {
        private static String[] marks = { "Неудовлетворительно", "Удовлетворительно", "Хорошо", "Отлично" };

        public String TestName { get; set; }

        public String TestDate { get; set; }

        public String TestAuthor { get; set; }

        public String StudentName { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime FinishTime { get; set; }
        
        public String TotalTime => (FinishTime - StartTime).ToString(@"mm") + " мин. " +
                                   (FinishTime - StartTime).ToString(@"ss") + " сек.";

        public int CorrectAnswersCount { get; set; }
        public int IncorrectAnswersCount => QuestionsCount - CorrectAnswersCount;

        public int QuestionsCount { get; set; }

        public int Mark { get; set; } = 2;

        public String MarkStr => marks[Mark - 2];

        public int TotalAnswers { get; set; }

    }
}
