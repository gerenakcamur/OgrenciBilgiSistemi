using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using System.Diagnostics;

namespace OgrenciBilgiSistemi.Controllers
{
    public class DersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DersController> _logger;

        public DersController(ApplicationDbContext context, ILogger<DersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dersler.ToListAsync());
        }

        // GET: Ders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ders = await _context.Dersler
                .FirstOrDefaultAsync(m => m.dersID == id);
            if (ders == null)
            {
                return NotFound();
            }

            return View(ders);
        }

        // GET: Ders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("dersKodu,dersAd,kredi")] Ders ders)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Ders ekleme iþlemi baþlatýldý: {DersKodu}, {DersAd}", ders.dersKodu, ders.dersAd);

                    // dersID'yi bind etmiyoruz, veritabaný otomatik atayacak
                    _context.Add(ders);

                    var result = await _context.SaveChangesAsync();
                    _logger.LogInformation("SaveChangesAsync sonucu: {Result} satýr etkilendi", result);

                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Ders baþarýyla eklendi.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning("Ders eklenemedi: SaveChangesAsync 0 satýr etkiledi");
                        ModelState.AddModelError("", "Veritabanýna kayýt yapýlamadý.");
                    }
                }
                else
                {
                    _logger.LogWarning("Model doðrulama hatasý");
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogWarning("Model hatasý: {ErrorMessage}", error.ErrorMessage);
                        }
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Veritabaný güncelleme hatasý: {Message}", ex.InnerException?.Message ?? ex.Message);
                ModelState.AddModelError("", $"Veritabaný hatasý: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ders eklenirken hata oluþtu: {Message}", ex.Message);
                ModelState.AddModelError("", $"Ders eklenirken bir hata oluþtu: {ex.Message}");
            }

            return View(ders);
        }

        // GET: Ders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ders = await _context.Dersler.FindAsync(id);
            if (ders == null)
            {
                return NotFound();
            }
            return View(ders);
        }

        // POST: Ders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("dersID,dersKodu,dersAd,kredi")] Ders ders)
        {
            if (id != ders.dersID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ders);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ders baþarýyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!DersExists(ders.dersID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Ders güncellenirken hata oluþtu: {Message}", ex.Message);
                        ModelState.AddModelError("", $"Güncelleme hatasý: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ders güncellenirken hata oluþtu: {Message}", ex.Message);
                    ModelState.AddModelError("", $"Ders güncellenirken bir hata oluþtu: {ex.Message}");
                }
            }
            return View(ders);
        }

        // GET: Ders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ders = await _context.Dersler
                .FirstOrDefaultAsync(m => m.dersID == id);
            if (ders == null)
            {
                return NotFound();
            }

            return View(ders);
        }

        // POST: Ders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ders = await _context.Dersler.FindAsync(id);
                if (ders != null)
                {
                    _context.Dersler.Remove(ders);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ders baþarýyla silindi.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ders silinirken veritabaný hatasý: {Message}", ex.InnerException?.Message ?? ex.Message);
                TempData["ErrorMessage"] = $"Ders silinemedi: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ders silinirken hata: {Message}", ex.Message);
                TempData["ErrorMessage"] = $"Ders silinemedi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool DersExists(int id)
        {
            return _context.Dersler.Any(e => e.dersID == id);
        }
    }
}
