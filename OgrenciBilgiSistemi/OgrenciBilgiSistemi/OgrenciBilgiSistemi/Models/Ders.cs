using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    [Table("tDers")]
    public class Ders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int dersID { get; set; }

        [Required(ErrorMessage = "Ders kodu zorunludur")]
        [StringLength(10)]
        [Display(Name = "Ders Kodu")]
        public string dersKodu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ders adı zorunludur")]
        [StringLength(100)]
        [Display(Name = "Ders Adı")]
        public string dersAd { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Kredi 1-10 arasında olmalıdır")]
        [Display(Name = "Kredi")]
        public int kredi { get; set; }

        // Navigation property - null olmaması için initialize ediyoruz
        public virtual ICollection<OgrenciDers> OgrenciDersler { get; set; } = new List<OgrenciDers>();
    }
}
