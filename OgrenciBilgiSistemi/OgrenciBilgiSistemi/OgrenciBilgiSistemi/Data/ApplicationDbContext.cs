using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Fakulte> Fakulteler { get; set; }
        public DbSet<Bolum> Bolumler { get; set; }
        public DbSet<Ogrenci> Ogrenciler { get; set; }
        public DbSet<Ders> Dersler { get; set; }
        public DbSet<OgrenciDers> OgrenciDersler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for OgrenciDers
            modelBuilder.Entity<OgrenciDers>()
                .HasKey(od => new { od.ogrenciID, od.dersID });

            // Configure relationships
            modelBuilder.Entity<OgrenciDers>()
                .HasOne(od => od.Ogrenci)
                .WithMany(o => o.OgrenciDersler)
                .HasForeignKey(od => od.ogrenciID);

            modelBuilder.Entity<OgrenciDers>()
                .HasOne(od => od.Ders)
                .WithMany(d => d.OgrenciDersler)
                .HasForeignKey(od => od.dersID);

            // Seed data
            modelBuilder.Entity<Fakulte>().HasData(
                new Fakulte { fakulteID = 1, fakulteAd = "Mühendislik Fakültesi" },
                new Fakulte { fakulteID = 2, fakulteAd = "Fen Edebiyat Fakültesi" }
            );

            modelBuilder.Entity<Bolum>().HasData(
                new Bolum { bolumID = 1, bolumAd = "Bilgisayar Mühendisliği", fakulteID = 1 },
                new Bolum { bolumID = 2, bolumAd = "Elektrik-Elektronik Mühendisliği", fakulteID = 1 },
                new Bolum { bolumID = 3, bolumAd = "Matematik", fakulteID = 2 }
            );

            modelBuilder.Entity<Ders>().HasData(
                new Ders { dersID = 1, dersKodu = "BIL101", dersAd = "Programlama Temelleri", kredi = 5 },
                new Ders { dersID = 2, dersKodu = "BIL102", dersAd = "Veri Tabanı Yönetimi", kredi = 4 },
                new Ders { dersID = 3, dersKodu = "MAT101", dersAd = "Matematik I", kredi = 6 }
            );
        }
    }
}
