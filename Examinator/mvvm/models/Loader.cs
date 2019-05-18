using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DevExpress.Mvvm;
using Examinator.mvvm.models.subModels;
using Examinator.other;

namespace Examinator.mvvm.models
{
    class Loader : BindableBase
    {
        public ObservableCollection<PreloadedTestInfo> PreloadedTests { get; }

        public List<TestException> LoadExceptions { get; }

        private const string TestDirectoryName = "Tests";
        private const string ResultDirectoryName = "Results";

        private readonly string _baseDir;

        private string PathToTests => _baseDir + $"{TestDirectoryName}";
        private string PathToResults => _baseDir + $"{ResultDirectoryName}";

        public Loader()
        {
            _baseDir = AppDomain.CurrentDomain.BaseDirectory;


            if (!StructureIsReady())
            {
                RecreateStructure();
            }

            PreloadedTests = new ObservableCollection<PreloadedTestInfo>();

            LoadExceptions = PreloadTests().ToList();
        }

        private void RecreateStructure()
        {
            Directory.CreateDirectory(PathToTests);
            Directory.CreateDirectory(PathToResults);
        }

        private bool StructureIsReady()
        {
            var isTestsFolderExist = Directory.Exists(PathToTests);
            var isResultsFolderExist = Directory.Exists(PathToResults);

            return isTestsFolderExist && isResultsFolderExist;
        }

        private IEnumerable<TestException> PreloadTests()
        {
            var errorsList = new List<TestException>();
            var files = Directory.GetFiles(PathToTests);
            foreach (var file in files)
            {
                try
                {
                    var testName = GetTestName(file);

                    PreloadedTests.Add(new PreloadedTestInfo(testName, file));
                }
                catch (TestException e)
                {
                    errorsList.Add(e);
                }
            }

            return errorsList;
        }

        static string GetTestName(string path)
        {
            string name;
            using (var sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                name = sr.ReadLine();
            }

            if (string.IsNullOrEmpty(name))
                throw new TestException("Файл поврежден : Невозможно прочитать название теста", path);

            return name;
        }

        static TestModel GetTest(string path)
        {
            var test = new TestModel();



            return test;
        }
    }
}
