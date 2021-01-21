using CarManagment.Cache;
using CarManagment.DB;
using CarManagment.DB.Tables;
using CarManagment.DB.Tables.DataGridCase;
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
    /// Interaction logic for VodAvtoView.xaml
    /// </summary>
    public partial class VodAvtoView : UserControl
    {
        private readonly Context db = new Context();

        public VodAvtoView()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            if (Search.Text.Equals("")) AddItems();
            else AddItemsBySearch();
        }

        public void AddItems()
        {
            var result = from vodavto in db.VodAvtos
                         join avto in db.Avtos on vodavto.IdAvto equals avto.IdAvto
                         join vod in db.Vods on vodavto.IdVod equals vod.IdVod
                         join vidgruz in db.VidGruzs on avto.IdVidGruz equals vidgruz.IdVidGruz
                         select new VodAvtoCase
                         {
                             IdVodAvto = vodavto.IdVodAvto,
                             FIO = vod.F + " " + vod.I + " " + vod.O,
                             Marka = avto.Marka + " \"" + vidgruz.NameVidGruz + "\""
                         };
            VodAvtoTable.ItemsSource = result.ToList();
        }

        public void AddItemsBySearch()
        {
            var result = from vodavto in db.VodAvtos
                         join avto in db.Avtos on vodavto.IdAvto equals avto.IdAvto
                         join vod in db.Vods on vodavto.IdVod equals vod.IdVod
                         where avto.Marka.Contains(Search.Text) || avto.Nomer.Contains(Search.Text) ||
                         vod.F.Contains(Search.Text) || vod.I.Contains(Search.Text) || vod.O.Contains(Search.Text)
                         select new VodAvtoCase
                         {
                             IdVodAvto = vodavto.IdVodAvto,
                             FIO = vod.F + " " + vod.I + " " + vod.O,
                             Marka = avto.Marka
                         };
            VodAvtoTable.ItemsSource = result.ToList();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            VodAvtoEditView.IsEnabled = true;
            VodAvtoEditView.Visibility = Visibility.Visible;
            VodAvtoEditView.Initialize();
            EditHeight.Height = new GridLength(150);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (VodAvtoTable.SelectedIndex >= 0)
            {
                VodAvtoCase Item = (dynamic)VodAvtoTable.SelectedItem;
                VodAvtoEditView.IsEnabled = true;
                VodAvtoEditView.Visibility = Visibility.Visible;
                VodAvtoEditView.Initialize(Item);
                EditHeight.Height = new GridLength(150);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить данные?", "Требуется подстверждение!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes && VodAvtoTable.SelectedIndex >= 0)
            {
                VodAvtoCase Item = (dynamic)VodAvtoTable.SelectedItem;
                LogDelete(Item);
                db.VodAvtos.Remove(db.VodAvtos.Where(e => e.IdVodAvto == Item.IdVodAvto).Single());
                db.SaveChanges();
                Initialize();
            }
        }

        private void LogDelete(VodAvtoCase vodAvto)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " удалил запись в таблице VODAVTO: " +
                       +vodAvto.IdVodAvto + "^" + vodAvto.FIO + "^" + vodAvto.Marka);
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

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Initialize();
        }

        public void VodAvtoEditView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EditHeight.Height = new GridLength(0);
            Initialize();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (VodAvtoReportView.IsEnabled == false)
            {
                VodAvtoReportView.IsEnabled = true;
                VodAvtoReportView.Visibility = Visibility.Visible;
                VodAvtoReportView.Initialize();
                ReportWidth.Width = new GridLength(1.5, GridUnitType.Star);
            }
            else
            {
                VodAvtoReportView.IsEnabled = false;
                VodAvtoReportView.Visibility = Visibility.Hidden;
                ReportWidth.Width = new GridLength(0);
            }
        }
    }
}
