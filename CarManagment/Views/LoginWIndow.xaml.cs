using CarManagment.AddServices;
using CarManagment.Cache;
using CarManagment.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarManagment.Views
{
    /// <summary>
    /// Interaction logic for LoginWIndow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        Context db = new Context();
        public LoginWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            Login.ItemsSource = db.Users.Select(e => e.NameUser).ToList();
        }

        private bool Check()
        {
            if (Login.SelectedItem == null)
            {
                MessageBox.Show("Отсутствует имя пользователя! Введите данные заново!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            //MessageBox.Show(MD5Hash.GetMd5Hash(Password.Password));
            if (db.Users.Where(e => e.NameUser.Equals(Login.Text) && e.Password.Equals(MD5Hash.GetMd5Hash(Password.Password))).FirstOrDefault() == null)
            {
                MessageBox.Show("Неверный пароль! Введите данные заново!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            if (Check())
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Initialize(db.Users.Where(e => e.NameUser.Equals(Login.Text) && e.Password.Equals(MD5Hash.GetMd5Hash(Password.Password))).Single().IdUser);
                mainWindow.Show();
                Close();
            }
        }
    }
}
