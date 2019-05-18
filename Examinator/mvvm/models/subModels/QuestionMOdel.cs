using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    class QuestionModel : BindableBase
    {
        public string QuestionText { get; set; }

        public ObservableCollection<Answer> Answers { get; }

        public QuestionModel(string questionText, ObservableCollection<Answer> answers)
        {
            QuestionText = questionText;
            Answers = answers;
        }

    }
}
