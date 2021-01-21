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
    /// Interaction logic for KlientView.xaml
    /// </summary>
    public partial class KlientView : UserControl
    {
        private readonly Context db = new Context();
        public KlientView()
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
            KlientTable.ItemsSource = db.Klients.ToList();
        }

        public void AddItemsBySearch()
        {
            KlientTable.ItemsSource = db.Klients.Where(e => e.FIO.Contains(Search.Text)).ToList();
        }
        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            KlientEditView.IsEnabled = true;
            KlientEditView.Visibility = Visibility.Visible;
            KlientEditView.Initialize();
            EditHeight.Height = new GridLength(90);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (KlientTable.SelectedIndex >= 0)
            {
                Klient Item = (dynamic)KlientTable.SelectedItem;
                KlientEditView.IsEnabled = true;
                KlientEditView.Visibility = Visibility.Visible;
                KlientEditView.Initialize(Item);
                EditHeight.Height = new GridLength(90);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить данные?", "Требуется подстверждение!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes && KlientTable.SelectedIndex >= 0)
            {
                LogDelete((dynamic)KlientTable.SelectedItem);
                db.Klients.Remove((dynamic)KlientTable.SelectedItem);
                db.SaveChanges();
                Initialize();
            }
        }

        private void LogDelete(Klient klient)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " удалил запись в таблице KLIENT: " +
                       + klient.IdKlient + "^" + klient.FIO + "^" + klient.Adres + "^" + klient.Telefon);
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

        public void KlientEditView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EditHeight.Height = new GridLength(0);
            Initialize();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (KlientReportView.IsEnabled == false)
            {
                KlientReportView.IsEnabled = true;
                KlientReportView.Visibility = Visibility.Visible;
                KlientReportView.Initialize();
                ReportWidth.Width = new GridLength(1.5, GridUnitType.Star);
            }
            else
            {
                KlientReportView.IsEnabled = false;
                KlientReportView.Visibility = Visibility.Hidden;
                ReportWidth.Width = new GridLength(0);
            }
        }
    }
}
