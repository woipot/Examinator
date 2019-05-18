using System;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    class TestModel : BindableBase
    {
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
    }
}
