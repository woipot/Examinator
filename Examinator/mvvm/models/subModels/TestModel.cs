using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using DevExpress.Mvvm;
using Examinator.other;

namespace Examinator.mvvm.models.subModels
{
    class TestModel : BindableBase
    {
        public static string DeffaultBlockName = "Test";

        public TestModel()
        {
            Questions = new ObservableCollection<QuestionModel>();
        }

        public TestModel(string createdDate)
        {
            Questions = new ObservableCollection<QuestionModel>();
            CreatedDate = createdDate;
        }
        public TestModel(DateTime createdDate, string datePattern = "MM/dd/yyyy")
        {
            Questions = new ObservableCollection<QuestionModel>();
            CreatedDate = createdDate.ToString(datePattern);
        }

        public bool Skipable { get; set; } = true;

        public string TestName { get; set; }

        public string Author { get; set; }

        public string CreatedDate { get; set; }

        public int MinutsToTest { get; set; }

        public ObservableCollection<QuestionModel> Questions { get; }

        public XDocument ToXML(string documentName, string questionName, string answerName)
        {
            var xdoc = new XDocument();

            var test = new XElement(documentName);
            var testNameAttr = new XAttribute("Name", TestName);
            var testTimeAttr = new XAttribute("Time", MinutsToTest);
            var testSkipableAttr = new XAttribute("Skipable", Skipable);
            test.Add(testNameAttr);
            test.Add(testTimeAttr);
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

        public static TestModel FromXMl(XDocument xdoc, string documentName, string questionName, string answerName)
        {
            var result = new TestModel();

            var testElement = xdoc.Element(documentName);
            if(testElement == null)
                throw new TestException("Фаил испорчен: невозможно прочитать заголовок");

            var nameAttr = testElement.Attribute("Name");
            var dateAttr = testElement.Attribute("Date");
            var timeAttr = testElement.Attribute("Time");
            var skipableAttr = testElement.Attribute("Skipable");
            var authorAttr = testElement.Attribute("Author");

            result.TestName = nameAttr?.Value ?? throw new TestException("Фаил испорчен: невозможно прочитать название теста");

            if (timeAttr == null)
                throw new TestException("Фаил испорчен: невозможно определить время на прохождение теста");
            result.MinutsToTest = int.Parse(timeAttr.Value);

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

            foreach (var questionElemnt in xquestions)
            {
                var question = QuestionModel.FromXML(questionElemnt, answerName);
                result.Questions.Add(question);
            }

            return result;
        }
    }
}
