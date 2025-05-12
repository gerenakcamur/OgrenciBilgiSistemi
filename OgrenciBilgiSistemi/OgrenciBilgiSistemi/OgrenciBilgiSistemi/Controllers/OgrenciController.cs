using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Data;

namespace OgrenciBilgiSistemi.Controllers
{
    public class OgrenciController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OgrenciController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ogrenci/Index
        public async Task<IActionResult> Index()
        {
            var ogrenciler = await _context.Ogrenciler
                .Include(o => o.Bolum)
                .ThenInclude(b => b.Fakulte)
                .ToListAsync();
            return View(ogrenciler);
        }

        // GET: Ogrenci/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ogrenci = await _context.Ogrenciler
                .Include(o => o.Bolum)
                .ThenInclude(b => b.Fakulte)
                .FirstOrDefaultAsync(m => m.ogrenciID == id);

            if (ogrenci == null)
                return NotFound();

            return View(ogrenci);
        }

        // GET: Ogrenci/Create
        public IActionResult Create()
        {
            if (!_context.Bolumler.Any())
            {
                ViewBag.ErrorMessage = "Bölüm bulunamadý. Lütfen önce bir bölüm ekleyin.";
            }
            ViewBag.Bolumler = new SelectList(_context.Bolumler, "bolumID", "bolumAd");
            return View();
        }

        // POST: Ogrenci/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ad,soyad,bolumID")] Ogrenci ogrenci)
        {
            // Navigasyon özelliklerini doðrulama dýþý býrak
            ModelState.Remove("Bolum");
            ModelState.Remove("OgrenciDersler");

            if (ModelState.IsValid)
            {
                try
                {
                    // OgrenciDersler koleksiyonunu boþ bir liste ile baþlat
                    ogrenci.OgrenciDersler = new List<OgrenciDers>();

                    _context.Ogrenciler.Add(ogrenci);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Öðrenci eklenirken bir hata oluþtu: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ModelState.AddModelError("", "Formda hatalar var: " + string.Join(", ", errors));
            }

            ViewBag.Bolumler = new SelectList(_context.Bolumler, "bolumID", "bolumAd", ogrenci.bolumID);
            return View(ogrenci);
        }

        // GET: Ogrenci/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ogrenci = await _context.Ogrenciler.FindAsync(id);
            if (ogrenci == null)
                return NotFound();

            ViewBag.Bolumler = new SelectList(_context.Bolumler, "bolumID", "bolumAd", ogrenci.bolumID);
            return View(ogrenci);
        }

        // POST: Ogrenci/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ogrenciID,ad,soyad,bolumID")] Ogrenci ogrenci)
        {
            if (id != ogrenci.ogrenciID)
                return NotFound();

            // Navigasyon özelliklerini doðrulama dýþý býrak
            ModelState.Remove("Bolum");
            ModelState.Remove("OgrenciDersler");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ogrenci);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Ogrenciler.Any(e => e.ogrenciID == ogrenci.ogrenciID))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Öðrenci güncellenirken bir hata oluþtu: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ModelState.AddModelError("", "Formda hatalar var: " + string.Join(", ", errors));
            }

            ViewBag.Bolumler = new SelectList(_context.Bolumler, "bolumID", "bolumAd", ogrenci.bolumID);
            return View(ogrenci);
        }

        // GET: Ogrenci/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ogrenci = await _context.Ogrenciler
                .Include(o => o.Bolum)
                .ThenInclude(b => b.Fakulte)
                .FirstOrDefaultAsync(m => m.ogrenciID == id);

            if (ogrenci == null)
                return NotFound();

            return View(ogrenci);
        }

        // POST: Ogrenci/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ogrenci = await _context.Ogrenciler.FindAsync(id);
            if (ogrenci != null)
            {
                _context.Ogrenciler.Remove(ogrenci);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}