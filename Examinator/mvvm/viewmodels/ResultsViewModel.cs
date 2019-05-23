using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.Mvvm;
using Examinator.other;

namespace Examinator.mvvm.viewmodels
{
    class ResultsViewModel : BindableBase
    {
       public String Result => $"{PercentResult}\n{TotalResult}";

        public String PercentResult => string.Format("{0:N2}%", ResultModel.CorrectAnswersCount / (double)ResultModel.QuestionsCount * 100);

        public String TotalResult => $"{ResultModel.CorrectAnswersCount}/{ResultModel.QuestionsCount}";

        public String TotalTime => (ResultModel.FinishTime - ResultModel.StartTime).ToString(@"mm") + " мин. " +
                                   (ResultModel.FinishTime - ResultModel.StartTime).ToString(@"ss") + " сек.";

        public ResultModel ResultModel { get; set; }

        public ObservableCollection<ResultModel> Results { get; set; }

        public ResultsViewModel()
        {
            ResultModel = new ResultModel();
            Results=new ObservableCollection<ResultModel>();
        //    LoadResults();
        }

        private void SaveResults()
        {
            using (FileStream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/results.axax",
                FileMode.Create, FileAccess.Write))
            {
                DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();

                cryptic.Key = ASCIIEncoding.ASCII.GetBytes("Forichok");
                cryptic.IV = ASCIIEncoding.ASCII.GetBytes("Forichok");

                using (CryptoStream crStream = new CryptoStream(stream,
                    cryptic.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    
                    formatter.Serialize(crStream, Results);
                }

            }
        }

        public void LoadResults()
        {
            try
            {
                using (FileStream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/results.axax",
                    FileMode.OpenOrCreate, FileAccess.Read))
                {
                    DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();

                    cryptic.Key = ASCIIEncoding.ASCII.GetBytes("Forichok");
                    cryptic.IV = ASCIIEncoding.ASCII.GetBytes("Forichok");

                    using (CryptoStream crStream = new CryptoStream(stream,
                        cryptic.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

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