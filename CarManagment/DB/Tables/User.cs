using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarManagment.DB.Tables
{
    [Table("USERS", Schema= "dbo")]
    public class User
    {
        [Column("ID_USER"), Key] public int IdUser { get; set; }
        [Column("NAME_USER")] public string NameUser { get; set; }
        [Column("PASSWORD")] public string Password { get; set; }
        [Column("ADRES")] public string Adres { get; set; }
        [Column("BIRTHDAY")] public DateTime? Birthday { get; set; }
        [Column("DOLZH")] public string Dolzh { get; set; }
        [Column("OKLAD")] public decimal? Oklad { get; set; }
        [Column("PRIEM")] public DateTime? Priem { get; set; }
        [Column("NPRIKAZPRIEM")] public string NPrikazPriem { get; set; }
        [Column("UVOL")] public DateTime? Uvol { get; set; }
        [Column("NPRIKAZUVOL")] public string NPrikazUvol { get; set; }
    }
}
