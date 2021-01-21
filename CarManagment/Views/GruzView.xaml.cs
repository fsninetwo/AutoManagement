using CarManagment.Cache;
using CarManagment.DB;
using CarManagment.DB.Tables;
using CarManagment.DB.Tables.DataGridCase;
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
    /// Interaction logic for GruzView.xaml
    /// </summary>
    public partial class GruzView : UserControl
    {
        private readonly Context db = new Context();

        public GruzView()
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
            var result = from gruz in db.Gruzs
                         join vidgruz in db.VidGruzs on gruz.IdVidGruz equals vidgruz.IdVidGruz
                         select new GruzCase
                         {
                             IdGruz = gruz.IdGruz,
                             NameGruz = gruz.NameGruz,
                             VidGruz = vidgruz.NameVidGruz,
                             Stoim = gruz.Stoim
                         };
            GruzTable.ItemsSource = result.ToList();
        }

        public void AddItemsBySearch()
        {
            var result = from gruz in db.Gruzs
                         join vidgruz in db.VidGruzs on gruz.IdVidGruz equals vidgruz.IdVidGruz
                         where gruz.NameGruz.Contains(Search.Text)
                         select new GruzCase
                         {
                             IdGruz = gruz.IdGruz,
                             NameGruz = gruz.NameGruz,
                             VidGruz = vidgruz.NameVidGruz,
                             Stoim = gruz.Stoim
                         };
            GruzTable.ItemsSource = result.ToList();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            GruzEditView.IsEnabled = true;
            GruzEditView.Visibility = Visibility.Visible;
            GruzEditView.Initialize();
            EditHeight.Height = new GridLength(120);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (GruzTable.SelectedIndex >= 0)
            {
                GruzCase Item = (dynamic)GruzTable.SelectedItem;
                GruzEditView.IsEnabled = true;
                GruzEditView.Visibility = Visibility.Visible;
                GruzEditView.Initialize(Item);
                EditHeight.Height = new GridLength(120);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить данные?", "Требуется подстверждение!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes && GruzTable.SelectedIndex >= 0)
            {
                GruzCase Item = (dynamic)GruzTable.SelectedItem;
                LogDelete(Item);
                db.Gruzs.Remove(db.Gruzs.Where(e => e.IdGruz == Item.IdGruz).Single());
                db.SaveChanges();
                Initialize();
            }
        }

        private void LogDelete(GruzCase gruz)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " удалил запись в таблице GRUZ: " +
                       +gruz.IdGruz + "^" + gruz.NameGruz + "^" + gruz.VidGruz + "^" + gruz.Stoim);
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

        public void GruzEditView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EditHeight.Height = new GridLength(0);
            Initialize();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (GruzReportView.IsEnabled == false)
            {
                GruzReportView.IsEnabled = true;
                GruzReportView.Visibility = Visibility.Visible;
                GruzReportView.Initialize();
                ReportWidth.Width = new GridLength(1.5, GridUnitType.Star);
            }
            else
            {
                GruzReportView.IsEnabled = false;
                GruzReportView.Visibility = Visibility.Hidden;
                ReportWidth.Width = new GridLength(0);
            }
        }
    }
}
