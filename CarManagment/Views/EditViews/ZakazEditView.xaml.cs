using CarManagment.DB;
using CarManagment.DB.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using CarManagment.DB.Tables.DataGridCase;
using CarManagment.Cache;
using Microsoft.EntityFrameworkCore;
using CarManagment.Views.AddWindows;

namespace CarManagment.Views.EditViews
{
    /// <summary>
    /// Interaction logic for ZakazEditView.xaml
    /// </summary>
    public partial class ZakazEditView : UserControl
    {
        readonly Context db = new Context();
        int SelectedId = 0;
        DateTime dateVypoln;
        string[] gruz;
        EntityState pos = EntityState.Added;
        public ZakazEditView()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            pos = EntityState.Added;
            CleanFields();
            var klients = from klient in db.Klients select new string(klient.FIO + " " + klient.Adres + " " + klient.Telefon);
            Klient.ItemsSource = klients.ToList();
            var gruzs = from gruz in db.Gruzs
                        join vidgruz in db.VidGruzs on gruz.IdVidGruz equals vidgruz.IdVidGruz
                        select new string(gruz.NameGruz + " \"" + vidgruz.NameVidGruz + "\" - " + gruz.Stoim + " за 1 кг");
            Gruz.ItemsSource = gruzs.ToList();
            SelectZakaz();
        }
        public void Initialize(ZakazCase zakazCase)
        {
            Initialize();
            pos = EntityState.Modified;
            SelectedId = zakazCase.IdZakaz;
            DateZakaz.SelectedDate = zakazCase.DateZakaz;
            var klients = from klient in db.Klients where klient.FIO.Equals(zakazCase.FIOKlient) select new string(klient.FIO + " " + klient.Adres + " " + klient.Telefon);
            Klient.SelectedItem = klients.Single();
            var gruzs = from gruz in db.Gruzs
                        join vidgruz in db.VidGruzs on gruz.IdVidGruz equals vidgruz.IdVidGruz
                        where gruz.NameGruz.Equals(zakazCase.NameGruz)
                        select new string(gruz.NameGruz + " \"" + vidgruz.NameVidGruz + "\" - " + gruz.Stoim + " за 1 кг");
            Gruz.SelectedItem = gruzs.Single();
            string[] Text = Gruz.Text.Split(" ");
            Price.Content = db.Gruzs.Where( e => e.NameGruz.Equals(Text[0])).Single().Stoim;
            dateVypoln = zakazCase.DateVypoln;
            DateVypoln.SelectedDate = zakazCase.DateVypoln;
            Otkuda.Text = zakazCase.Otkuda;
            Kuda.Text = zakazCase.Kuda;
            Kol.Text = zakazCase.Kol.ToString();
            Summa.Content = zakazCase.Summa.ToString();
        }

        private void CleanFields()
        {
            SelectedId = 0;
            DateZakaz.SelectedDate = DateTime.Now;
            Klient.SelectedItem = null;
            Gruz.SelectedItem = null;
            Price.Content = "";
            dateVypoln = DateTime.Now;
            DateVypoln.SelectedDate = DateTime.Now;
            Otkuda.Text = "";
            Kuda.Text = "";
            Kol.Text = "0";
            Summa.Content = "";
        }
        
        private void Gruz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Gruz.SelectedItem != null)
            {
                gruz = Gruz.SelectedItem.ToString().Split("\"");
                Price.Content = db.Gruzs.Where(e => e.NameGruz.Equals(gruz[0].Substring(0, gruz[0].Length - 1))).Single().Stoim;
                if (Regex.IsMatch(Kol.Text, "\\d+"))
                    Summa.Content = (Convert.ToDouble(Price.Content) * Convert.ToDouble(Kol.Text)).ToString();
                SelectZakaz();
            }
        }

        private void Kol_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(Kol.Text, out double num)&& double.TryParse(Price.Content.ToString(), out double number))
                Summa.Content = (Convert.ToDouble(Price.Content) * Convert.ToDouble(Kol.Text)).ToString();
            else Summa.Content = "";
            SelectZakaz();
        }

        private void ZakazEditTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ZakazEditTable.SelectedIndex >= 0)
            {
                var zakaz = (dynamic)ZakazEditTable.SelectedItem;
                Avto.Content = zakaz.Marka;
                Vod.Content = zakaz.FIOVod;
            }
        }

        private bool CheckFields()
        {
            if (DateZakaz.SelectedDate == null)
            {
                MessageBox.Show("Отсутствуют данные в поле даты заказа. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Gruz.SelectedItem == null)
            {
                MessageBox.Show("Отсутствуют данные в поле автомобиля. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Klient.SelectedItem == null)
            {
                MessageBox.Show("Отсутствуют данные в поле клиента. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(Otkuda.Text, "\\w+"))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле откуда. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
           
            if (!Regex.IsMatch(Kuda.Text, "\\w+"))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле откуда. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (DateVypoln.SelectedDate == null)
            {
                MessageBox.Show("Отсутствуют данные в поле даты выполнения. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (DateZakaz.SelectedDate > DateVypoln.SelectedDate)
            {
                MessageBox.Show("Дата заказа не может быть позже даты выполнения. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ZakazEditTable.SelectedItem == null)
            {
                MessageBox.Show("Отсутствуют данные в поле автомобиля. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(Kol.Text, "\\d+"))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле количества. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(Summa.Content.ToString(), "\\d+"))
            {
                MessageBox.Show("Отсутствуют или неверные данные в поле суммы. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            var item = ZakazEditTable.SelectedItem as ZakazEditCase;
            var zakaz = db.Zakazs.FirstOrDefault(e => e.IdVod == item.IdVod);
            if (zakaz != null)
            {
                if ((pos == EntityState.Added && DateVypoln.SelectedDate == zakaz.DateVypoln) ||
                (pos == EntityState.Modified && DateVypoln.SelectedDate == zakaz.DateVypoln && DateVypoln.SelectedDate != dateVypoln))
                {
                    MessageBox.Show("Заказ может быть только по одному в день. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            var avto = db.Avtos.First(e => e.IdAvto == item.IdAvto);
            if (avto.GruzPod < Convert.ToDouble(Kol.Text))
            {
                MessageBox.Show("Количество груза не может быть больше грузоподъемности машины. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            string[] Text = Gruz.SelectedItem.ToString().Split("\"");
            if (!db.VidGruzs.First(e => e.IdVidGruz == avto.IdVidGruz).NameVidGruz.Equals(Text[1]))
            {
                MessageBox.Show("Неврный вид груза. Введите заново!", "Неверные данные!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            
            if (CheckFields())
            {
                string[] gruz = Gruz.SelectedItem.ToString().Split(" ");
                ZakazEditCase item = (dynamic)ZakazEditTable.SelectedItem;
                string[] FIOVod = item.FIOVod.Split(" ");
                string[] FIOKlient = Klient.Text.Split(" ");
                if (SelectedId == 0)
                {
                    db.Zakazs.Add(new Zakaz
                    {
                        DateZakaz = DateZakaz.SelectedDate.Value,
                        IdGruz = db.Gruzs.Where(e => e.NameGruz.Equals(gruz[0])).Single().IdGruz,
                        Otkuda = Otkuda.Text,
                        Kuda = Kuda.Text,
                        DateVypoln = DateVypoln.SelectedDate.Value,
                        IdAvto = db.Avtos.Where(e => e.Marka.Equals(item.Marka)).Single().IdAvto,
                        IdVod = db.Vods.Where(e => e.F.Equals(FIOVod[0]) && e.I.Equals(FIOVod[1]) && e.O.Equals(FIOVod[2])).Single().IdVod,
                        Summa = Convert.ToDouble(Summa.Content.ToString()),
                        IdKlient = db.Klients.Where(e => e.FIO.Equals(FIOKlient[0] + " " + FIOKlient[1] + " " + FIOKlient[2])).Single().IdKlient,
                        Kol = Convert.ToDouble(Kol.Text)
                    }) ;
                }
                else
                {
                    Zakaz zakaz = db.Zakazs.Where(e => e.IdZakaz == SelectedId).Single();
                    LogUpdate(zakaz);
                    zakaz.DateZakaz = DateZakaz.SelectedDate.Value;
                    zakaz.IdGruz = db.Gruzs.Where(e => e.NameGruz.Equals(gruz[0])).Single().IdGruz;
                    zakaz.Otkuda = Otkuda.Text;
                    zakaz.Kuda = Kuda.Text;
                    zakaz.DateVypoln = DateVypoln.SelectedDate.Value;
                    zakaz.IdAvto = db.Avtos.Where(e => e.Marka.Equals(item.Marka)).Single().IdAvto;
                    zakaz.IdVod = db.Vods.Where(e => e.F.Equals(FIOVod[0]) || e.I.Equals(FIOVod[1]) || e.O.Equals(FIOVod[2])).Single().IdVod;
                    zakaz.Summa = Convert.ToDouble(Summa.Content.ToString());
                    zakaz.IdKlient = db.Klients.Where(e => e.FIO.Equals(FIOKlient[0] + " " + FIOKlient[1] + " " + FIOKlient[2])).Single().IdKlient;
                    zakaz.Kol = Convert.ToInt32(Kol.Text);
                }
                db.SaveChanges();
                ZakazList list = new ZakazList();
                list.Initialize(new ZakazCase
                {
                    IdZakaz = SelectedId,
                    DateZakaz = DateZakaz.SelectedDate.Value,
                    NameGruz = gruz[0],
                    Otkuda = Otkuda.Text,
                    Kuda = Kuda.Text,
                    DateVypoln = DateVypoln.SelectedDate.Value,
                    Marka = item.Marka,
                    FIOVod = FIOVod[0] + " " + FIOVod[1] + " " + FIOVod[2],
                    Summa = Convert.ToDouble(Summa.Content.ToString()),
                    FIOKlient = FIOKlient[0] + " " + FIOKlient[1] + " " + FIOKlient[2],
                    Kol = Convert.ToInt32(Kol.Text),
                });
                list.Show();
                Exit();
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


        private void LogInsert()
           {
               try
               {
                    string[] gruz = Gruz.SelectedItem.ToString().Split(" ");
                    ZakazEditCase item = (dynamic)ZakazEditTable.SelectedItem;
                    string[] FIOKlient = item.FIOVod.Split(" ");
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                    writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " добавил запись в таблицу ZAKAZ: " +
                               DateZakaz.SelectedDate + "^" + gruz[0] + "^" + Otkuda.Text + "^" + Kuda.Text + "^" + DateVypoln.SelectedDate
                               + item.Marka + "^" + item.FIOVod + "^" + FIOKlient[0] + " " + FIOKlient[1] + " " + FIOKlient[2] 
                               + "^" + Kol.Text + "^" + Summa.Content.ToString());
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

           private void LogUpdate(Zakaz zakaz)
           {
               try
               {
                   System.IO.StreamWriter writer = new System.IO.StreamWriter(@"Log.txt", true);
                   var FIO = from vod in db.Vods where vod.IdVod == zakaz.IdVod select new string(vod.F + " " + vod.I + " " + vod.O);
                   writer.WriteLine(DateTime.Now.ToString() + " Пользователь " + ActiveUser.NameUser + " отредактировал запись в таблице ZAKAZ: " +
                          zakaz.DateZakaz + "^" + db.Gruzs.Where(e => e.IdGruz == zakaz.IdGruz).Single().NameGruz + "^" + zakaz.Otkuda 
                          + "^" + zakaz.Kuda + "^" + zakaz.DateVypoln + "^" + db.Avtos.Where(e => e.IdAvto == zakaz.IdAvto).Single().Marka
                          + "^" + FIO + "^" + db.Klients.Where(e => e.IdKlient == zakaz.IdKlient).Single().FIO + "^" + zakaz.Kol + "^" + zakaz.Summa);
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

        private void SelectZakaz()
        {
            if (gruz != null && DateVypoln.SelectedDate != null && double.TryParse(Kol.Text, out double number))
            {
                if (pos == EntityState.Added)
                {
                    var result = from vod in db.Vods
                                 join vodavto in db.VodAvtos on vod.IdVod equals vodavto.IdVod
                                 join avto in db.Avtos on vodavto.IdAvto equals avto.IdAvto
                                 join vidgruz in db.VidGruzs on avto.IdVidGruz equals vidgruz.IdVidGruz
                                 where avto.Ispr == true && avto.GruzPod >= Convert.ToDouble(Kol.Text) && vidgruz.NameVidGruz.Equals(gruz[1]) 
                                 select new ZakazEditCase
                                 {
                                     IdVod = vod.IdVod,
                                     FIOVod = vod.F + " " + vod.I + " " + vod.O,
                                     IdAvto = avto.IdAvto,
                                     Marka = avto.Marka,
                                     GruzPod = avto.GruzPod,
                                     IdVidGruz = vidgruz.IdVidGruz,
                                     NameVidGruz = vidgruz.NameVidGruz,
                                 };
                    ZakazEditTable.ItemsSource = result.ToList();
                }
                else if (pos == EntityState.Modified)
                {
                    var result = from vod in db.Vods
                                 join vodavto in db.VodAvtos on vod.IdVod equals vodavto.IdVod
                                 join avto in db.Avtos on vodavto.IdAvto equals avto.IdAvto
                                 join vidgruz in db.VidGruzs on avto.IdVidGruz equals vidgruz.IdVidGruz
                                 where avto.Ispr == true && avto.GruzPod >= Convert.ToDouble(Kol.Text)
                                 && vidgruz.NameVidGruz.Equals(gruz[1])
                                 select new ZakazEditCase
                                 {
                                     IdVod = vod.IdVod,
                                     FIOVod = vod.F + " " + vod.I + " " + vod.O,
                                     IdAvto = avto.IdAvto,
                                     Marka = avto.Marka,
                                     GruzPod = avto.GruzPod,
                                     IdVidGruz = vidgruz.IdVidGruz,
                                     NameVidGruz = vidgruz.NameVidGruz,
                                 };
                    ZakazEditTable.ItemsSource = result.ToList();
                }
            }
        }

        private void DateVypoln_KeyDown(object sender, KeyEventArgs e)
        {
            if(DateVypoln != null) SelectZakaz();
        }

        private void DateVypoln_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateVypoln != null) SelectZakaz();
        }

        private void DateVypoln_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DateVypoln != null) SelectZakaz();
        }
    }
}
