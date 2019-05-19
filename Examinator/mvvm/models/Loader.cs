using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
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
        private static string DeffaultPass = "19voenkr";


        private readonly string _baseDir;

        private string PathToTests => _baseDir + $"{TestDirectoryName}";
        private string PathToResults => _baseDir + $"{ResultDirectoryName}";

        public Loader()
        {
            _baseDir = AppDomain.CurrentDomain.BaseDirectory;

            //var test = new TestModel(DateTime.Now);
            //test.TestName = "cats are best";
            //test.Author = "ivanov ivan";
            //test.MinutsToTest = 1;
            //var t = new QuestionModel("Кто такой мяуске");
            //t.Answers.Add(new Answer("кот", true));
            //t.Answers.Add(new Answer("медведь", false));
            //t.Answers.Add(new Answer("лошадь", false));
            //test.Questions.Add(t);
            //var r = new QuestionModel("Почему мяуске");
            //r.Answers.Add(new Answer("мясо", false));
            //r.Answers.Add(new Answer("шерсть", true));
            //r.Answers.Add(new Answer("ценная пушнина", false));
            //test.Questions.Add(r);
            //SaveTest(PathToTests + "\\tessssss.xml", test);
            //var res = LoadTest(PathToTests + "\\tessssss.xml");


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

        static TestModel LoadTest(string path)
        {
            var text = DecryptFile(path);
            var xdoc = XDocument.Parse(text);

            return TestModel.FromXMl(xdoc, TestModel.DeffaultBlockName, QuestionModel.DeffaultBlockName, Answer.DeffaultBlockName);
        }

        static void SaveTest(string path, TestModel model)
        {
            var xdoc = model.ToXML(TestModel.DeffaultBlockName, QuestionModel.DeffaultBlockName, Answer.DeffaultBlockName);

            EncryptToFile(xdoc.ToString(), path);
            
            //xdoc.Save(path);
        }

        private static void EncryptToFile(string input, string outputFile)
        {
            var ue = new UnicodeEncoding();
            var key = ue.GetBytes(DeffaultPass);

            var cryptFile = outputFile;
            var fsCrypt = new FileStream(cryptFile, FileMode.Create);

            var rmCrypto = new RijndaelManaged();

            var cs = new CryptoStream(fsCrypt,
                rmCrypto.CreateEncryptor(key, key),
                CryptoStreamMode.Write);

            var m = new MemoryStream(Encoding.Default.GetBytes(input));
            int data;
            while ((data = m.ReadByte()) != -1)
                cs.WriteByte((byte)data);


            m.Close();
            cs.Close();
            fsCrypt.Close();
        }

        private static string DecryptFile(string inputFile)
        {
            var ue = new UnicodeEncoding();
            var key = ue.GetBytes(DeffaultPass);

            var fsCrypt = new FileStream(inputFile, FileMode.Open);

            var rmCrypto = new RijndaelManaged();

            var cs = new CryptoStream(fsCrypt,
                rmCrypto.CreateDecryptor(key, key),
                CryptoStreamMode.Read);

            var bytes = new List<byte>(); 

            int data;
            while ((data = cs.ReadByte()) != -1)
                bytes.Add((byte)data);

            cs.Close();
            fsCrypt.Close();

            return Encoding.Default.GetString(bytes.ToArray());
        }
    }
}
