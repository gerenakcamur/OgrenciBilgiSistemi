using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    [Table("tOgrenciDers")]
    public class OgrenciDers
    {
        [Key, Column(Order = 0)]
        public int ogrenciID { get; set; }

        [Key, Column(Order = 1)]
        public int dersID { get; set; }

        [Required(ErrorMessage = "Yıl zorunludur")]
        [Display(Name = "Yıl")]
        public string yil { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yarıyıl zorunludur")]
        [Display(Name = "Yarıyıl")]
        public string yariyil { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Vize notu 0-100 arasında olmalıdır")]
        [Display(Name = "Vize")]
        public int? vize { get; set; }

        [Range(0, 100, ErrorMessage = "Final notu 0-100 arasında olmalıdır")]
        [Display(Name = "Final")]
        public int? final { get; set; }

        // Navigation properties
        [ForeignKey("ogrenciID")]
        public virtual Ogrenci Ogrenci { get; set; } = null!;

        [ForeignKey("dersID")]
        public virtual Ders Ders { get; set; } = null!;

        [NotMapped]
        [Display(Name = "Ortalama")]
        public double? Ortalama => vize.HasValue && final.HasValue ? (vize.Value * 0.4) + (final.Value * 0.6) : null;
    }
}
