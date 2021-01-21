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

namespace CarManagment.Views.EditViews
{
    /// <summary>
    /// Interaction logic for KlientEditView.xaml
    /// </summary>
    public partial class KlientEditView : UserControl
    {
        readonly Context db = new Context();
        int SelectedId = 0;
        public KlientEditView()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize() 
        {
            CleanFields();
        }

        public void Initialize(Klient entity)
        {
            Initialize();
            SelectedId = entity.IdKlient;
            FIO.Text = entity.FIO;
            Adres.Text = entity.Adres;
            Telefon.Text = entity.Telefon;
        }

        private void CleanFields()
        {
            FIO.Text = "";
            Adres.Text = "";
            Telefon.Text = "";
        }

        private bool CheckFields()
        {
            if (!Regex.IsMatch(FIO.Text, @"^\w+ \w+ \w+$"))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле имени. Введите полное ФИО!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(Adres.Text, ("\\w+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле адреса. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(Telefon.Text, @"\+\d+"))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле телефона. Введите полный номер телефона!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    db.Klients.Add(new Klient
                    {
                        FIO = FIO.Text,
                        Adres = Adres.Text,
                        Telefon = Telefon.Text
                    });
                    LogInsert();
                }
                else
                {
                    Klient klient = db.Klients.Where(e => e.IdKlient == SelectedId).Single();
                    LogUpdate(klient);
                    klient.FIO = FIO.Text;
                    klient.Adres = Adres.Text;
                    klient.Telefon = Telefon.Text;
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
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " добавил запись в таблицу KLIENT: " +
                        FIO.Text + "^" + Adres.Text + "^" + Telefon.Text);
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

        private void LogUpdate(Klient klient)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " отредактировал запись в таблице KLIENT: " +
                       +klient.IdKlient + "^" + klient.FIO + "^" + klient.Adres + "^" + klient.Telefon);
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
