using System;
using System.Windows;
using Examinator.mvvm.models;
using Examinator.other;

namespace Examinator.Views
{
    /// <summary>
    /// Interaction logic for MarksSettingsWindow.xaml
    /// </summary>
    public partial class MarksSettingsWindow : Window
    {
        private MarkClass _marks;

        private string _pathToMarks; 

        public MarksSettingsWindow(Loader loader)
        {
            InitializeComponent();

            var marks = new MarkClass();
            try
            {
                marks = Loader.LoadMark(loader.PathToMarkFile);
            }
            catch (TestException e)
            {
                MessageBox.Show("Ошибка: невозможно открыть файл\nБудут установленны значения по умолчанию");

            }

            FiveBlock.Value = marks.FivePercent;
            FourBlock.Value = marks.FourPercent;
            ThreeBlock.Value = marks.ThreePercent;

            _marks = marks;
            _pathToMarks = loader.PathToMarkFile;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (FiveBlock.Value == null || FourBlock.Value == null || ThreeBlock.Value == null)
            {
                MessageBox.Show("Ошибка: Отсутствует значение");
                return;
            }

            try
            {
                
                 _marks.SetAllParam((int) FiveBlock.Value, (int) FourBlock.Value, (int) ThreeBlock.Value);
            }
            catch (TestException exception)
            {
                MessageBox.Show($"Ошибка: {exception.Message}\nИсправьте и попробуйте снова");
            }

            try
            {
                Loader.SaveMark(_marks, _pathToMarks);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Непредвиденная ошибка: невозможно сохранить в файл");
            }

            MessageBox.Show("Сохранено!");

        }
    }
}
