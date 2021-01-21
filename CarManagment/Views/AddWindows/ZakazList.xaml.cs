using CarManagment.DB.Tables.DataGridCase;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CarManagment.Views.AddWindows
{
    /// <summary>
    /// Interaction logic for ZakazList.xaml
    /// </summary>
    public partial class ZakazList : Window
    {
        public ZakazList()
        {
            InitializeComponent();
        }

        public void Initialize(ZakazCase entity)
        {
            TextBox.AppendText("Номер заказа: " + entity.IdZakaz + "\n");
            TextBox.AppendText("Дата заказа: " + entity.DateZakaz + "\n");
            TextBox.AppendText("Вид Груза: " + entity.NameGruz + "\n");
            TextBox.AppendText("Откуда: " + entity.Otkuda + "\n");
            TextBox.AppendText("Куда: " + entity.Kuda + "\n");
            TextBox.AppendText("Дата выполнения: " + entity.DateVypoln + "\n");
            TextBox.AppendText("Автомобиль: " + entity.Marka + "\n");
            TextBox.AppendText("Водитель: " + entity.FIOVod + "\n");
            TextBox.AppendText("Клиент: " + entity.FIOKlient + "\n");
            TextBox.AppendText("Количество: " + entity.Kol + "\n");
            TextBox.AppendText("Сумма: " + entity.Summa + "\n");
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {
                pd.PrintVisual(TextBox as Visual, "printing as visual");
                pd.PrintDocument((((IDocumentPaginatorSource)TextBox.Document).DocumentPaginator), "printing as paginator");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var result = new SaveFileDialog
            {
                Filter = "RichTextFormat (*.rtf)|*.rtf",
                DefaultExt = "rtf"
            };
            result.ShowDialog();
            if (!result.FileName.Equals("")) SaveWordFile(result.FileName);
        }

        private void SaveWordFile(string path)
        {
            TextRange range = new TextRange(TextBox.Document.ContentStart, TextBox.Document.ContentEnd);
            FileStream fStream = new FileStream(path, FileMode.Create);
            range.Save(fStream, DataFormats.Rtf);
            fStream.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
