using System;
using System.Collections.Generic;
using System.Text;

namespace CarManagment.DB.Tables.DataGridCase
{
    class ZakazEditCase
    {
        public int IdVod { get; set; }
        public string FIOVod { get; set; }
        public int IdAvto { get; set; }
        public string Marka { get; set; }
        public double GruzPod { get; set; }
        public int IdVidGruz { get; set; }
        public string NameVidGruz { get; set; }
    }
}
