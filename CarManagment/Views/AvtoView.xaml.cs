using CarManagment.Cache;
using CarManagment.DB;
using CarManagment.DB.Tables;
using CarManagment.DB.Tables.DataGridCase;
using CarManagment.Views.Reports;
using System;
using System.Collections.Generic;
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

namespace CarManagment.Views
{
    /// <summary>
    /// Interaction logic for Cars.xaml
    /// </summary>
    public partial class AvtoView : UserControl
    {
        private readonly Context db = new Context();
        public AvtoView()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            if (Search.Equals("")) AddItems();
            else AddItemsBySearch();
        }

        public void AddItems()
        {
            var result = from avto in db.Avtos
                         join vidgruz in db.VidGruzs on avto.IdVidGruz equals vidgruz.IdVidGruz
                         select new AvtoCase
                         {
                             IdAvto = avto.IdAvto,
                             Nomer = avto.Nomer,
                             Marka = avto.Marka,
                             GruzPod = avto.GruzPod,
                             VidGruz = vidgruz.NameVidGruz,
                             Ispr = avto.Ispr?"Исправен":"Неиспрвен"
                         };
            AvtoTable.ItemsSource = result.ToList();
        }

        public void AddItemsBySearch()
        {
            var result = from avto in db.Avtos
                         join vidgruz in db.VidGruzs on avto.IdVidGruz equals vidgruz.IdVidGruz
                         where avto.Marka.Contains(Search.Text) || avto.Nomer.Contains(Search.Text)
                         select new AvtoCase
                         {
                             IdAvto = avto.IdAvto,
                             Nomer = avto.Nomer,
                             Marka = avto.Marka,
                             GruzPod = avto.GruzPod,
                             VidGruz = vidgruz.NameVidGruz,
                             Ispr = avto.Ispr ? "Исправен" : "Неиспрвен"
                         };
            AvtoTable.ItemsSource = result.ToList();
        }
        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            AvtoEditView.IsEnabled = true;
            AvtoEditView.Visibility = Visibility.Visible;
            AvtoEditView.Initialize();
            EditHeight.Height = new GridLength(120);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if(AvtoTable.SelectedIndex >= 0)
            {
                AvtoCase Item = (dynamic)AvtoTable.SelectedItem;
                AvtoEditView.IsEnabled = true;
                AvtoEditView.Visibility = Visibility.Visible;
                AvtoEditView.Initialize(Item);
                EditHeight.Height = new GridLength(120);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить данные?", "Требуется подстверждение!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes && AvtoTable.SelectedIndex >= 0)
            {
                AvtoCase Item = (dynamic)AvtoTable.SelectedItem;
                LogDelete(Item);
                db.Avtos.Remove(db.Avtos.Where(e => e.IdAvto == Item.IdAvto).Single());
                db.SaveChanges();
                Initialize();
            }
        }

        private void LogDelete(AvtoCase avto)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " удалил запись в таблице AVTO: " +
                       + avto.IdAvto + "^" + avto.Marka + "^" + avto.Nomer + "^" + avto.GruzPod + "^" + avto.VidGruz + "^" + avto.Ispr);
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

        public void AvtoEditView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EditHeight.Height = new GridLength(0);
            Initialize();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (AvtoReportView.IsEnabled == false)
            {
                AvtoReportView.IsEnabled = true;
                AvtoReportView.Visibility = Visibility.Visible;
                AvtoReportView.Initialize();
                ReportWidth.Width = new GridLength(1.5, GridUnitType.Star);
            }
            else
            {
                AvtoReportView.IsEnabled = false;
                AvtoReportView.Visibility = Visibility.Hidden;
                ReportWidth.Width = new GridLength(0);
            }
        }
    }
}
