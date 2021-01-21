using CarManagment.DB;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarManagment.DB.Tables.DataGridCase;

namespace CarManagment.Views.Reports
{
    /// <summary>
    /// Interaction logic for ZakazReportView.xaml
    /// </summary>
    public partial class ZakazReportView : UserControl
    {
        public List<string> Fields { get; set; }

        readonly Context db = new Context();
        public ZakazReportView()
        {
            InitializeComponent();
            Initialize();
        }

        public void CleanFields()
        {
            ZakazReportTable.SelectedItem = null;
            DateZakaz.SelectedDate = null;
            Otkuda.Text = "";
            Kuda.Text = "";
            DateVypoln.SelectedDate = null;
            FIOVod.Text = "";
            Marka.Text = "";
            FIOKlient.Text = "";
            Kol.Text = "";
            Sum.Text = "";
        }

        public void Initialize()
        {
            CleanFields();
            Fields = new List<string> { "ID", "Дата заказа", "Вид груза", "Откуда", "Куда", "Дата выполнения", "Водитель", "Автомобиль",
                "Клиент", "Количество", "Сумма"};
            ZakazReportTable.ItemsSource = Fields;
        }

        private bool CheckFields()
        {
            if (!NameGruz.Text.Equals("") && !Regex.IsMatch(NameGruz.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Otkuda.Text.Equals("") && !Regex.IsMatch(Otkuda.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Kuda.Text.Equals("") && !Regex.IsMatch(Kuda.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле водителя. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!FIOVod.Text.Equals("") && !Regex.IsMatch(FIOVod.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Marka.Text.Equals("") && !Regex.IsMatch(Marka.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!FIOKlient.Text.Equals("") && !Regex.IsMatch(FIOKlient.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле водителя. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Kol.Text.Equals("") && !Regex.IsMatch(Kol.Text, ("\\d+")))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Sum.Text.Equals("") && !Regex.IsMatch(Sum.Text, ("\\d+")))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ZakazReportTable.SelectedItems.Count != 0 && CheckFields())
            {
                var result = new SaveFileDialog
                {
                    Filter = "New ExcelFile (*.xlsx)|*.xlsx|Old ExcelFile (*.xls)|*.xls",
                    DefaultExt = "xlsx"
                };
                result.ShowDialog();
                if (!result.FileName.Equals("")) SaveExcelFile(result.FileName);
            }
        }

        private void SaveExcelFile(string path)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            string[] vods;
            if (FIOVod.Equals("")) vods = FIOVod.Text.Split(" ");
            else vods = new string[] { "", "", "" };

            var zakazs = from zakaz in db.Zakazs
                       join avto in db.Avtos on zakaz.IdAvto equals avto.IdAvto
                       join vod in db.Vods on zakaz.IdVod equals vod.IdVod
                       join gruz in db.Gruzs on zakaz.IdGruz equals gruz.IdGruz
                       join klient in db.Klients on zakaz.IdKlient equals klient.IdKlient
                       where (DateZakaz.SelectedDate == null || DateZakaz.SelectedDate == zakaz.DateZakaz) && zakaz.Otkuda.Contains(Otkuda.Text)
                       && zakaz.Kuda.Contains(Kuda.Text) && (DateVypoln.SelectedDate == null || DateVypoln.SelectedDate == zakaz.DateVypoln)
                       && avto.Marka.Contains(Marka.Text) && vod.F.Contains(vods[0]) && vod.I.Contains(vods[1]) && vod.O.Contains(vods[2])
                       && klient.FIO.Contains(FIOKlient.Text) && (Kol.Text.Equals("") || zakaz.Kol >= Convert.ToDouble(Kol.Text))
                       && (Sum.Text.Equals("") || zakaz.Summa >= Convert.ToDouble(Sum.Text)) && gruz.NameGruz.Contains(NameGruz.Text)
                       select new ZakazCase
                       {
                           IdZakaz = zakaz.IdZakaz,
                           DateZakaz = zakaz.DateZakaz,
                           NameGruz = gruz.NameGruz,
                           Otkuda = zakaz.Otkuda,
                           Kuda = zakaz.Kuda,
                           DateVypoln = zakaz.DateVypoln,
                           Marka = avto.Marka,
                           FIOVod = vod.F + " " + vod.I + " " + vod.O,
                           FIOKlient = klient.FIO,
                           Kol = zakaz.Kol,
                           Summa = zakaz.Summa
                       };

            var index = 1;
            foreach (var item in ZakazReportTable.SelectedItems)
            {
                var recordIndex = 2;
                switch (item.ToString())
                {

                    case "ID":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.IdZakaz))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Дата заказа":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.DateZakaz))
                        {
                            workSheet.Cells[recordIndex, index].Value = id.Day + "/" + id.Month + "/" + id.Year;
                            recordIndex++;
                        }
                        break;
                    case "Вид груза":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.NameGruz))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Откуда":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.Otkuda))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Куда":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.Kuda))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Дата выполнения":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.DateVypoln))
                        {
                            workSheet.Cells[recordIndex, index].Value = id.Day +"/" + id.Month + "/" + id.Year;
                            recordIndex++;
                        }
                        break;
                    case "Водитель":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.FIOVod))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Автомобиль":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.Marka))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Клиент":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.IdZakaz))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Количество":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.Kol))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Сумма":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in zakazs.Select(e => e.Summa))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                }
                index++;
            }
            if (File.Exists(path)) File.Delete(path);
            FileStream objFileStrm = File.Create(path);
            objFileStrm.Close();
            File.WriteAllBytes(path, excel.GetAsByteArray());
            ZakazReportTable.SelectedItem = null;
        }
    }
}
