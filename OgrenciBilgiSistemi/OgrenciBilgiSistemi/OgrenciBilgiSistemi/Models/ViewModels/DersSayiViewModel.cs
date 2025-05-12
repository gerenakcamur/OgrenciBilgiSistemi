namespace OgrenciBilgiSistemi.Models.ViewModels
{
    public class DersSayiViewModel
    {
        public int DersID { get; set; }
        public string DersKodu { get; set; } = string.Empty;
        public string DersAd { get; set; } = string.Empty;
        public int OgrenciSayisi { get; set; }
    }
}
