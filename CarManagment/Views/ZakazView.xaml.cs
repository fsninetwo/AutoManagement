using CarManagment.Cache;
using CarManagment.DB;
using CarManagment.DB.Tables;
using CarManagment.DB.Tables.DataGridCase;
using CarManagment.Views.AddWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ZakazView.xaml
    /// </summary>
    public partial class ZakazView : UserControl
    {
        private readonly Context db = new Context();
        private string SelectedUser;

        public ZakazView()
        {
            InitializeComponent();
            Initialize();
        }

        public void InitializeUser(long Id)
        {
            SelectedUser = db.Users.Where(e => e.IdUser == Id).Single().NameUser;
        }

        public void Initialize()
        {
            if (Search.Text.Equals("")) AddItems();
            else AddItemsBySearch();
        }

        public void AddItems()
        {
            var result = from zakaz in db.Zakazs
                         join avto in db.Avtos on zakaz.IdAvto equals avto.IdAvto
                         join vod in db.Vods on zakaz.IdVod equals vod.IdVod
                         join gruz in db.Gruzs on zakaz.IdGruz equals gruz.IdGruz
                         join klient in db.Klients on zakaz.IdKlient equals klient.IdKlient
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
            ZakazTable.ItemsSource = result.ToList();
        }

        public void AddItemsBySearch()
        {
            var result = from zakaz in db.Zakazs
                         join avto in db.Avtos on zakaz.IdAvto equals avto.IdAvto
                         join vod in db.Vods on zakaz.IdVod equals vod.IdVod
                         join gruz in db.Gruzs on zakaz.IdGruz equals gruz.IdGruz
                         join klient in db.Klients on zakaz.IdKlient equals klient.IdKlient
                         where avto.Marka.Contains(Search.Text) || avto.Nomer.Contains(Search.Text) ||
                         vod.F.Contains(Search.Text) || vod.I.Contains(Search.Text) || vod.O.Contains(Search.Text) ||
                         gruz.NameGruz.Contains(Search.Text) || klient.FIO.Contains(Search.Text)
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
            ZakazTable.ItemsSource = result.ToList();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            ZakazEditView.IsEnabled = true;
            ZakazEditView.Visibility = Visibility.Visible;
            ZakazEditView.Initialize();
            EditHeight.Height = new GridLength(350);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (ZakazTable.SelectedIndex >= 0)
            {
                ZakazCase Item = (dynamic)ZakazTable.SelectedItem;
                ZakazEditView.IsEnabled = true;
                ZakazEditView.Visibility = Visibility.Visible;
                ZakazEditView.Initialize(Item);
                EditHeight.Height = new GridLength(350);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить данные?", "Требуется подстверждение!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes && ZakazTable.SelectedIndex >= 0)
            {
                ZakazCase Item = (dynamic)ZakazTable.SelectedItem;
                LogDelete(Item);
                db.Zakazs.Remove(db.Zakazs.Where(e => e.IdZakaz == Item.IdZakaz).Single());
                db.SaveChanges();
                Initialize();
            }
        }
        private void LogDelete(ZakazCase zakaz)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " удалил запись в таблице ZAKAZ: " +
                       zakaz.DateZakaz + "^" + zakaz.NameGruz + "^" + zakaz.Otkuda + "^" + zakaz.Kuda + "^" + zakaz.DateVypoln + "^"
                       + zakaz.Marka + "^" + zakaz.FIOVod + "^" + zakaz.FIOKlient + "^" + zakaz.Kol + "^" + zakaz.Summa);
                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("");
            }
        }

        public void ZakazEditView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EditHeight.Height = new GridLength(0);
            Initialize();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Initialize();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (ZakazReportView.IsEnabled == false)
            {
                ZakazReportView.IsEnabled = true;
                ZakazReportView.Visibility = Visibility.Visible;
                ZakazReportView.Initialize();
                ReportWidth.Width = new GridLength(1.5, GridUnitType.Star);
            }
            else
            {
                ZakazReportView.IsEnabled = false;
                ZakazReportView.Visibility = Visibility.Hidden;
                ReportWidth.Width = new GridLength(0);
            }
        }

        private void ZakazTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ZakazList list = new ZakazList();
            list.Initialize((dynamic)ZakazTable.SelectedItem);
            list.Show();
        }
    }
}
