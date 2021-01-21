using CarManagment.Cache;
using CarManagment.DB;
using CarManagment.DB.Tables;
using CarManagment.DB.Tables.DataGridCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for VodAvtoEditView.xaml
    /// </summary>
    public partial class VodAvtoEditView : UserControl
    {
        Context db = new Context();
        int SelectedId = 0;
        string SelectedUser;
        public VodAvtoEditView()
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
            var FIO = from vod in db.Vods select new string(vod.F + " " + vod.I + " " + vod.O);
            Vod.ItemsSource = FIO.ToList();
            var avtos = from avto in db.Avtos
                        join vidgruz in db.VidGruzs on avto.IdVidGruz equals vidgruz.IdVidGruz
                        select new string ( avto.Marka + " \"" + vidgruz.NameVidGruz + "\"" );
            Avto.ItemsSource = avtos.ToList();
            CleanFields();
        }

        public void Initialize(VodAvtoCase entity)
        {
            Initialize();
            SelectedId = entity.IdVodAvto;
            Vod.SelectedItem = entity.FIO;
            var avto = entity.Marka.Split("\"");
            Avto.SelectedItem = avto[1];
        }

        private void CleanFields()
        {
            SelectedId = 0;
            Vod.SelectedItem = null;
            Avto.SelectedItem = null;
        }

        private bool CheckFields()
        {
            if (Vod.SelectedItem == null)
            {
                MessageBox.Show("Отсутствуют данные в поле водителя. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Avto.SelectedItem == null)
            {
                MessageBox.Show("Отсутствуют данные в поле автомобиля. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {

            if (CheckFields())
            {
                var FIO = Vod.Text.Split(" ");
                var avto = Avto.Text.Split("\"");
                if (SelectedId == 0)
                {
                    
                    db.VodAvtos.Add(new VodAvto
                    {
                        IdVod = db.Vods.Where(e => e.F.Equals(FIO[0]) && e.I.Equals(FIO[1]) && e.O.Equals(FIO[2])).Single().IdVod,
                        IdAvto = db.Avtos.Single(e => e.Marka.Equals(avto[0].Substring(0, avto[0].Length - 1)) && e.IdVidGruz == db.VidGruzs.Single(e => e.NameVidGruz.Equals(avto[1])).IdVidGruz).IdAvto
                    });
                    LogInsert();
                }
                else
                {
                    VodAvto vodavto = db.VodAvtos.Where(e => e.IdVodAvto == SelectedId).Single();
                    LogUpdate(vodavto);
                    vodavto.IdVod = db.Vods.Where(e => e.F.Equals(FIO[0]) && e.I.Equals(FIO[1]) && e.O.Equals(FIO[2])).Single().IdVod;
                    vodavto.IdAvto = db.Avtos.Single(e => e.Marka.Equals(avto[0].Substring(0, avto[0].Length - 1)) && e.IdVidGruz == db.VidGruzs.Single(e => e.NameVidGruz.Equals(avto[1])).IdVidGruz).IdAvto;
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
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " добавил запись в таблицу VODAVTO: " +
                        Vod.Text + "^" + Avto.Text);
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
        private void LogUpdate(VodAvto vodAvto)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                var FIO = from vod in db.Vods where vod.IdVod == vodAvto.IdVod select new string(vod.F + " " + vod.I + " " + vod.O);
                writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " отредактировал запись в таблице VODAVTO: " +
                       +vodAvto.IdVodAvto + "^" + FIO + "^" + db.Avtos.Where(e => e.IdAvto == vodAvto.IdAvto).Single().Marka);
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
