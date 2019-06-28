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

        ObservableCollection<ResultModel> _results = new ObservableCollection<ResultModel>();

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
                        _results = new ObservableCollection<ResultModel>(t);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Непредвиденная ошибка: невозможно открыть файл с результатами либо он пуст");
            }

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ClearResults();
        }

        private void ClearResults()
        {
            try
            {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/Results/results.db", string.Empty);
                _results.Clear();
            }
            catch (Exception у)
            {
                MessageBox.Show("Непредвиденная ошибка: Невозможно очистить файл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
