using System.Collections.ObjectModel;
using System.Xml.Linq;
using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    class QuestionModel : BindableBase
    {
        public static string DeffaultBlockName = "Question";   

        public string QuestionText { get; set; }

        public ObservableCollection<Answer> Answers { get; }

        public QuestionModel(string questionText)
        {
            QuestionText = questionText;
            Answers = new ObservableCollection<Answer>();
        }

        public QuestionModel(string questionText, ObservableCollection<Answer> answers)
        {
            QuestionText = questionText;
            Answers = answers;
        }

        public XElement ToXML(string blockName, string answersBlockName)
        {
            var element = new XElement(blockName);
            var textAttr = new XAttribute("Text", QuestionText);
            element.Add(textAttr);

            foreach (var answer in Answers)
            {
                var xmlAnswer = answer.ToXML(answersBlockName);
                element.Add(xmlAnswer);
            }

            return element;
        }
    }
}
