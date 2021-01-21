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
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
        private readonly Context db = new Context();

        public UserView()
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
            UserTable.ItemsSource = db.Users.ToList();
        }

        public void AddItemsBySearch()
        {
            UserTable.ItemsSource = db.Users.Where(e => e.NameUser.Contains(Search.Text)).ToList();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            UserEditView.IsEnabled = true;
            UserEditView.Visibility = Visibility.Visible;
            UserEditView.Initialize();
            EditHeight.Height = new GridLength(150);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (UserTable.SelectedIndex >= 0)
            {
                User Item = (dynamic)UserTable.SelectedItem;
                UserEditView.IsEnabled = true;
                UserEditView.Visibility = Visibility.Visible;
                UserEditView.Initialize(Item);
                EditHeight.Height = new GridLength(150);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить данные?", "Требуется подстверждение!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes && UserTable.SelectedIndex >= 0)
            {
                LogDelete((dynamic)UserTable.SelectedItem);
                db.Users.Remove((dynamic)UserTable.SelectedItem);
                db.SaveChanges();
                Initialize();
            }
        }
        
        private void LogDelete(User user)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " удалил запись в таблице USER: " +
                       + user.IdUser + "^" + user.NameUser + "^" + user.Password + "^" + user.Adres + "^" + user.Birthday
                       + "^" + user.Dolzh + "^" + user.Oklad + "^" + user.Priem + "^" + user.NPrikazPriem + "^" + user.Uvol
                       + "^" + user.NPrikazUvol);
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

        public void UserEditView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EditHeight.Height = new GridLength(0);
            Initialize();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (UserReportView.IsEnabled == false)
            {
                UserReportView.IsEnabled = true;
                UserReportView.Visibility = Visibility.Visible;
                UserReportView.Initialize();
                ReportWidth.Width = new GridLength(1.5, GridUnitType.Star);
            }
            else
            {
                UserReportView.IsEnabled = false;
                UserReportView.Visibility = Visibility.Hidden;
                ReportWidth.Width = new GridLength(0);
            }
        }
    }
}
