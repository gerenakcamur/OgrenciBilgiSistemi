using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    [Table("tFakulte")]
    public class Fakulte
    {
        [Key]
        public int fakulteID { get; set; }

        [Required(ErrorMessage = "Fakülte adı zorunludur")]
        [StringLength(100)]
        [Display(Name = "Fakülte Adı")]
        public string fakulteAd { get; set; }

        // Navigation property
        public virtual ICollection<Bolum> Bolumler { get; set; }
    }
}
