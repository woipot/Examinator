using System.Collections.ObjectModel;
using System.Xml.Linq;
using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    public class QuestionModel : BindableBase
    {
        public static string DeffaultBlockName = "Question";   

        public string QuestionText { get; set; }

        public ObservableCollection<AnswerModel> Answers { get; }

        public QuestionModel(string questionText)
        {
            QuestionText = questionText;
            Answers = new ObservableCollection<AnswerModel>();
        }

        public QuestionModel(string questionText, ObservableCollection<AnswerModel> answers)
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

        public static QuestionModel FromXML(XElement questElement, string answerName)
        {
            var text = questElement.FirstAttribute.Value;
            var result = new QuestionModel(text);

            var answers = questElement.Elements(answerName);

            foreach (var answerElement in answers)
            {
                var answer = AnswerModel.FromXML(answerElement);
                result.Answers.Add(answer);
            }
            return result;
        }
    }
}
