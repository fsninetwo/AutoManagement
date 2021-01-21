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

namespace CarManagment.Views.EditViews
{
    /// <summary>
    /// Interaction logic for AvtoEditView.xaml
    /// </summary>
    public partial class AvtoEditView : UserControl
    {
        readonly Context db = new Context();
        int SelectedId = 0;
        public AvtoEditView()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            VidGruz.ItemsSource = db.VidGruzs.Select(e => e.NameVidGruz).ToList();
            CleanFields();
        }

        public void Initialize(AvtoCase entity)
        {
            Initialize();
            SelectedId = entity.IdAvto;
            Marka.Text = entity.Marka;
            Nomer.Text = entity.Nomer;
            GruzPod.Text = entity.GruzPod.ToString();
            VidGruz.SelectedItem = entity.VidGruz;
            if (entity.Ispr.Equals("Исправен")) Ispr.IsChecked = true;
            else Ispr.IsChecked = false;
        }

        private void CleanFields()
        {
            Marka.Text = "";
            Nomer.Text = "";
            GruzPod.Text = "";
            VidGruz.SelectedItem = null;
            Ispr.IsChecked = false;
        }

        private bool CheckFields()
        {
            if (!Marka.Text.Equals("") && !Regex.IsMatch(Marka.Text, ("\\w+")))
            {
                MessageBox.Show("Неверные данные в поле марки. Введите заново!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Nomer.Text.Equals("") && !Regex.IsMatch(Nomer.Text, "\\w{2}-\\d{4}"))
            {
                MessageBox.Show("Неверные данные в поле пароля. Введите в виде \"AA-1111\"!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(GruzPod.Text, ("\\d+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле введите заново. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (VidGruz.SelectedItem == null)
            {
                MessageBox.Show("Отсутствуют данные в поле вида груза. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    db.Avtos.Add(new Avto
                    {
                        Marka = Marka.Text,
                        Nomer = Nomer.Text,
                        GruzPod = Convert.ToDouble(GruzPod.Text),
                        IdVidGruz = db.VidGruzs.Where(e => e.NameVidGruz.Equals(VidGruz.Text)).Single().IdVidGruz,
                        Ispr = Ispr.IsChecked.Value
                    });
                    LogInsert();
                }
                else
                {
                    Avto avto = db.Avtos.Where(e => e.IdAvto == SelectedId).Single();
                    LogUpdate(avto);
                    avto.Marka = Marka.Text;
                    avto.Nomer = Nomer.Text;
                    avto.GruzPod = Convert.ToDouble(GruzPod.Text);
                    avto.IdVidGruz = db.VidGruzs.Where(e => e.NameVidGruz.Equals(VidGruz.Text)).Single().IdVidGruz;
                    avto.Ispr = Ispr.IsChecked.Value;
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
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " добавил запись в таблицу AVTO: " +
                        Marka.Text + "^" + Nomer.Text + "^" + GruzPod.Text + "^" + VidGruz.Text + "^" + Ispr.IsChecked.ToString());
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

        private void LogUpdate(Avto avto)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " отредактировал запись в таблице AVTO: " +
                       + avto.IdAvto + "^" + avto.Marka + "^" + avto.Nomer + "^" + avto.GruzPod + "^" 
                       + db.VidGruzs.Where(e => e.IdVidGruz == avto.IdVidGruz).Single().NameVidGruz + "^" + avto.Ispr);
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
