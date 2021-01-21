using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarManagment.DB.Tables
{
    [Table("GRUZ", Schema = "dbo")]
    class Gruz
    {
        [Column("ID_GRUZ"), Key] public int IdGruz { get; set; }
        [Column("NAME_GRUZ")] public string NameGruz { get; set; }
        [Column("ID_VID_GRUZ")] public int IdVidGruz { get; set; }
        [Column("STOIM_1")] public double Stoim { get; set; }
    }
}
