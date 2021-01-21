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
    /// Interaction logic for VodAvtoReportView.xaml
    /// </summary>
    public partial class VodAvtoReportView : UserControl
    {
        public List<string> Fields { get; set; }

        readonly Context db = new Context();
        public VodAvtoReportView()
        {
            InitializeComponent();
            Initialize();
        }

        public void CleanFields()
        {
            VodAvtoReportTable.SelectedItem = null;
            Marka.Text = "";
            FIO.Text = "";
        }

        public void Initialize()
        {
            CleanFields();
            Fields = new List<string> { "ID", "Автомобиль", "Водитель"};
            VodAvtoReportTable.ItemsSource = Fields;
        }

        private bool CheckFields()
        {
            if (!Marka.Text.Equals("") && !Regex.IsMatch(Marka.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!FIO.Text.Equals("") && !Regex.IsMatch(FIO.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле водителя. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (VodAvtoReportTable.SelectedItems.Count != 0 && CheckFields())
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
            if (FIO.Equals("")) vods = FIO.Text.Split(" ");
            else vods = new string[] { "", "", "" };

            var avtos = from vodavto in db.VodAvtos
                        join avto in db.Avtos on vodavto.IdAvto equals avto.IdAvto
                        join vod in db.Vods on vodavto.IdVod equals vod.IdVod
                        join vidgruz in db.VidGruzs on avto.IdVidGruz equals vidgruz.IdVidGruz
                        where avto.Marka.Contains(Marka.Text) && vod.F.Contains(vods[0]) && vod.I.Contains(vods[1]) && vod.O.Contains(vods[2])
                        select new VodAvtoCase
                        {
                            IdVodAvto = avto.IdVidGruz,
                            FIO = vod.F + " " + vod.I +" " + vod.O,
                            Marka = avto.Marka + " \"" + vidgruz.NameVidGruz + "\"",
                        };

            var index = 1;
            foreach (var item in VodAvtoReportTable.SelectedItems)
            {
                var recordIndex = 2;
                switch (item.ToString())
                {

                    case "ID":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.IdVodAvto))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Водитель":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.FIO))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Автомобиль":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Marka))
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
            VodAvtoReportTable.SelectedItem = null;
        }
    }
}
