using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    class TestModel : BindableBase
    {
        public static string DeffaultBlockName = "Test";

        public TestModel()
        {
            Questions = new ObservableCollection<QuestionModel>();
        }

        public TestModel(DateTime createdDate)
        {
            Questions = new ObservableCollection<QuestionModel>();
            CreatedDate = createdDate;
        }

        public bool Skipable { get; set; } = true;

        public string TestName { get; set; }

        public string Author { get; set; }

        public DateTime CreatedDate { get; set; }

        public int MinutsToTest { get; set; }

        public ObservableCollection<QuestionModel> Questions { get; }

        public XDocument ToXML(string documentName)
        {
            var xdoc = new XDocument();

            var test = new XElement(documentName);
            var testNameAttr = new XAttribute("Name", TestName);
            var testDateAttr = new XAttribute("Date", CreatedDate);
            var testAuthorAttr = new XAttribute("Author", Author);
            var testTimeAttr = new XAttribute("Time", MinutsToTest);
            var testSkipableAttr = new XAttribute("Skipable", Skipable);
            test.Add(testNameAttr);
            test.Add(testDateAttr);
            test.Add(testAuthorAttr);
            test.Add(testTimeAttr);
            test.Add(testSkipableAttr);

            foreach (var questionModel in Questions)
            {
                var question = questionModel.ToXML(QuestionModel.DeffaultBlockName, Answer.DeffaultBlockName);
                test.Add(question);
            }

            xdoc.Add(test);

            return xdoc;
        }

    }
}
