using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarManagment.DB.Tables
{
    [Table("KLIENT", Schema = "dbo")]
    public class Klient
    {
        [Column("ID_KLIENT"), Key] public int IdKlient { get; set; }
        [Column("FIO")] public string FIO{ get; set; }
        [Column("ADRES")] public string Adres { get; set; }
        [Column("TELEFON")] public string Telefon { get; set; }
    }
}
