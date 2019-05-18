using DevExpress.Mvvm;

namespace Examinator.mvvm.models.subModels
{
    public class PreloadedTestInfo : BindableBase
    {
        public string TestName { get; set; }

        public string AssociatedPath { get; set; }

        public PreloadedTestInfo(string testName, string associatedPath)
        {
            TestName = testName;
            AssociatedPath = associatedPath;
        }

        public override string ToString()
        {
            return TestName;
        }
    }
}
