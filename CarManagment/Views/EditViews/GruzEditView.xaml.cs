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
    /// Interaction logic for GruzEditView.xaml
    /// </summary>
    public partial class GruzEditView : UserControl
    {
        readonly Context db = new Context();
        int SelectedId = 0;
        public GruzEditView()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            VidGruz.ItemsSource = db.VidGruzs.Select(e => e.NameVidGruz).ToList();
            CleanFields();
        }

        public void Initialize(GruzCase entity)
        {
            Initialize();
            SelectedId = entity.IdGruz;
            NameGruz.Text = entity.NameGruz;
            VidGruz.SelectedItem = entity.VidGruz;
            Stoim.Text = entity.Stoim.ToString();
        }

        private void CleanFields()
        {
            NameGruz.Text = "";
            VidGruz.SelectedItem = null;
            Stoim.Text = "";
        }

        private bool CheckFields()
        {
            if (!Regex.IsMatch(NameGruz.Text, ("\\w+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле названия. Введите заново. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (VidGruz.SelectedItem == null)
            {
                MessageBox.Show("Отсутствуют данные в поле вида груза. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(Stoim.Text, ("\\d+")))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле стоимости. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    db.Gruzs.Add(new Gruz
                    {
                        NameGruz = NameGruz.Text,
                        IdVidGruz = db.VidGruzs.Where(e => e.NameVidGruz.Equals(VidGruz.Text)).Single().IdVidGruz,
                        Stoim = Convert.ToDouble(Stoim.Text),
                    });
                    LogInsert();
                }
                else
                {
                    Gruz gruz = db.Gruzs.Where(e => e.IdGruz == SelectedId).Single();
                    LogUpdate(gruz);
                    gruz.NameGruz = NameGruz.Text;
                    gruz.IdVidGruz = db.VidGruzs.Where(e => e.NameVidGruz.Equals(VidGruz.Text)).Single().IdVidGruz;
                    gruz.Stoim = Convert.ToDouble(Stoim.Text);
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
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " добавил запись в таблицу GRUZ: " +
                        NameGruz.Text + "^" + VidGruz.Text + "^" + Stoim.Text);
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

        private void LogUpdate(Gruz gruz)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " отредактировал запись в таблице GRUZ: " +
                       +gruz.IdGruz + "^" + gruz.NameGruz + db.VidGruzs.Where(e => e.IdVidGruz == gruz.IdVidGruz).Single().NameVidGruz
                       + "^" + gruz.Stoim);
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
