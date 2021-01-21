using CarManagment.Cache;
using CarManagment.DB;
using CarManagment.DB.Tables.DataGridCase;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarManagment.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Context db = new Context();
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            if (AvtoTab.IsSelected) AvtoView.Search.Text = Search.Text;
            else if (GruzTab.IsSelected) GruzView.Search.Text = Search.Text;
            else if (KlientTab.IsSelected) KlientView.Search.Text = Search.Text;
            else if (UsersTab.IsSelected) UserView.Search.Text = Search.Text;
            else if (VodAvtoTab.IsSelected) VodAvtoView.Search.Text = Search.Text;
            else if (VodTab.IsSelected) VodView.Search.Text = Search.Text;
            else if (ZakazTab.IsSelected) ZakazView.Search.Text = Search.Text;
            else if (LogTab.IsSelected) LogView.Initialize();
        }

        public void Initialize(long Id)
        {
            var user = db.Users.Where(e => e.IdUser == Id).Single();
            ActiveUser.IDUser = user.IdUser;
            ActiveUser.NameUser = user.NameUser;
            LogTab.Visibility = Visibility.Hidden;
            Administration();
        }
        private void Administration()
        {
            if(!ActiveUser.NameUser.ToLower().Equals("администратор"))
            {
                UsersTab.Visibility = Visibility.Hidden;
                LogMenu.Visibility = Visibility.Hidden;
            }
        }
        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            if (Logs.IsChecked == true)
            {
                LogTab.Visibility = Visibility.Hidden;
                Logs.IsChecked = false;
            }
            else
            {
                LogTab.Visibility = Visibility.Visible;
                Logs.IsChecked = true;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            ActiveUser.IDUser = 0;
            ActiveUser.NameUser = null;
            window.Show();
            Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Initialize();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Initialize();
        }

        private string SaveFile()
        {
            var result = new SaveFileDialog
            {
                Filter = "New ExcelFile (*.xlsx)|*.xlsx|Old ExcelFile (*.xls)|*.xls",
                DefaultExt = "xlsx"
            };
            result.ShowDialog();
            return result.FileName;
        }

        private void Gruz_Click(object sender, RoutedEventArgs e)
        {
            string path = SaveFile();
            if (path.Equals("")) return;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            var zakazs = from zakaz in db.Zakazs
                         join avto in db.Avtos on zakaz.IdAvto equals avto.IdAvto
                         join vod in db.Vods on zakaz.IdVod equals vod.IdVod
                         join gruz in db.Gruzs on zakaz.IdGruz equals gruz.IdGruz
                         join klient in db.Klients on zakaz.IdKlient equals klient.IdKlient
                         where zakaz.DateVypoln.Month == DateTime.Now.Month - 1
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
            var gruzs = zakazs.GroupBy(e => e.NameGruz).Select(e => new
            {
                Gruz = e.Key,
                Kol = e.Count(),
                Summa = e.Sum(e => e.Kol)
            }).OrderByDescending(e => e.Summa).ToList();

            workSheet.Cells[1, 1].Value = "Груз";
            workSheet.Cells[1, 2].Value = "Количество за месяц";
            var index = 2;
            foreach (var item in gruzs)
            {
                workSheet.Cells[index, 1].Value = item.Gruz;
                workSheet.Cells[index, 2].Value = item.Summa;
                index++;
            }

            if (File.Exists(path)) File.Delete(path);
            FileStream objFileStrm = File.Create(path);
            objFileStrm.Close();
            File.WriteAllBytes(path, excel.GetAsByteArray());
        }

        private void Vod_Click(object sender, RoutedEventArgs e)
        {
            string path = SaveFile();
            if (path.Equals("")) return;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            var zakazs = from zakaz in db.Zakazs
                         join avto in db.Avtos on zakaz.IdAvto equals avto.IdAvto
                         join vod in db.Vods on zakaz.IdVod equals vod.IdVod
                         join gruz in db.Gruzs on zakaz.IdGruz equals gruz.IdGruz
                         join klient in db.Klients on zakaz.IdKlient equals klient.IdKlient
                         where zakaz.DateVypoln.Month == DateTime.Now.Month - 1
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
            var gruzs = zakazs.GroupBy(e => e.FIOVod).Select(e => new
            {
                FIOVod = e.Key,
                Kol = e.Count()
            }).OrderByDescending(e => e.Kol).ToList();

            workSheet.Cells[1, 1].Value = "Водитель";
            workSheet.Cells[1, 2].Value = "Количество за месяц";
            var index = 2;
            foreach (var item in gruzs)
            {
                workSheet.Cells[index, 1].Value = item.FIOVod;
                workSheet.Cells[index, 2].Value = item.Kol;
                index++;
            }

            if (File.Exists(path)) File.Delete(path);
            FileStream objFileStrm = File.Create(path);
            objFileStrm.Close();
            File.WriteAllBytes(path, excel.GetAsByteArray());
        }

        private void Zakaz_Click(object sender, RoutedEventArgs e)
        {
            string path = SaveFile();
            if (path.Equals("")) return;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            var zakazs = from zakaz in db.Zakazs
                         join avto in db.Avtos on zakaz.IdAvto equals avto.IdAvto
                         join vod in db.Vods on zakaz.IdVod equals vod.IdVod
                         join gruz in db.Gruzs on zakaz.IdGruz equals gruz.IdGruz
                         join klient in db.Klients on zakaz.IdKlient equals klient.IdKlient
                         where zakaz.DateVypoln.Month == DateTime.Now.Month - 1
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
            var gruzs = zakazs.GroupBy(e => e.Kuda).Select(e => new
            {
                Kuda = e.Key,
                Kol = e.Count(),
            }).OrderByDescending(e => e.Kol).ToList();

            foreach (var item in gruzs) MessageBox.Show(item.Kuda + " " + item.Kol);

            workSheet.Cells[1, 1].Value = "Куда";
            workSheet.Cells[1, 2].Value = "Количество за месяц";
            var index = 2;
            foreach (var item in gruzs)
            {
                workSheet.Cells[index, 1].Value = item.Kol;
                workSheet.Cells[index, 2].Value = item.Kol;
                index++;
            }

            if (File.Exists(path)) File.Delete(path);
            FileStream objFileStrm = File.Create(path);
            objFileStrm.Close();
            File.WriteAllBytes(path, excel.GetAsByteArray());
        }
    }
}
