
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using K22C_Day6.Models;

public class genreController : Controller
{
    private readonly K22C_Day6Context _context;

    public genreController(K22C_Day6Context context)
    {
        _context = context;
    }

    // GET: GENRES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.genre.ToListAsync());
    }

    // GET: GENRES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var genre = await _context.genre
            .FirstOrDefaultAsync(m => m.Id == id);
        if (genre == null)
        {
            return NotFound();
        }

        return View(genre);
    }

    // GET: GENRES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: GENRES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] genre genre)
    {
        if (ModelState.IsValid)
        {
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(genre);
    }

    // GET: GENRES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var genre = await _context.genre.FindAsync(id);
        if (genre == null)
        {
            return NotFound();
        }
        return View(genre);
    }

    // POST: GENRES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name")] genre genre)
    {
        if (id != genre.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(genre);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(genre.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(genre);
    }

    // GET: GENRES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var genre = await _context.genre
            .FirstOrDefaultAsync(m => m.Id == id);
        if (genre == null)
        {
            return NotFound();
        }

        // Kiểm tra nếu thể loại này đang có phim
        bool hasMovies = await _context.movie.AnyAsync(m => m.GenreId == id);
        if (hasMovies)
        {
            ViewBag.ErrorMessage = "Thể loại này đang chứa các bộ phim, bạn không được phép xóa!";
        }

        return View(genre);
    }

    // POST: GENRES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        // Chặn xóa nếu trong movie còn dữ liệu của genre này
        bool hasMovies = await _context.movie.AnyAsync(m => m.GenreId == id);
        if (hasMovies)
        {
            TempData["ErrorMessage"] = "Không thể xóa thể loại này vì vẫn còn phim thuộc thể loại này!";
            return RedirectToAction(nameof(Index));
        }

        var genre = await _context.genre.FindAsync(id);
        if (genre != null)
        {
            _context.genre.Remove(genre);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool GenreExists(int? id)
    {
        return _context.genre.Any(e => e.Id == id);
    }
}
