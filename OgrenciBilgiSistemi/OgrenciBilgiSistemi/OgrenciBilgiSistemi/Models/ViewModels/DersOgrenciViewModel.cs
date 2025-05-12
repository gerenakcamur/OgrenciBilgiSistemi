namespace OgrenciBilgiSistemi.Models.ViewModels
{
    public class DersOgrenciViewModel
    {
        public int DersID { get; set; }
        public string DersAd { get; set; } = string.Empty;
        public string Yil { get; set; } = string.Empty;
        public string Yariyil { get; set; } = string.Empty;
        public List<OgrenciNotViewModel> OgrenciNotlar { get; set; } = new List<OgrenciNotViewModel>();
    }

    public class OgrenciNotViewModel
    {
        public int OgrenciID { get; set; }
        public string OgrenciAdSoyad { get; set; } = string.Empty;
        public int? Vize { get; set; }
        public int? Final { get; set; }
    }
}
