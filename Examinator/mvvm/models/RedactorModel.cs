using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Examinator.mvvm.models.subModels;

namespace Examinator.mvvm.models
{
    public class RedactorModel : BindableBase
    {
        private TestModel _testModel;
        private PreloadedTestInfo _info;

        public ObservableCollection<QuestionModel> Questions => _testModel?.Questions;

        public void SetData(TestModel testModel, PreloadedTestInfo preloadedInfo)
        {
            _testModel = testModel;
            _info = preloadedInfo;
            RaisePropertiesChanged("Questions");
        }


    }
}
