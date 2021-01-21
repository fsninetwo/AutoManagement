using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarManagment.DB.Tables
{
    [Table("AVTO", Schema = "dbo")]
    class Avto
    {
        [Column("ID_AVTO"), Key] public int IdAvto { get; set; }
        [Column("MARKA")] public string Marka { get; set; }
        [Column("NOMER")] public string Nomer { get; set; }
        [Column("GRUZPOD")] public double GruzPod { get; set; }
        [Column("ID_VID_GRUZ")] public int IdVidGruz { get; set; }
        [Column("ISPR")] public bool Ispr { get; set; }
    }
}
