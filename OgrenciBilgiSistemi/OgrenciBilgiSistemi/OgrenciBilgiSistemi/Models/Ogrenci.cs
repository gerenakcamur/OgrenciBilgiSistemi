using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    [Table("tOgrenci")]
    public class Ogrenci
    {
        [Key]
        public int ogrenciID { get; set; }

        [Required(ErrorMessage = "Öğrenci adı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Adı")]
        public string ad { get; set; }

        [Required(ErrorMessage = "Öğrenci soyadı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Soyadı")]
        public string soyad { get; set; }

        [Required(ErrorMessage = "Bölüm seçimi zorunludur")]
        [Display(Name = "Bölüm")]
        public int bolumID { get; set; }

        // Navigation properties
        [ForeignKey("bolumID")]
        public virtual Bolum Bolum { get; set; } // Zorunlu değil, sadece bolumID zorunlu

        // OgrenciDersler koleksiyonu boş olabilir
        public virtual ICollection<OgrenciDers> OgrenciDersler { get; set; } = new List<OgrenciDers>();

        [NotMapped]
        [Display(Name = "Adı Soyadı")]
        public string TamAd => $"{ad} {soyad}";
    }
}