using CarManagment.DB;
using CarManagment.DB.Tables.DataGridCase;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Interaction logic for KlientReportView.xaml
    /// </summary>
    public partial class KlientReportView : UserControl
    {
        public List<string> Fields { get; set; }

        readonly Context db = new Context();
        public KlientReportView()
        {
            InitializeComponent();
            Initialize();
        }

        public void CleanFields()
        {
            KlientReportTable.SelectedItem = null;
            FIO.Text = "";
            Adres.Text = "";
            Telefon.Text = "";
        }

        public void Initialize()
        {
            CleanFields();
            Fields = new List<string> { "ID", "ФИО", "Адрес", "Телефон" };
            KlientReportTable.ItemsSource = Fields;
        }

        private bool CheckFields()
        {
            if (!FIO.Text.Equals("") && !Regex.IsMatch(FIO.Text, @"^\w+ \w+ \w+$"))
            {
                MessageBox.Show("Неверные данные в поле ФИО. Введите полное ФИО!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Adres.Text.Equals("") && !Regex.IsMatch(Adres.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле вида адреса. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Telefon.Text.Equals("") && !Regex.IsMatch(Telefon.Text, @"\+\d+"))
            {
                MessageBox.Show("Неверные данные в поле телефона. Введите полный номер телефона!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (KlientReportTable.SelectedItems.Count != 0 && CheckFields())
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

            var avtos = db.Klients.Where(e => e.FIO.Contains(FIO.Text) && e.Adres.Contains(Adres.Text) && e.Telefon.Contains(Telefon.Text));

            var index = 1;
            foreach (var item in KlientReportTable.SelectedItems)
            {
                var recordIndex = 2;
                switch (item.ToString())
                {

                    case "ID":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.IdKlient))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "ФИО":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.FIO))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Адрес":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Adres))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Телефон":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Telefon))
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
            KlientReportTable.SelectedItem = null;
        }
    }
}
