using CarManagment.DB.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarManagment.DB
{
    class Context : DbContext
    {
        public DbSet<VidGruz> VidGruzs { get; set; }
        public DbSet<Avto> Avtos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vod> Vods { get; set; }
        public DbSet<Klient> Klients { get; set; }
        public DbSet<Gruz> Gruzs { get; set; }
        public DbSet<VodAvto> VodAvtos { get; set; }
        public DbSet<Zakaz> Zakazs { get; set; }
        //DESKTOP-I55QATK\\SQLEXPRESS
        //DESKTOP-K4L490P
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseNpgsql(@"host=localhost;database=AutoDB;user id=postgres;password=CfrVfq<jkc1Nfqvc;");
    }
}
