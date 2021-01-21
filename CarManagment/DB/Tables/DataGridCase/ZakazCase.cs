using System;
using System.Collections.Generic;
using System.Text;

namespace CarManagment.DB.Tables.DataGridCase
{
    public class ZakazCase
    {
        public int IdZakaz { get; set; }
        public DateTime DateZakaz { get; set; }
        public string NameGruz { get; set; }
        public string Otkuda { get; set; }
        public string Kuda { get; set; }
        public DateTime DateVypoln { get; set; }
        public string Marka { get; set; }
        public string FIOVod { get; set; }
        public double Summa { get; set; }
        public string FIOKlient { get; set; }
        public double Kol { get; set; }
    }
}
