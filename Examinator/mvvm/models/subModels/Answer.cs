using System.Xml.Linq;
using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    class Answer : BindableBase
    {
        public static string DeffaultBlockName = "Answer";

        public string AnswerText { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRight { get; set;  }

        public Answer(string answerText, bool isRight)
        {
            AnswerText = answerText;
            IsRight = isRight;
        }

        public XElement ToXML(string elementName)
        {
            var element = new XElement(elementName);
            
            var textAttr = new XAttribute("AnswerText", AnswerText);
            element.Add(textAttr);

            var rightAttr = new XAttribute("IsRight", IsRight);
            element.Add(rightAttr);

            return element;
        }
    }
}
