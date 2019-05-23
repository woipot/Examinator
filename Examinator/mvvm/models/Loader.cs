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
    internal class Loader : BindableBase
    {
        public ObservableCollection<PreloadedTestInfo> PreloadedTests { get; }

        public List<TestException> LoadExceptions { get; }

        private const string TestDirectoryName = "Tests";
        private const string ResultDirectoryName = "Results";
        private static string DeffaultPass = "19voenkr";


        private readonly string _baseDir;

        public string PathToTests => _baseDir + $"{TestDirectoryName}";
        public string PathToResults => _baseDir + $"{ResultDirectoryName}";

        public Loader()
        {
            _baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (!StructureIsReady())
            {
                RecreateStructure();
            }

            //place here code to test

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
            var info = new DirectoryInfo(PathToTests);
            var files = info.GetFiles().OrderBy(p=>p.CreationTime);
            foreach (var file in files)
            {
                try
                {
                    var testName = GetTestName(file.FullName);

                    PreloadedTests.Add(new PreloadedTestInfo(testName, file.FullName));
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
            var test = LoadTest(path, true);
            return test.TestName;
        }

        public static TestModel LoadTest(string path, bool loadOnlyHeader = false)
        {
            var text = DecryptFile(path);
            var xdoc = XDocument.Parse(text);

            return TestModel.FromXMl(xdoc, TestModel.DeffaultBlockName, QuestionModel.DeffaultBlockName, AnswerModel.DeffaultBlockName, !loadOnlyHeader);
        }

        public static void SaveTest(string path, TestModel model)
        {
            var xdoc = model.ToXML(TestModel.DeffaultBlockName, QuestionModel.DeffaultBlockName, AnswerModel.DeffaultBlockName);

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

        public static string SaveTest(TestModel testModel, string destinationFolder)
        {
            var path = FindWayToSave(testModel.TestName, destinationFolder);

            SaveTest(path, testModel);

            return path;
        }

        private static string FindWayToSave(string name, string destinationFolder, string extension = ".xml")
        {
            var dirinfo = new DirectoryInfo(destinationFolder);
            var files = dirinfo.GetFiles();

            var typedName = $"{name}_{DateTime.Now:MM_dd_yyyy}";
            

            var additinalPrefix = 0;
            string finalName;
            do
            {
                finalName = typedName + (additinalPrefix == 0 ? "" : $"_{additinalPrefix}") + extension;
                additinalPrefix++;

            } while (files.Any(s => string.Equals(s.Name, finalName, StringComparison.CurrentCultureIgnoreCase)));

            return $"{destinationFolder}\\{finalName}";

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

        public static void DeleteTest(string infoAssociatedPath)
        {
            File.Delete(infoAssociatedPath);
        }
    }
}
