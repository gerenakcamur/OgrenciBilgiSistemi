using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Controllers
{
    public class FakulteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FakulteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fakulte
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fakulteler.ToListAsync());
        }

        // GET: Fakulte/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var fakulte = await _context.Fakulteler
                .Include(f => f.Bolumler) // Bolumler navigasyon �zelli�ini y�kle
                .FirstOrDefaultAsync(m => m.fakulteID == id);

            if (fakulte == null)
                return NotFound();

            return View(fakulte);
        }

        // GET: Fakulte/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fakulte/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("fakulteAd")] Fakulte fakulte) // fakulteID ba�lamadan ��kar
        {
            // Navigasyon �zelliklerini do�rulama d��� b�rak
            ModelState.Remove("Bolumler");

            if (ModelState.IsValid)
            {
                try
                {
                    // Bolumler koleksiyonunu bo� bir liste ile ba�lat
                    fakulte.Bolumler = new List<Bolum>();

                    _context.Fakulteler.Add(fakulte);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Fak�lte eklenirken bir hata olu�tu: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ModelState.AddModelError("", "Formda hatalar var: " + string.Join(", ", errors));
            }

            // Hata durumunda ViewBag'e mesaj ekle
            ViewBag.ErrorMessage = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault();
            return View(fakulte);
        }

        // GET: Fakulte/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var fakulte = await _context.Fakulteler.FindAsync(id);
            if (fakulte == null)
                return NotFound();

            return View(fakulte);
        }

        // POST: Fakulte/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("fakulteID,fakulteAd")] Fakulte fakulte)
        {
            if (id != fakulte.fakulteID)
                return NotFound();

            // Navigasyon �zelliklerini do�rulama d��� b�rak
            ModelState.Remove("Bolumler");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fakulte);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FakulteExists(fakulte.fakulteID))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Fak�lte g�ncellenirken bir hata olu�tu: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ModelState.AddModelError("", "Formda hatalar var: " + string.Join(", ", errors));
            }

            return View(fakulte);
        }

        // GET: Fakulte/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var fakulte = await _context.Fakulteler
                .Include(f => f.Bolumler)
                .FirstOrDefaultAsync(m => m.fakulteID == id);

            if (fakulte == null)
                return NotFound();

            return View(fakulte);
        }

        // POST: Fakulte/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fakulte = await _context.Fakulteler
                .Include(f => f.Bolumler) // �lgili b�l�mleri y�kle
                .FirstOrDefaultAsync(f => f.fakulteID == id);

            if (fakulte != null)
            {
                // Fak�lteye ba�l� b�l�mleri sil (yabanc� anahtar k�s�tlamas�n� �nlemek i�in)
                if (fakulte.Bolumler != null && fakulte.Bolumler.Any())
                {
                    _context.Bolumler.RemoveRange(fakulte.Bolumler);
                }

                _context.Fakulteler.Remove(fakulte);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FakulteExists(int id)
        {
            return _context.Fakulteler.Any(e => e.fakulteID == id);
        }
    }
}