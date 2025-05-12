using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Models.ViewModels;
using System.Diagnostics;

namespace OgrenciBilgiSistemi.Controllers
{
    public class OgrenciDersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OgrenciDersController> _logger;

        public OgrenciDersController(ApplicationDbContext context, ILogger<OgrenciDersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: OgrenciDers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OgrenciDersler
                .Include(o => o.Ders)
                .Include(o => o.Ogrenci);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OgrenciDers/Create
        public IActionResult Create()
        {
            ViewBag.Ogrenciler = new SelectList(_context.Ogrenciler, "ogrenciID", "TamAd");
            ViewBag.Dersler = new SelectList(_context.Dersler, "dersID", "dersAd");
            ViewBag.Yillar = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" });
            ViewBag.Yariyillar = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" });

            return View();
        }

        // POST: OgrenciDers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ogrenciID, int dersID, string yil, string yariyil)
        {
            try
            {
                _logger.LogInformation("Öğrenci-Ders ekleme işlemi başlatıldı: Öğrenci ID: {OgrenciID}, Ders ID: {DersID}, Yıl: {Yil}, Yarıyıl: {Yariyil}",
                    ogrenciID, dersID, yil, yariyil);

                // Form verilerini kontrol et
                if (ogrenciID <= 0 || dersID <= 0 || string.IsNullOrEmpty(yil) || string.IsNullOrEmpty(yariyil))
                {
                    _logger.LogWarning("Form verileri geçersiz: Öğrenci ID: {OgrenciID}, Ders ID: {DersID}, Yıl: {Yil}, Yarıyıl: {Yariyil}",
                        ogrenciID, dersID, yil, yariyil);

                    if (ogrenciID <= 0)
                        ModelState.AddModelError("ogrenciID", "Öğrenci seçimi zorunludur.");

                    if (dersID <= 0)
                        ModelState.AddModelError("dersID", "Ders seçimi zorunludur.");

                    if (string.IsNullOrEmpty(yil))
                        ModelState.AddModelError("yil", "Yıl seçimi zorunludur.");

                    if (string.IsNullOrEmpty(yariyil))
                        ModelState.AddModelError("yariyil", "Yarıyıl seçimi zorunludur.");

                    ViewBag.Ogrenciler = new SelectList(_context.Ogrenciler, "ogrenciID", "TamAd", ogrenciID);
                    ViewBag.Dersler = new SelectList(_context.Dersler, "dersID", "dersAd", dersID);
                    ViewBag.Yillar = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
                    ViewBag.Yariyillar = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
                    return View();
                }

                // Check if the student already has this course in the same year and semester
                var exists = await _context.OgrenciDersler
                    .AnyAsync(od => od.ogrenciID == ogrenciID &&
                                   od.dersID == dersID &&
                                   od.yil == yil &&
                                   od.yariyil == yariyil);

                if (exists)
                {
                    _logger.LogWarning("Bu öğrenci bu dersi zaten almış: Öğrenci ID: {OgrenciID}, Ders ID: {DersID}, Yıl: {Yil}, Yarıyıl: {Yariyil}",
                        ogrenciID, dersID, yil, yariyil);

                    ModelState.AddModelError("", "Bu öğrenci bu dersi zaten almış!");
                    ViewBag.Ogrenciler = new SelectList(_context.Ogrenciler, "ogrenciID", "TamAd", ogrenciID);
                    ViewBag.Dersler = new SelectList(_context.Dersler, "dersID", "dersAd", dersID);
                    ViewBag.Yillar = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
                    ViewBag.Yariyillar = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
                    return View();
                }

                // Öğrenci ve Ders varlıklarını kontrol et
                var ogrenci = await _context.Ogrenciler.FindAsync(ogrenciID);
                var ders = await _context.Dersler.FindAsync(dersID);

                if (ogrenci == null || ders == null)
                {
                    _logger.LogWarning("Öğrenci veya ders bulunamadı: Öğrenci ID: {OgrenciID}, Ders ID: {DersID}",
                        ogrenciID, dersID);

                    if (ogrenci == null)
                        ModelState.AddModelError("ogrenciID", "Seçilen öğrenci bulunamadı.");

                    if (ders == null)
                        ModelState.AddModelError("dersID", "Seçilen ders bulunamadı.");

                    ViewBag.Ogrenciler = new SelectList(_context.Ogrenciler, "ogrenciID", "TamAd", ogrenciID);
                    ViewBag.Dersler = new SelectList(_context.Dersler, "dersID", "dersAd", dersID);
                    ViewBag.Yillar = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
                    ViewBag.Yariyillar = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
                    return View();
                }

                // Yeni OgrenciDers nesnesi oluştur
                var ogrenciDers = new OgrenciDers
                {
                    ogrenciID = ogrenciID,
                    dersID = dersID,
                    yil = yil,
                    yariyil = yariyil,
                    vize = null,
                    final = null
                };

                _context.Add(ogrenciDers);
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync sonucu: {Result} satır etkilendi", result);

                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Öğrenciye ders başarıyla atandı.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("Öğrenci-Ders eklenemedi: SaveChangesAsync 0 satır etkiledi");
                    ModelState.AddModelError("", "Veritabanına kayıt yapılamadı.");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Veritabanı güncelleme hatası: {Message}", ex.InnerException?.Message ?? ex.Message);
                ModelState.AddModelError("", $"Veritabanı hatası: {ex.InnerException?.Message ?? ex.Message}");
                TempData["ErrorMessage"] = $"Veritabanı hatası: {ex.InnerException?.Message ?? ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Öğrenci-Ders eklenirken hata oluştu: {Message}", ex.Message);
                ModelState.AddModelError("", $"Öğrenci-Ders eklenirken bir hata oluştu: {ex.Message}");
                TempData["ErrorMessage"] = $"Öğrenci-Ders eklenirken bir hata oluştu: {ex.Message}";
            }

            ViewBag.Ogrenciler = new SelectList(_context.Ogrenciler, "ogrenciID", "TamAd", ogrenciID);
            ViewBag.Dersler = new SelectList(_context.Dersler, "dersID", "dersAd", dersID);
            ViewBag.Yillar = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
            ViewBag.Yariyillar = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
            return View();
        }

        // Diğer metodlar aynı kalacak...

        // GET: OgrenciDers/Edit/5/10
        public async Task<IActionResult> Edit(int? ogrenciId, int? dersId)
        {
            if (ogrenciId == null || dersId == null)
            {
                return NotFound();
            }

            var ogrenciDers = await _context.OgrenciDersler
                .FirstOrDefaultAsync(m => m.ogrenciID == ogrenciId && m.dersID == dersId);
            if (ogrenciDers == null)
            {
                return NotFound();
            }

            ViewBag.Ogrenciler = new SelectList(_context.Ogrenciler, "ogrenciID", "TamAd", ogrenciDers.ogrenciID);
            ViewBag.Dersler = new SelectList(_context.Dersler, "dersID", "dersAd", ogrenciDers.dersID);
            ViewBag.Yillar = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, ogrenciDers.yil);
            ViewBag.Yariyillar = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, ogrenciDers.yariyil);

            return View(ogrenciDers);
        }

        // POST: OgrenciDers/Edit/5/10
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ogrenciId, int dersId, string yil, string yariyil, int? vize, int? final)
        {
            var ogrenciDers = await _context.OgrenciDersler
                .FirstOrDefaultAsync(m => m.ogrenciID == ogrenciId && m.dersID == dersId);

            if (ogrenciDers == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ogrenciDers.yil = yil;
                    ogrenciDers.yariyil = yariyil;
                    ogrenciDers.vize = vize;
                    ogrenciDers.final = final;

                    _context.Update(ogrenciDers);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Öğrenci-Ders ilişkisi başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!OgrenciDersExists(ogrenciId, dersId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Öğrenci-Ders güncellenirken hata oluştu: {Message}", ex.Message);
                        ModelState.AddModelError("", $"Güncelleme hatası: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Öğrenci-Ders güncellenirken hata oluştu: {Message}", ex.Message);
                    ModelState.AddModelError("", $"Öğrenci-Ders güncellenirken bir hata oluştu: {ex.Message}");
                }
            }

            ViewBag.Ogrenciler = new SelectList(_context.Ogrenciler, "ogrenciID", "TamAd", ogrenciId);
            ViewBag.Dersler = new SelectList(_context.Dersler, "dersID", "dersAd", dersId);
            ViewBag.Yillar = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
            ViewBag.Yariyillar = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
            return View(ogrenciDers);
        }

        // GET: OgrenciDers/Delete/5/10
        public async Task<IActionResult> Delete(int? ogrenciId, int? dersId)
        {
            if (ogrenciId == null || dersId == null)
            {
                return NotFound();
            }

            var ogrenciDers = await _context.OgrenciDersler
                .Include(o => o.Ders)
                .Include(o => o.Ogrenci)
                .FirstOrDefaultAsync(m => m.ogrenciID == ogrenciId && m.dersID == dersId);
            if (ogrenciDers == null)
            {
                return NotFound();
            }

            return View(ogrenciDers);
        }

        // POST: OgrenciDers/Delete/5/10
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ogrenciId, int dersId)
        {
            try
            {
                var ogrenciDers = await _context.OgrenciDersler.FindAsync(ogrenciId, dersId);
                if (ogrenciDers != null)
                {
                    _context.OgrenciDersler.Remove(ogrenciDers);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Öğrenci-Ders ilişkisi başarıyla silindi.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Öğrenci-Ders silinirken veritabanı hatası: {Message}", ex.InnerException?.Message ?? ex.Message);
                TempData["ErrorMessage"] = $"Öğrenci-Ders ilişkisi silinemedi: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Öğrenci-Ders silinirken hata: {Message}", ex.Message);
                TempData["ErrorMessage"] = $"Öğrenci-Ders ilişkisi silinemedi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: OgrenciDers/OgrenciDersler
        public IActionResult OgrenciDersler()
        {
            return View();
        }

        // POST: OgrenciDers/OgrenciDersler
        [HttpPost]
        public async Task<IActionResult> OgrenciDersler(int ogrenciID)
        {
            if (ogrenciID <= 0)
            {
                ModelState.AddModelError("", "Geçerli bir öğrenci numarası giriniz.");
                return View();
            }

            var ogrenci = await _context.Ogrenciler
                .Include(o => o.Bolum)
                .FirstOrDefaultAsync(m => m.ogrenciID == ogrenciID);

            if (ogrenci == null)
            {
                ModelState.AddModelError("", "Öğrenci bulunamadı.");
                return View();
            }

            var dersler = await _context.OgrenciDersler
                .Include(od => od.Ders)
                .Where(od => od.ogrenciID == ogrenciID)
                .ToListAsync();

            ViewData["Ogrenci"] = ogrenci;
            ViewData["Dersler"] = dersler;
            return View();
        }

        // GET: OgrenciDers/DersSayilari
        public IActionResult DersSayilari()
        {
            ViewData["Yillar"] = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" });
            ViewData["Yariyillar"] = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" });
            return View();
        }

        // POST: OgrenciDers/DersSayilari
        [HttpPost]
        public async Task<IActionResult> DersSayilari(string yil, string yariyil)
        {
            if (string.IsNullOrEmpty(yil) || string.IsNullOrEmpty(yariyil))
            {
                ViewData["Yillar"] = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
                ViewData["Yariyillar"] = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
                ModelState.AddModelError("", "Yıl ve yarıyıl seçimi zorunludur.");
                return View();
            }

            var dersSayilari = await _context.OgrenciDersler
                .Where(od => od.yil == yil && od.yariyil == yariyil)
                .GroupBy(od => new { od.dersID, od.Ders.dersKodu, od.Ders.dersAd })
                .Select(g => new DersSayiViewModel
                {
                    DersID = g.Key.dersID,
                    DersKodu = g.Key.dersKodu,
                    DersAd = g.Key.dersAd,
                    OgrenciSayisi = g.Count()
                })
                .ToListAsync();

            ViewData["Yillar"] = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
            ViewData["Yariyillar"] = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
            ViewData["Yil"] = yil;
            ViewData["Yariyil"] = yariyil;
            return View(dersSayilari);
        }

        // GET: OgrenciDers/DersOgrencileri
        public IActionResult DersOgrencileri()
        {
            ViewData["Dersler"] = new SelectList(_context.Dersler, "dersID", "dersAd");
            ViewData["Yillar"] = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" });
            ViewData["Yariyillar"] = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" });
            return View();
        }

        // POST: OgrenciDers/DersOgrencileri
        [HttpPost]
        public async Task<IActionResult> DersOgrencileri(int dersID, string yil, string yariyil)
        {
            if (dersID <= 0 || string.IsNullOrEmpty(yil) || string.IsNullOrEmpty(yariyil))
            {
                ViewData["Dersler"] = new SelectList(_context.Dersler, "dersID", "dersAd", dersID);
                ViewData["Yillar"] = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
                ViewData["Yariyillar"] = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
                ModelState.AddModelError("", "Ders, yıl ve yarıyıl seçimi zorunludur.");
                return View();
            }

            var ders = await _context.Dersler
                .FirstOrDefaultAsync(d => d.dersID == dersID);
            if (ders == null)
            {
                return NotFound();
            }

            var ogrenciler = await _context.OgrenciDersler
                .Include(od => od.Ogrenci)
                .Where(od => od.dersID == dersID && od.yil == yil && od.yariyil == yariyil)
                .Select(od => new OgrenciNotViewModel
                {
                    OgrenciID = od.ogrenciID,
                    OgrenciAdSoyad = od.Ogrenci.TamAd,
                    Vize = od.vize,
                    Final = od.final
                })
                .ToListAsync();

            var viewModel = new DersOgrenciViewModel
            {
                DersID = dersID,
                DersAd = ders.dersAd,
                Yil = yil,
                Yariyil = yariyil,
                OgrenciNotlar = ogrenciler
            };

            ViewData["Dersler"] = new SelectList(_context.Dersler, "dersID", "dersAd", dersID);
            ViewData["Yillar"] = new SelectList(new List<string> { "2020-2021", "2021-2022", "2022-2023", "2023-2024", "2024-2025" }, yil);
            ViewData["Yariyillar"] = new SelectList(new List<string> { "Güz", "Bahar", "Yaz" }, yariyil);
            return View("DersOgrencileriSonuc", viewModel);
        }

        // POST: OgrenciDers/NotKaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotKaydet(DersOgrenciViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var ogrenciNot in viewModel.OgrenciNotlar)
                    {
                        var ogrenciDers = await _context.OgrenciDersler
                            .FirstOrDefaultAsync(od => od.ogrenciID == ogrenciNot.OgrenciID &&
                                                     od.dersID == viewModel.DersID &&
                                                     od.yil == viewModel.Yil &&
                                                     od.yariyil == viewModel.Yariyil);

                        if (ogrenciDers != null)
                        {
                            ogrenciDers.vize = ogrenciNot.Vize;
                            ogrenciDers.final = ogrenciNot.Final;
                            _context.Update(ogrenciDers);
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Notlar başarıyla kaydedildi.";
                    return RedirectToAction(nameof(DersOgrencileri));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Not kaydedilirken hata oluştu: {Message}", ex.Message);
                    ModelState.AddModelError("", $"Not kaydedilirken bir hata oluştu: {ex.Message}");
                }
            }

            return View("DersOgrencileriSonuc", viewModel);
        }

        private bool OgrenciDersExists(int ogrenciId, int dersId)
        {
            return _context.OgrenciDersler.Any(e => e.ogrenciID == ogrenciId && e.dersID == dersId);
        }
    }
}
