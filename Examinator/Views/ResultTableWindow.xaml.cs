using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Examinator.mvvm.models;

namespace Examinator.Views
{
    /// <summary>
    /// Логика взаимодействия для ResultTableWindow.xaml
    /// </summary>
    public partial class ResultTableWindow : Window
    {

        List<ResultModel> _results = new List<ResultModel>();

        public ResultTableWindow()
        {
            InitializeComponent();
            LoadResults();
            DateGrid.ItemsSource = _results;
        }

        private void LoadResults()
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
                        _results = new List<ResultModel>(t);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Неапредвиденныая ошибка: невозможно открыть файл с результатами");
            }

        }
    }
}
