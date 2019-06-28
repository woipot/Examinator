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
    public class Loader : BindableBase
    {
        public ObservableCollection<PreloadedTestInfo> PreloadedTests { get; }

        public List<TestException> LoadExceptions { get; }

        private const string TestDirectoryName = "Tests";
        private const string ResultDirectoryName = "Results";
        private static string DeffaultPass = "19voenkr";

        private readonly string _baseDir;

        public string PathToTests => _baseDir + $"{TestDirectoryName}";
        public string PathToResults => _baseDir + $"{ResultDirectoryName}";
        public string PathToMarkFile => PathToResults + $"//{MarkClass.DeffautFileName}";

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

        public static TestModel LoadFromFile(string filename)
        {
            StreamReader sr = new StreamReader(filename, Encoding.Default);
            var test = new TestModel();
            bool questions_count_setted = false;
            bool questions_time_setted = false;
            
            string line;
            QuestionModel tmpuestion = null;
            while ((line = sr.ReadLine()) != null)
            {
                //var line_copy = "awdfesgrd";
                //var kek = line_copy.Split('=');

                line = line.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                var first = line.First();
                switch(first)
                {
                    case '-':
                    case '+':
                        if(tmpuestion == null)
                        {
                            tmpuestion = new QuestionModel("");
                            test.Questions.Add(tmpuestion);
                        }
                                                
                        if (first == '+')
                        {
                            line = line.Remove(0, 1);
                            line = line.Trim();
                            tmpuestion.Answers.Add(new AnswerModel(line, true));
                        }
                        else
                        {
                            line = line.Remove(0, 1);
                            line = line.Trim();
                            tmpuestion.Answers.Add(new AnswerModel(line, false));
                        }
                        break;
                    case '=':
                        line = line.Remove(0, 1);
                        line = line.Trim();

                        tmpuestion = new QuestionModel(line);
                        test.Questions.Add(tmpuestion);
                        break;
                    default:
                        var line_split = line.Split('=');
                        if (line_split.Length == 2)
                        {
                            var variant = line_split[0].Trim().ToLower();
                            var data = line_split[1].Trim();
                            if (!string.IsNullOrEmpty(data))
                            {
                                switch (variant)
                                {
                                case "название":
                                case "testname":
                                    test.TestName = data;
                                    break;
                                case "автор":
                                case "author":
                                    test.Author = data;
                                    break;
                                case "время на вопрос":
                                case "time":
                                    try
                                    {
                                        var time = Int32.Parse(data);
                                        test.MinutsToTest = time;
                                        questions_time_setted = true; 
                                    } catch (FormatException) {
                                        test.MinutsToTest = 0;
                                        questions_time_setted = true;
                                    }
                                    break;
                                case "вопросы":
                                case "questioncount":
                                    try
                                    {
                                        var count = Int32.Parse(data);
                                        test.QuestionsInTest = count;
                                        questions_count_setted = true;
                                    }
                                    catch (FormatException) {
                                        test.QuestionsInTest = 0;
                                        questions_count_setted = true;
                                    }
                                    break;
                                case "можно пропускать":
                                case "skipable":
                                    if (data.Contains("Не") || data.Contains("False"))
                                    {
                                        test.Skipable = false;
                                    }
                                    else if (data.Contains("Да") || data.Contains("True"))
                                    {
                                        test.Skipable = true;
                                    }
                                    else
                                    {
                                        test.Skipable = true;
                                    }
                                    //test.Skipable = 
                                    break;
                                default:
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            if (!questions_count_setted)
                test.QuestionsInTest = 0;
            if (!questions_time_setted)
                test.MinutsToTest = 0;

            var check = test.CheckToCorrect();
            if(check.Item2)
            {
                throw new TestException("В файле обнаружены критические ошибки", check.Item1);
            }

            test.Clean();

            return test;
            
        } 

        private void RecreateStructure()
        {
            Directory.CreateDirectory(PathToTests);
            Directory.CreateDirectory(PathToResults);

            if (File.Exists(PathToMarkFile))
            {
                // Проверить есть ли в нем что-то, если нет заполнить
                try
                {
                    LoadMark(PathToMarkFile);
                } catch (TestException ex)
                {
                    LoadExceptions.Add(ex);
                }
                // StreamReader sr = new StreamReader(PathToMarkFile, Encoding.Default);

            } else
            {
                // Создать и заполнить
                SaveMark(new MarkClass(), PathToMarkFile);
            }
            
        }

        private bool StructureIsReady()
        {
            var isTestsFolderExist = Directory.Exists(PathToTests);
            var isResultsFolderExist = Directory.Exists(PathToResults);

            var isMarkEx = File.Exists(PathToMarkFile);

            return isTestsFolderExist && isResultsFolderExist && isMarkEx;
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
                catch (Exception ex)
                {
                    errorsList.Add(new TestException("Файл поврежден", file.Name));
                }
            }

            return errorsList;
        }

        static string GetTestName(string path)
        {
            var test = LoadTest(path, true);
            return test.TestName;
        }

        public static MarkClass LoadMark(string path)
        {
            var text = DecryptFile(path);
            var xdoc = XDocument.Parse(text);

            return MarkClass.fromXML(xdoc, MarkClass.DeffautBlockName);
        }

        public static void SaveMark(MarkClass marks, string path)
        {
            var marksXdox = MarkClass.toXML(marks);

            EncryptToFile(marksXdox.ToString(), path);
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
                bytes.Add((byte) data);

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
