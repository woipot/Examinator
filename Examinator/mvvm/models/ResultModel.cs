using System;

namespace Examinator.mvvm.models
{
    [Serializable]
    public class ResultModel
    {
        private static String[] marks = { "Неудовлетворительно / 2", "Удовлетворительно / 3", "Хорошо / 4", "Отлично / 5" };

        public String TestName { get; set; }

        public String StudentName { get; set; }

        public String Group{ get; set; }
        
        public DateTime StartTime { get; set; }

        public string StartTimeStr => StartTime.ToString("g");
        
        public DateTime FinishTime { get; set; }

        public string FinishTimeStr => FinishTime.ToString("g");


        public String TotalTime => (FinishTime - StartTime).ToString(@"mm") + " мин. " +
                                   (FinishTime - StartTime).ToString(@"ss") + " сек.";

        public int Mark { get; set; } = 2;

        public String MarkStr => marks[Mark - 2];

        public int TotalAnswers { get; set; }

        public double? Perent { get; internal set; }
    }
}
