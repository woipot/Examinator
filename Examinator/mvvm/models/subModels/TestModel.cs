using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.Mvvm;
using Examinator.other;

namespace Examinator.mvvm.models.subModels
{
    public class TestModel : BindableBase
    {
        public static string DeffaultBlockName = "Test";

        public TestModel()
        {
            Questions = new ObservableCollection<QuestionModel>();
            DeleteCommand = new DelegateCommand<QuestionModel>(Delete);
        }


        public bool Skipable { get; set; } = true;

        public string TestName { get; set; } = "Test";

        public string Author { get; set; } = "Author";

        public string CreatedDate { get; set; } = DateTime.Now.ToString();

        public int MinutsToTest { get; set; } = 10;

        public int QuestionsInTest { get; set; } = 10;

        public ObservableCollection<QuestionModel> Questions { get; }


        public XDocument ToXML(string documentName, string questionName, string answerName)
        {
            var xdoc = new XDocument();

            var test = new XElement(documentName);
            var testNameAttr = new XAttribute("Name", TestName);
            var testTimeAttr = new XAttribute("Time", MinutsToTest);
            var testCountAttr = new XAttribute("Count", QuestionsInTest);
            var testSkipableAttr = new XAttribute("Skipable", Skipable);
            test.Add(testNameAttr);
            test.Add(testTimeAttr);
            test.Add(testCountAttr);
            test.Add(testSkipableAttr);

            if (!string.IsNullOrEmpty(CreatedDate))
            {
                var testDateAttr = new XAttribute("Date", CreatedDate);
                test.Add(testDateAttr);
            }

            if (!string.IsNullOrEmpty(Author))
            {
                var testAuthorAttr = new XAttribute("Author", Author);
                test.Add(testAuthorAttr);
            }

            foreach (var questionModel in Questions)
            {
                var question = questionModel.ToXML(questionName, answerName);
                test.Add(question);
            }

            xdoc.Add(test);

            return xdoc;
        }

        public static TestModel FromXMl(XDocument xdoc, string documentName, string questionName, string answerName,
            bool needToParseQuestions = true)
        {
            var result = new TestModel();

            var testElement = xdoc.Element(documentName);
            if (testElement == null)
                throw new TestException("Файл испорчен: невозможно прочитать заголовок");

            var nameAttr = testElement.Attribute("Name");
            var dateAttr = testElement.Attribute("Date");
            var timeAttr = testElement.Attribute("Time");
            var countAttr = testElement.Attribute("Count");
            var skipableAttr = testElement.Attribute("Skipable");
            var authorAttr = testElement.Attribute("Author");

            result.TestName = nameAttr?.Value ??
                              throw new TestException("Файл испорчен: невозможно прочитать название теста");

            if (timeAttr == null)
                throw new TestException("Файл испорчен: невозможно определить время на прохождение теста");
            result.MinutsToTest = int.Parse(timeAttr.Value);

            if (countAttr == null)
                throw new TestException("Файл испорчен: невозможно определить кол-во вопросов на тест");
            result.QuestionsInTest = int.Parse(countAttr.Value);

            if (skipableAttr != null)
            {
                result.Skipable = bool.Parse(skipableAttr.Value);
            }

            if (authorAttr != null)
            {
                result.Author = authorAttr.Value;
            }

            if (dateAttr != null)
            {
                result.CreatedDate = dateAttr.Value;
            }

            var xquestions = testElement.Elements(questionName);

            if (needToParseQuestions)
            {
                var questions = QuestionsFromXElement(xquestions, answerName);
                foreach (var questionModel in questions)
                {
                    result.Questions.Add(questionModel);
                }
            }

            return result;
        }

        public static IEnumerable<QuestionModel> QuestionsFromXElement(IEnumerable<XElement> elements,
            string answerName)
        {
            var result = new List<QuestionModel>();

            foreach (var questionElemnt in elements)
            {
                var question = QuestionModel.FromXML(questionElemnt, answerName);
                result.Add(question);
            }

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"TestName = {TestName}");
            if(!string.IsNullOrEmpty(Author))
                sb.AppendLine($"Author = {Author}");

            sb.AppendLine($"Date = {CreatedDate}");

            sb.AppendLine($"Time = {MinutsToTest}");
            sb.AppendLine($"QuestionCount = {QuestionsInTest}");
            sb.AppendLine($"Skipable = {Skipable}\n");

            foreach (var questionModel in Questions)
            {
                sb.AppendLine(questionModel.ToString());
            }

            return sb.ToString();
        }


        public DelegateCommand<QuestionModel> DeleteCommand { get; }

        public void Delete(QuestionModel question)
        {
            Questions.Remove(question);
        }


        public Tuple<string, bool> CheckToCorrect()
        {
            var sb = new StringBuilder();

            var critical = false;

            if (string.IsNullOrEmpty(TestName))
            {
                sb.AppendLine("*<Отсутствует название теста>");
                critical = true;
            }

            if (string.IsNullOrEmpty(Author))
            {
                sb.AppendLine("<Отсутствует Автор>");
            }

            if (MinutsToTest < 10)
            {
                sb.AppendLine("<Время на вопрос < 10 сек>");
            }

            if (QuestionsInTest < 1)
            {
                sb.AppendLine("<Кол-во вопросов для теста < 1 >");
            }

            var correctCounter = 0;
            foreach (var questionModel in Questions)
            {
                var res = questionModel.CheckToCorrect();

                if (res.Item2 == false)
                    correctCounter++;
                else if (!string.IsNullOrEmpty(res.Item1))
                {
                    sb.AppendLine(res.Item1);

                    if (res.Item2)
                        critical = true;
                }
            }

            if (correctCounter == 0)
            {
                sb.AppendLine("*<Отсутствуют корректные вопросы>");
                critical = true;
            }

            return new Tuple<string, bool>(sb.ToString(), critical);
        }

        public void Clean()
        {
            if (string.IsNullOrEmpty(TestName))
            {
                TestName = "Заголовок теста";
            }

            if (string.IsNullOrEmpty(Author))
            {
                Author = "";
            }

            if (MinutsToTest < 10)
            {
                MinutsToTest = 10;
            }

            if (QuestionsInTest < 1)
            {
                QuestionsInTest = 20;
            }

            for (var i = 0; i < Questions.Count; i++)
            {
                var questionModel = Questions[i];
                questionModel.Clean();

                if(!questionModel.Answers.Any())
                    Questions.Remove(questionModel);
            }

        }
    }        
}
