using CarManagment.Cache;
using CarManagment.DB;
using CarManagment.DB.Tables;
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
    /// Interaction logic for VodView.xaml
    /// </summary>
    public partial class VodView : UserControl
    {
        private readonly Context db = new Context();

        public VodView()
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
            VodTable.ItemsSource = db.Vods.ToList();
        }

        public void AddItemsBySearch()
        {
            VodTable.ItemsSource = db.Vods.Where(e => e.F.Contains(Search.Text) || e.I.Contains(Search.Text) || e.O.Contains(Search.Text)).ToList();
        }


        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            VodEditView.IsEnabled = true;
            VodEditView.Visibility = Visibility.Visible;
            VodEditView.Initialize();
            EditHeight.Height = new GridLength(90);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (VodTable.SelectedIndex >= 0)
            {
                Vod Item = (dynamic)VodTable.SelectedItem;
                VodEditView.IsEnabled = true;
                VodEditView.Visibility = Visibility.Visible;
                VodEditView.Initialize(Item);
                EditHeight.Height = new GridLength(90);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить данные?", "Требуется подстверждение!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes && VodTable.SelectedIndex >= 0)
            {
                db.Vods.Remove((dynamic)VodTable.SelectedItem);
                db.SaveChanges();
                Initialize();
            }
        }

        private void LogDelete(Vod vod)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " удалил запись в таблице VOD: " +
                       +vod.IdVod + "^" + vod.F + "^" + vod.I + "^" + vod.O + "^" + vod.Klass + "^" + vod.Stazh);
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

        public void VodEditView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EditHeight.Height = new GridLength(0);
            Initialize();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (VodReportView.IsEnabled == false)
            {
                VodReportView.IsEnabled = true;
                VodReportView.Visibility = Visibility.Visible;
                VodReportView.Initialize();
                ReportWidth.Width = new GridLength(1.5, GridUnitType.Star);
            }
            else
            {
                VodReportView.IsEnabled = false;
                VodReportView.Visibility = Visibility.Hidden;
                ReportWidth.Width = new GridLength(0);
            }
        }
    }
}
