using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    [Table("tBolum")]
    public class Bolum
    {
        [Key]
        public int bolumID { get; set; }

        [Required(ErrorMessage = "Bölüm adı zorunludur")]
        [StringLength(100)]
        [Display(Name = "Bölüm Adı")]
        public string bolumAd { get; set; }

        [Required(ErrorMessage = "Fakülte seçimi zorunludur")]
        [Display(Name = "Fakülte")]
        public int fakulteID { get; set; }

        // Navigation properties
        [ForeignKey("fakulteID")]
        public virtual Fakulte Fakulte { get; set; }
        public virtual ICollection<Ogrenci> Ogrenciler { get; set; }
    }
}
