using Microsoft.AspNetCore.Mvc.Rendering;

namespace OgrenciBilgiSistemi.Models.ViewModels
{
    public class OgrenciDersViewModel
    {
        public OgrenciDers OgrenciDers { get; set; } = new OgrenciDers();
        public SelectList Ogrenciler { get; set; } = new SelectList(Enumerable.Empty<object>());
        public SelectList Dersler { get; set; } = new SelectList(Enumerable.Empty<object>());
        public SelectList Yillar { get; set; } = new SelectList(Enumerable.Empty<object>());
        public SelectList Yariyillar { get; set; } = new SelectList(Enumerable.Empty<object>());
    }
}
