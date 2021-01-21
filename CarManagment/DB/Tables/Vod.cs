using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarManagment.DB.Tables
{
    [Table("VOD", Schema = "dbo")]
    public class Vod
    {
        [Column("ID_VOD"), Key] public int IdVod { get; set; }
        [Column("F")] public string F { get; set; }
        [Column("I")] public string I { get; set; }
        [Column("O")] public string O { get; set; }
        [Column("KLASS")] public int Klass { get; set; }
        [Column("STAZH")] public int Stazh { get; set; }
    }
}
