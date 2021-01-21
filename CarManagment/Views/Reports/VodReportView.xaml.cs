using CarManagment.DB;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Interaction logic for VodReportView.xaml
    /// </summary>
    public partial class VodReportView : UserControl
    {
        public List<string> Fields { get; set; }

        readonly Context db = new Context();
        public VodReportView()
        {
            InitializeComponent();
            Initialize();
        }

        public void CleanFields()
        {
            VodReportTable.SelectedItem = null;
            F.Text = "";
            I.Text = "";
            O.Text = "";
            Klass.Text = "";
            Stazh.Text = "";
        }

        public void Initialize()
        {
            CleanFields();
            Fields = new List<string> { "ID", "Фамилия", "Имя", "Отчество", "Класс", "Стаж",};
            VodReportTable.ItemsSource = Fields;
        }

        private bool CheckFields()
        {
            if (!F.Text.Equals("") && !Regex.IsMatch(F.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле фамилии. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!I.Text.Equals("") && !Regex.IsMatch(I.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле вида имени. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!O.Text.Equals("") && !Regex.IsMatch(O.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле отчества. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Klass.Text.Equals("") && !Regex.IsMatch(Klass.Text, ("\\d+")))
            {
                MessageBox.Show("Неверные данные в поле класса. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Stazh.Text.Equals("") && !Regex.IsMatch(Stazh.Text, ("\\d")))
            {
                MessageBox.Show("Неверные данные в поле стажа. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (VodReportTable.SelectedItems.Count != 0 && CheckFields())
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

            var avtos = db.Vods.Where(e => e.F.Contains(F.Text) && e.I.Contains(I.Text) && e.O.Contains(O.Text)
            && (Klass.Text.Equals("") || e.Klass == Convert.ToInt32(Klass.Text))
            && (Stazh.Text.Equals("") || e.Stazh <= Convert.ToInt32(Stazh.Text)));

            var index = 1;
            foreach (var item in VodReportTable.SelectedItems)
            {
                var recordIndex = 2;
                switch (item.ToString())
                {

                    case "ID":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.IdVod))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Фамилия":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.F))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Имя":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.I))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Отчество":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.O))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Класс":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Klass))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Стаж":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Stazh))
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
            VodReportTable.SelectedItem = null;
        }
    }
}
