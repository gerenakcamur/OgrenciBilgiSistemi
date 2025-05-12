using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Controllers
{
    public class BolumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BolumController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bolum
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bolumler.Include(b => b.Fakulte);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bolum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var bolum = await _context.Bolumler
                .Include(b => b.Fakulte)
                .Include(b => b.Ogrenciler) // Ogrenciler navigasyon �zelli�ini y�kle
                .FirstOrDefaultAsync(m => m.bolumID == id);

            if (bolum == null)
                return NotFound();

            return View(bolum);
        }

        // GET: Bolum/Create
        public IActionResult Create()
        {
            if (!_context.Fakulteler.Any())
            {
                ViewBag.ErrorMessage = "Fak�lte bulunamad�. L�tfen �nce bir fak�lte ekleyin.";
            }
            ViewData["fakulteID"] = new SelectList(_context.Fakulteler, "fakulteID", "fakulteAd");
            return View();
        }

        // POST: Bolum/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("bolumAd,fakulteID")] Bolum bolum)
        {
            ModelState.Remove("Fakulte");
            ModelState.Remove("Ogrenciler");

            if (ModelState.IsValid)
            {
                try
                {
                    bolum.Ogrenciler = new List<Ogrenci>();
                    _context.Bolumler.Add(bolum);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"B�l�m eklenirken bir hata olu�tu: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ModelState.AddModelError("", "Formda hatalar var: " + string.Join(", ", errors));
            }

            ViewData["fakulteID"] = new SelectList(_context.Fakulteler, "fakulteID", "fakulteAd", bolum.fakulteID);
            return View(bolum);
        }

        // GET: Bolum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var bolum = await _context.Bolumler.FindAsync(id);
            if (bolum == null)
                return NotFound();

            ViewData["fakulteID"] = new SelectList(_context.Fakulteler, "fakulteID", "fakulteAd", bolum.fakulteID);
            return View(bolum);
        }

        // POST: Bolum/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("bolumID,bolumAd,fakulteID")] Bolum bolum)
        {
            if (id != bolum.bolumID)
                return NotFound();

            ModelState.Remove("Fakulte");
            ModelState.Remove("Ogrenciler");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bolum);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BolumExists(bolum.bolumID))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"B�l�m g�ncellenirken bir hata olu�tu: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ModelState.AddModelError("", "Formda hatalar var: " + string.Join(", ", errors));
            }

            ViewData["fakulteID"] = new SelectList(_context.Fakulteler, "fakulteID", "fakulteAd", bolum.fakulteID);
            return View(bolum);
        }

        // GET: Bolum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var bolum = await _context.Bolumler
                .Include(b => b.Fakulte)
                .Include(b => b.Ogrenciler)
                .FirstOrDefaultAsync(m => m.bolumID == id);

            if (bolum == null)
                return NotFound();

            return View(bolum);
        }

        // POST: Bolum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bolum = await _context.Bolumler
                .Include(b => b.Ogrenciler) // �lgili ��rencileri y�kle
                .FirstOrDefaultAsync(b => b.bolumID == id);

            if (bolum != null)
            {
                // B�l�me ba�l� ��rencileri sil (yabanc� anahtar k�s�tlamas�n� �nlemek i�in)
                if (bolum.Ogrenciler != null && bolum.Ogrenciler.Any())
                {
                    _context.Ogrenciler.RemoveRange(bolum.Ogrenciler);
                }

                _context.Bolumler.Remove(bolum);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BolumExists(int id)
        {
            return _context.Bolumler.Any(e => e.bolumID == id);
        }
    }
}