using CarManagment.AddServices;
using CarManagment.Cache;
using CarManagment.DB;
using CarManagment.DB.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace CarManagment.Views.EditViews
{
    /// <summary>
    /// Interaction logic for UserEditView.xaml
    /// </summary>
    public partial class UserEditView : UserControl
    {
        readonly Context db = new Context();
        int SelectedId = 0;
        public UserEditView()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize() 
        {
            CleanFields();
        }

        public void Initialize(User entity)
        {
            Initialize();
            SelectedId = entity.IdUser;
            NameUser.Text = entity.NameUser;
            Adres.Text = entity.Adres;
            Birthday.SelectedDate = entity.Birthday;
            Dolzh.Text = entity.Dolzh;
            Oklad.Text = entity.Oklad.ToString();
            Priem.SelectedDate = entity.Priem;
            NPrikazPriem.Text = entity.NPrikazPriem;
            Uvol.SelectedDate = entity.Uvol;
            NPrikazUvol.Text = entity.NPrikazUvol;
        }

        private void CleanFields()
        {
            NameUser.Text = "";
            Password.Text = "";
            Adres.Text = "";
            Dolzh.Text = "";
            Oklad.Text = "";
            NPrikazPriem.Text = "";
            NPrikazUvol.Text = "";
        }

        private bool CheckFields()
        {
            if (!Regex.IsMatch(NameUser.Text, ("\\w+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле имени. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Password.Text.Equals("") && !Regex.IsMatch(Password.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле пароля. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Adres.Text.Equals("") && !Regex.IsMatch(Adres.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле адреса. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if(Birthday.SelectedDate > DateTime.Now)
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
                MessageBox.Show("Неверные данные в поле оклада. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!NPrikazPriem.Text.Equals("") && !Regex.IsMatch(NPrikazPriem.Text, ("\\d+")))
            {
                MessageBox.Show("Неверные данные в поле приказа приема. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!NPrikazUvol.Text.Equals("") && !Regex.IsMatch(NPrikazUvol.Text, ("\\d+")))
            {
                MessageBox.Show("Неверные данные в поле приказа увольнения. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {

            if (CheckFields())
            {
                if (SelectedId == 0)
                {
                    
                    User user = new User
                    {
                        NameUser = NameUser.Text,
                        Password = MD5Hash.GetMd5Hash(Password.Text),
                        Adres = Adres.Text,
                        Birthday = Birthday.SelectedDate,
                        Dolzh = Dolzh.Text,
                        Priem = Priem.SelectedDate,
                        NPrikazPriem = NPrikazPriem.Text,
                        Uvol = Uvol.SelectedDate,
                        NPrikazUvol = NPrikazUvol.Text
                    };
                    if (!Oklad.Text.Equals("")) user.Oklad = Convert.ToDecimal(Oklad.Text);
                    db.Users.Add(user);
                    LogInsert();
                }
                else
                {
                    User user = db.Users.Where(e => e.IdUser == SelectedId).Single();
                    LogUpdate(user);
                    user.NameUser = NameUser.Text;
                    user.Password = MD5Hash.GetMd5Hash(Password.Text);
                    user.Adres = Adres.Text;
                    user.Birthday = Birthday.SelectedDate;
                    user.Dolzh = Dolzh.Text;
                    if (!Oklad.Text.Equals("")) user.Oklad = Convert.ToDecimal(Oklad.Text);
                    user.Priem = Priem.SelectedDate;
                    user.NPrikazPriem = NPrikazPriem.Text;
                    user.Uvol = Uvol.SelectedDate;
                    user.NPrikazUvol = NPrikazUvol.Text;
                }
                db.SaveChanges();
                Exit();
            }
        }

        private void LogInsert()
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " добавил запись в таблицу USER: " +
                        NameUser.Text + "^" + Password.Text + "^" + Adres.Text + "^" + Birthday.SelectedDate + "^" + Dolzh.Text
                        + "^" + Oklad.Text + "^" + Priem.SelectedDate + "^" + NPrikazPriem.Text + "^" + Uvol.SelectedDate
                        + "^" + NPrikazUvol.Text);
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
        private void LogUpdate(User user)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " отредактировал запись в таблице USER: " +
                       +user.IdUser + "^" + user.NameUser + "^" + user.Password + "^" + user.Adres + "^" + user.Birthday
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void Exit()
        {
            SelectedId = 0;
            IsEnabled = false;
            Visibility = Visibility.Hidden;
        }
    }
}
