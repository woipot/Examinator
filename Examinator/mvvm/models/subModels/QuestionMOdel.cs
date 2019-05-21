using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    public class QuestionModel : BindableBase, ICloneable
    {
        public static string DeffaultBlockName = "Question";   

        public string QuestionText { get; set; }

        public ObservableCollection<AnswerModel> Answers { get; }


        public QuestionModel(string questionText)
        {
            QuestionText = questionText;
            Answers = new ObservableCollection<AnswerModel>();
            DeleteCommand = new DelegateCommand<AnswerModel>(Delete);
            AddEmptyCommand = new DelegateCommand(AddEmpty);
        }

        public QuestionModel(QuestionModel question)
        {
            QuestionText = question.QuestionText;
            Answers = new ObservableCollection<AnswerModel>(question.Answers);

            DeleteCommand = new DelegateCommand<AnswerModel>(Delete);
            AddEmptyCommand = new DelegateCommand(AddEmpty);
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"={QuestionText}");

            foreach (var answerModel in Answers)
            {
                sb.AppendLine(answerModel.ToString());
            }

            return sb.ToString();
        }


        public object Clone()
        {
            return new QuestionModel(this);
        }


        public DelegateCommand<AnswerModel> DeleteCommand { get; }

        public void Delete(AnswerModel answer)
        {
            Answers.Remove(answer);
        }


        public DelegateCommand AddEmptyCommand { get; }

        public void AddEmpty()
        {
            Answers.Add(new AnswerModel("", false));
        }


        public Tuple<string, bool> CheckToCorrect()
        {
            var sb = new StringBuilder();

            var critical = false;
            if (string.IsNullOrEmpty(QuestionText))
            {
                sb.AppendLine("*<Отсутствует текст вопроса>");
                sb.AppendLine(ToString());
                critical = true;
            }

            var counter = 0;
            var isRightAnswers = 0;

            foreach (var answerModel in Answers)
            {
                var isCorrect = answerModel.IsCorrect();
                if (isCorrect)
                {
                    counter++;
                    if (answerModel.IsRight)
                        isRightAnswers++;
                }
            }

            if (isRightAnswers == 0)
            {
                sb.AppendLine("*<Не отмечен Верный ответ> : " + QuestionText);
                critical = true;
            }

            if (counter == 0)
            {
                sb.AppendLine("*<Отсутствуют ответы> : " + QuestionText);
                critical = true;
            }

            return new Tuple<string, bool>(sb.ToString(), critical);
        }

        public void Clean()
        {
            if (string.IsNullOrEmpty(QuestionText))
                QuestionText = "Заголовок вопроса";

            for (var i = 0; i < Answers.Count; i++)
            {
                var answerModel = Answers[i];

                if (!answerModel.IsCorrect())
                    Answers.Remove(answerModel);
            }

        }
    }
}
