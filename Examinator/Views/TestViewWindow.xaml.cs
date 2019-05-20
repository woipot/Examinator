using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using System.Xml;
using Examinator.mvvm.models.subModels;

namespace Examinator.Views
{
    /// <summary>
    /// Логика взаимодействия для TestViewWindow.xaml
    /// </summary>
    public partial class TestViewWindow : Window
    {
        private TestModel _testModel;

        public TestViewWindow(TestModel testmodel)
        {
            InitializeComponent();
            _testModel = testmodel;

            HeaderText.Text = _testModel.TestName;
            //var xml = _testModel.ToXML(TestModel.DeffaultBlockName, QuestionModel.DeffaultBlockName, AnswerModel.DeffaultBlockName);

            DocumentViewer.Document = ToFlowDocument(_testModel.ToString());

        }

        private static FlowDocument ToFlowDocument(string input)
        {
            var result = new FlowDocument();

            var p = new Paragraph(new Run(input))
            {
                FontSize = 14,
                FontStyle = FontStyles.Normal,
                TextAlignment = TextAlignment.Left
            };

            result.Blocks.Add(p);

            return result;
        }


    }
}
