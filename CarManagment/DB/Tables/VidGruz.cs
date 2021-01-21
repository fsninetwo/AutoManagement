using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarManagment.DB.Tables
{
    [Table("VID_GRUZ", Schema = "dbo")]
    class VidGruz
    {
        [Column("ID_VID_GRUZ"), Key] public int IdVidGruz { get; set; }
        [Column("NAME_VID_GRUZ")] public string NameVidGruz { get; set; }
    }
}
