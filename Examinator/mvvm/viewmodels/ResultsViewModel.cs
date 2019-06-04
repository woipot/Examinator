using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using DevExpress.Mvvm;
using Examinator.mvvm.models;

namespace Examinator.mvvm.viewmodels
{
    class ResultsViewModel : BindableBase
    {
       public String Result => $"{PercentResult}\n{TotalResult}";

        public String PercentResult =>
            $"{ResultModel.CorrectAnswersCount / (double) ResultModel.QuestionsCount * 100:N2}%";

        public String TotalResult => $"{ResultModel.CorrectAnswersCount}/{ResultModel.QuestionsCount}";

        public String TotalTime => (ResultModel.FinishTime - ResultModel.StartTime).ToString(@"mm") + " мин. " +
                                   (ResultModel.FinishTime - ResultModel.StartTime).ToString(@"ss") + " сек.";

        public ResultModel ResultModel { get; set; }

        public ObservableCollection<ResultModel> Results { get; set; }

        public ResultsViewModel()
        {
            ResultModel = new ResultModel();
            Results=new ObservableCollection<ResultModel>();
        }

        private void SaveResults()
        {
            using (var stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Results/results.db",
                FileMode.Create, FileAccess.Write))
            {
                var cryptic = new DESCryptoServiceProvider
                {
                    Key = Encoding.ASCII.GetBytes("Forichok"),
                    IV = Encoding.ASCII.GetBytes("Forichok")
                };


                using (var crStream = new CryptoStream(stream,
                    cryptic.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    var formatter = new BinaryFormatter();

                    formatter.Serialize(crStream, Results);
                }

            }
        }

        public void LoadResults()
        {
            try
            {
                using (var stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Results/results.db",
                    FileMode.OpenOrCreate, FileAccess.Read))
                {
                    var cryptic = new DESCryptoServiceProvider();

                    cryptic.Key = Encoding.ASCII.GetBytes("Forichok");
                    cryptic.IV = Encoding.ASCII.GetBytes("Forichok");

                    using (var crStream = new CryptoStream(stream,
                        cryptic.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        var formatter = new BinaryFormatter();

                        var t = (IEnumerable<ResultModel>)formatter.Deserialize(crStream);
                        Results = new ObservableCollection<ResultModel>(t);
                    }
                }
            }
            catch (Exception e)
            {

            }

            Results.Add(ResultModel);
            SaveResults();
        }
    }
}