using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarManagment.DB.Tables
{
    [Table("VODAVTO", Schema = "dbo")]
    class VodAvto
    {
        [Column("ID_VODAVTO"), Key] public int IdVodAvto { get; set; }
        [Column("ID_VOD")] public int IdVod { get; set; }
        [Column("ID_AVTO")] public int IdAvto { get; set; }
    }
}
