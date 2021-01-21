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
    /// Interaction logic for VodEditView.xaml
    /// </summary>
    public partial class VodEditView : UserControl
    {
        Context db = new Context();
        int SelectedId = 0;
        string SelectedUser;
        public VodEditView()
        {
            InitializeComponent();
            Initialize();
        }

        public void InitializeUser(string selectedUser)
        {
            SelectedUser = selectedUser;
        }

        public void Initialize() 
        {
            CleanFields();
        }

        public void Initialize(Vod entity)
        {
            Initialize();
            SelectedId = entity.IdVod;
            F.Text = entity.F;
            I.Text = entity.I;
            O.Text = entity.O;
            Klass.Text = entity.Klass.ToString();
            Stazh.Text = entity.Stazh.ToString();
        }

        private void CleanFields()
        {
            SelectedId = 0;
            F.Text = "";
            I.Text = "";
            O.Text = "";
            Klass.Text = "";
            Stazh.Text = "";
        }

        private bool CheckFields()
        {
            if (!Regex.IsMatch(F.Text, ("\\w+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле фамилии. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(I.Text, ("\\w+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле имени. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(O.Text, ("\\w+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле отчества. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(Klass.Text, ("\\d+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле класса. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(Stazh.Text, ("\\d+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле стажа. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    db.Vods.Add(new Vod
                    {
                        F = F.Text,
                        I = I.Text,
                        O = O.Text,
                        Klass = Convert.ToInt32(Klass.Text),
                        Stazh = Convert.ToInt32(Stazh.Text)
                    });
                    LogInsert();
                }
                else
                {
                    Vod vod = db.Vods.Where(e => e.IdVod == SelectedId).Single();
                    LogUpdate(vod);
                    vod.F = F.Text;
                    vod.I = I.Text;
                    vod.O = O.Text;
                    vod.Klass = Convert.ToInt32(Klass.Text);
                    vod.Stazh = Convert.ToInt32(Stazh.Text);
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
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " добавил запись в таблицу VOD: " +
                        F.Text + "^" + I.Text + "^" + O.Text + "^" + Klass.Text + "^" + Stazh.Text);
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

        private void LogUpdate(Vod vod)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " отредактировал запись в таблице VOD: " +
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
