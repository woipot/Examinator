using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    class Answer : BindableBase
    {
        public string AnswerText { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRight { get; set;  }

        public Answer(string answerText, bool isRight)
        {
            AnswerText = answerText;
            IsRight = isRight;
        }
    }
}
