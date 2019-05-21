using System;
using System.Text;
using System.Xml.Linq;
using DevExpress.Mvvm;
using Examinator.other;

namespace Examinator.mvvm.models.subModels
{
    public class AnswerModel : BindableBase, ICloneable
    {
        public static string DeffaultBlockName = "Answer";

        public string AnswerText { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRight { get; set;  }

        public AnswerModel(string answerText, bool isRight)
        {
            AnswerText = answerText;
            IsRight = isRight;
        }

        public AnswerModel(AnswerModel answerModel)
        {
            AnswerText = answerModel.AnswerText;
            IsRight = answerModel.IsRight;
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

        public static AnswerModel FromXML(XElement element)
        {
            var textAttr = element.Attribute("AnswerText");
            if(textAttr == null)
                throw new TestException("Фаил поврежден: отсутствует текст ответа");

            var isRightAttr = element.Attribute("IsRight");
            var isRight = false;
            if (isRightAttr != null)
                isRight = bool.Parse(isRightAttr.Value);

            return new AnswerModel(textAttr.Value, isRight);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var first = IsRight ? "+" : "-";
            sb.Append($"{first}{AnswerText}");

            return sb.ToString();
        }

        public object Clone()
        {
            return new AnswerModel(this);
        }
    }
}
