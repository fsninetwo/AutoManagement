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
    /// Interaction logic for UserReportView.xaml
    /// </summary>
    public partial class UserReportView : UserControl
    {
        public List<string> Fields { get; set; }

        readonly Context db = new Context();
        public UserReportView()
        {
            InitializeComponent();
            Initialize();
        }

        public void CleanFields()
        {
            UserReportTable.SelectedItem = null;
            NameUser.Text = "";
            Adres.Text = "";
            Birthday.SelectedDate = null;
            Dolzh.Text = "";
            Oklad.Text = "";
            Priem.SelectedDate = null;
            NPrikazPriem.Text = "";
            Uvol.SelectedDate = null;
            NPrikazUvol.Text = "";
        }

        public void Initialize()
        {
            CleanFields();
            Fields = new List<string> { "ID", "Пользователь", "Пароль", "Адрес", "Дата Рождения", "Должность",
                "Оклад", "Дата приема", "Приказ о приеме", "Дата увольнения", "Приказ об увольнении" };
            UserReportTable.ItemsSource = Fields;
        }

        private bool CheckFields()
        {
            if (!NameUser.Text.Equals("") && !Regex.IsMatch(NameUser.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле пользователя. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Adres.Text.Equals("") && !Regex.IsMatch(Adres.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле вида адреса. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Birthday.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("День рождения не может быть позже текущей даты. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Dolzh.Text.Equals("") && !Regex.IsMatch(Dolzh.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле должности. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Oklad.Text.Equals("") && !Regex.IsMatch(Oklad.Text, ("\\d+")))
            {
                MessageBox.Show("Неверные данные в поле оклада. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!NPrikazPriem.Text.Equals("") && !Regex.IsMatch(NPrikazPriem.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле приказа о приеме. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!NPrikazUvol.Text.Equals("") && !Regex.IsMatch(NPrikazUvol.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле приказа об увольнении. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (UserReportTable.SelectedItems.Count != 0 && CheckFields())
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

            var avtos = db.Users.Where(e => e.NameUser.Contains(NameUser.Text) && e.Adres.Contains(Adres.Text) 
            && (e.Birthday == Birthday.SelectedDate || Birthday.SelectedDate == null) && e.Dolzh.Contains(Dolzh.Text)
            && (Oklad.Text.Equals("") || e.Oklad <= Convert.ToDecimal(Oklad.Text)) && (e.Priem == Priem.SelectedDate || Priem.SelectedDate == null)
            && e.NPrikazPriem.Contains(NPrikazPriem.Text) && (e.Uvol == Uvol.SelectedDate || Uvol.SelectedDate == null)
            && e.NPrikazUvol.Contains(NPrikazUvol.Text));

            var index = 1;
            foreach (var item in UserReportTable.SelectedItems)
            {
                var recordIndex = 2;
                switch (item.ToString())
                {

                    case "ID":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.IdUser))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Пользователь":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.NameUser))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Пароль":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Password))
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
                    case "Дата рождения":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Birthday))
                        {
                            workSheet.Cells[recordIndex, index].Value = id.Value.Day + "/" + id.Value.Month + "/" + id.Value.Year;
                            recordIndex++;
                        }
                        break;
                    case "Должность":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Dolzh))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Оклад":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Oklad))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Дата приема":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Priem))
                        {
                            workSheet.Cells[recordIndex, index].Value = id.Value.Day + "/" + id.Value.Month + "/" + id.Value.Year;
                            recordIndex++;
                        }
                        break;
                    case "Приказ о приеме":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.NPrikazPriem))
                        {
                            workSheet.Cells[recordIndex, index].Value = id;
                            recordIndex++;
                        }
                        break;
                    case "Дата увольнения":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.Uvol))
                        {
                            workSheet.Cells[recordIndex, index].Value = id.Value.Day + "/" + id.Value.Month + "/" + id.Value.Year;
                            recordIndex++;
                        }
                        break;
                    case "Приказ об увольнении":
                        workSheet.Cells[1, index].Value = item;
                        foreach (var id in avtos.Select(e => e.NPrikazUvol))
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
            UserReportTable.SelectedItem = null;
        }
    }
}
