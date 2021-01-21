using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarManagment.DB.Tables
{
    [Table("ZAKAZ", Schema = "dbo")]
    class Zakaz
    {
        [Column("ID_ZAKAZ"), Key] public int IdZakaz { get; set; }
        [Column("DATE_ZAKAZ")] public DateTime DateZakaz { get; set; }
        [Column("ID_GRUZ")] public int IdGruz { get; set; }
        [Column("OTKUDA")] public string Otkuda{ get; set; }
        [Column("KUDA")] public string Kuda { get; set; }
        [Column("DATE_VYPOLN")] public DateTime DateVypoln { get; set; }
        [Column("ID_AVTO")] public int IdAvto { get; set; }
        [Column("ID_VOD")] public int IdVod { get; set; }
        [Column("SUMMA")] public double Summa { get; set; }
        [Column("ID_KLIENT")] public int IdKlient { get; set; }
        [Column("KOL")] public double Kol { get; set; }
    }
}
