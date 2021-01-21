using CarManagment.DB;
using CarManagment.DB.Tables;
using CarManagment.DB.Tables.DataGridCase;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarManagment.Views.Reports
{
    /// <summary>
    /// Interaction logic for AvtoReportView.xaml
    /// </summary>
    public partial class AvtoReportView : UserControl
    {
        public List<string> Fields { get; set; }

        readonly Context db = new Context();
        public AvtoReportView()
        {
            InitializeComponent();
            Initialize();
        }

        public void CleanFields()
        {
            AvtoReportTable.SelectedItem = null;
            Marka.Text = "";
            Nomer.Text = "";
            GruzPod.Text = "";
            VidGruz.Text = "";
        }

        public void Initialize()
        {
            CleanFields();
            Fields = new List<string> { "ID", "Марка", "Номер", "Вид Груза", "Исправность" };
            AvtoReportTable.ItemsSource = Fields;
        }

        private bool CheckFields()
        {
            if (!Marka.Text.Equals("") && !Regex.IsMatch(Marka.Text, "\\w+"))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Nomer.Text.Equals("") && !Regex.IsMatch(Nomer.Text, "\\w{2}-\\d{4}"))
            {
                MessageBox.Show("Неверные данные в поле номера. Введите в виде \"AA-1111\"!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!GruzPod.Text.Equals("") && !Regex.IsMatch(GruzPod.Text, "\\d+"))
            {
                MessageBox.Show("Неверные данные в поле грузоподемности. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!VidGruz.Text.Equals("") && !Regex.IsMatch(VidGruz.Text, "\\w+"))
            {
                MessageBox.Show("Неверные данные в поле вида груза. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (AvtoReportTable.SelectedItems.Count != 0 && CheckFields())
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

            var avtos = from avto in db.Avtos
                        join vidgruz in db.VidGruzs on avto.IdVidGruz equals vidgruz.IdVidGruz
                        where avto.Marka.Contains(Marka.Text) && avto.Nomer.Contains(Nomer.Text) 
                        && (GruzPod.Text.Equals("") || avto.GruzPod <= Convert.ToDouble(GruzPod.Text))
                        && vidgruz.NameVidGruz.Contains(VidGruz.Text) || avto.Ispr == Ispr.IsChecked
                        select new AvtoCase
                        {
                            IdAvto = avto.IdAvto,
                            Nomer = avto.Nomer,
                            Marka = avto.Marka,
                            GruzPod = avto.GruzPod,
                            VidGruz = vidgruz.NameVidGruz,
                            Ispr = avto.Ispr ? "Исправен" : "Неиспрвен"
                        };

            var index = 1;
            foreach (var item in AvtoReportTable.SelectedItems)
            {
                var recordIndex = 2;
                switch (item.ToString())
                {
                    
                    case "ID":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.IdAvto))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Марка":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Marka))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Номер":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Nomer))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Вид Груза":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.VidGruz))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Исправность":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Ispr))
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
            AvtoReportTable.SelectedItem = null;
        }
    }
}
