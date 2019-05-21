using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            CopyCommand = new DelegateCommand<QuestionModel>(AddCopy);
            AddEmptyQuestionCommand = new DelegateCommand(AddEmptyQuestion);
        }

        public bool Skipable { get; set; } = true;

        public string TestName { get; set; }

        public string Author { get; set; }

        public string CreatedDate { get; set; }

        public int MinutsToTest { get; set; }

        public int QuestionsInTest { get; set; }

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
                throw new TestException("Фаил испорчен: невозможно прочитать заголовок");

            var nameAttr = testElement.Attribute("Name");
            var dateAttr = testElement.Attribute("Date");
            var timeAttr = testElement.Attribute("Time");
            var countAttr = testElement.Attribute("Count");
            var skipableAttr = testElement.Attribute("Skipable");
            var authorAttr = testElement.Attribute("Author");

            result.TestName = nameAttr?.Value ??
                              throw new TestException("Фаил испорчен: невозможно прочитать название теста");

            if (timeAttr == null)
                throw new TestException("Фаил испорчен: невозможно определить время на прохождение теста");
            result.MinutsToTest = int.Parse(timeAttr.Value);

            if (countAttr == null)
                throw new TestException("Фаил испорчен: невозможно определить кол-во вопросов на тест");
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

        public DelegateCommand AddEmptyQuestionCommand { get; }

        public void AddEmptyQuestion()
        {
            Questions.Add(new QuestionModel(""));
        }

        public DelegateCommand<QuestionModel> CopyCommand { get; }

        public void AddCopy(QuestionModel question)
        {
            Questions.Add(new QuestionModel(question));
        }
    }        
}
