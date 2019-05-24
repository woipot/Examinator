using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Examinator.Views
{
    /// <summary>
    /// Логика взаимодействия для HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            try
            {
                var doc = new TextRange(TextBox.Document.ContentStart, TextBox.Document.ContentEnd);
                using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\Instruction.txt", FileMode.Open))
                {
                    doc.Load(fs, DataFormats.Text);

                }

            }
            catch (Exception exception)
            {
                MessageBox.Show("Ошибка открытия файла: Невозможно загрузит файл с инструкциями");
            }
            
        }
    }
}
