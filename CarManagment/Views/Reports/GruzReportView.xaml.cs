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
    /// Interaction logic for GruzReportView.xaml
    /// </summary>
    public partial class GruzReportView : UserControl
    {
        public List<string> Fields { get; set; }

        readonly Context db = new Context();
        public GruzReportView()
        {
            InitializeComponent();
            Initialize();
        }

        public void CleanFields()
        {
            GruzReportTable.SelectedItem = null;
            GruzName.Text = "";
            VidGruz.Text = "";
            Stoim.Text = "";
        }

        public void Initialize()
        {
            CleanFields();
            Fields = new List<string> { "ID", "Название", "Вид Груза", "Стоимость" };
            GruzReportTable.ItemsSource = Fields;
        }

        private bool CheckFields()
        {
            if (!GruzName.Text.Equals("") && !Regex.IsMatch(GruzName.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле названия. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }        
            if (!VidGruz.Text.Equals("") && !Regex.IsMatch(VidGruz.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле вида груза. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Stoim.Text.Equals("") && !Regex.IsMatch(Stoim.Text, ("\\d+")))
            {
                MessageBox.Show("Неверные данные в поле стоимости. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (GruzReportTable.SelectedItems.Count != 0 && CheckFields())
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

            var avtos = from gruz in db.Gruzs
                        join vidgruz in db.VidGruzs on gruz.IdVidGruz equals vidgruz.IdVidGruz
                        where gruz.NameGruz.Contains(GruzName.Text) && vidgruz.NameVidGruz.Contains(VidGruz.Text) 
                        && (Stoim.Text.Equals("") || gruz.Stoim <= Convert.ToDouble(Stoim.Text))
                        select new GruzCase
                        {
                            IdGruz = gruz.IdGruz,
                            NameGruz = gruz.NameGruz,
                            VidGruz = vidgruz.NameVidGruz,
                            Stoim = gruz.Stoim
                        };

            var index = 1;
            foreach (var item in GruzReportTable.SelectedItems)
            {
                var recordIndex = 2;
                switch (item.ToString())
                {

                    case "ID":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.IdGruz))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Название":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.NameGruz))
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
                    case "Стоимость":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Stoim))
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
            GruzReportTable.SelectedItem = null;
        }
    }
}