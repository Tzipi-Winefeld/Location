using Location2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Location2.Controllers
{
    public class SettlementsController : Controller
    {
        public static string ConvertKeyboardInput(string input)
        {
            var hebrewToEnglish = new Dictionary<char, char>
            {
                {'א', 't'}, {'ב', 'c'}, {'ג', 'd'}, {'ד', 's'}, {'ה', 'v'}, {'ו', 'u'}, {'ז', 'z'},
                {'ח', 'j'}, {'ט', 'y'}, {'י', 'h'}, {'כ', 'f'}, {'ל', 'k'}, {'מ', 'n'}, {'נ', 'b'},
                {'ס', 'x'}, {'ע', 'g'}, {'פ', 'p'}, {'צ', 'm'}, {'ק', 'e'}, {'ר', 'r'}, {'ש', 'a'},
                {'ת', ','}, {'ף', 'p'}, {'ך', 'f'}, {'ץ', 'm'}, {'ם', 'n'}, {'ן', 'b'}, {' ', ' '}
            };

            var englishToHebrew = new Dictionary<char, char>
            {
                {'t', 'א'}, {'c', 'ב'}, {'d', 'ג'}, {'s', 'ד'}, {'v', 'ה'}, {'u', 'ו'}, {'z', 'ז'},
                {'j', 'ח'}, {'y', 'ט'}, {'h', 'י'}, {'f', 'כ'}, {'k', 'ל'}, {'n', 'מ'}, {'b', 'נ'},
                {'x', 'ס'}, {'g', 'ע'}, {'p', 'פ'}, {'m', 'צ'}, {'e', 'ק'}, {'r', 'ר'}, {'a', 'ש'},
                {',', 'ת'}, {' ', ' '}
            };

            var output = new StringBuilder();
            foreach (char c in input)
            {
                if (hebrewToEnglish.ContainsKey(c))
                {
                    output.Append(hebrewToEnglish[c]);
                }
                else if (englishToHebrew.ContainsKey(c))
                {
                    output.Append(englishToHebrew[c]);
                }
                else
                {
                    output.Append(c);
                }
            }

            return output.ToString();
        }

        public  bool sameName(string name)
        {
            if (_context.Settlements.Any(s => s.Name == name))
            {
                return true;
            }
            return false;
        }

        private readonly ApplicationDbContext _context;

        public SettlementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Settlements
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            //ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPageNumber"] = pageNumber ?? 1;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Name_asc" : (sortOrder == "Name_asc" ? "Name_desc" : "Name_asc");
            ViewData["SortIcon"] = sortOrder == "Name_asc" ? "▲" : "▼";

            var settlements = from s in _context.Settlements select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                string convertedSearchString = ConvertKeyboardInput(searchString);
                settlements = settlements.Where(s => s.Name.Contains(searchString) || s.Name.Contains(convertedSearchString));
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    settlements = settlements.OrderByDescending(s => s.Name);
                    break;
                default:
                    settlements = settlements.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Settlement>.CreateAsync(settlements.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Settlements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Settlements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Settlement settlement)
        {
            if (ModelState.IsValid)
            {
                //if (_context.Settlements.Any(s => s.Name == settlement.Name))
                //{
                //    ModelState.AddModelError("Name", "ישוב עם שם זה כבר קיים");
                //    return View(settlement);
                //}
                if (sameName(settlement.Name))
                {
                    ModelState.AddModelError("Name", "ישוב עם שם זה כבר קיים");
                    return View(settlement);
                }

                try
                {
                    _context.Add(settlement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "שגיאה בעת שמירת הנתונים במסד הנתונים. נסה שוב מאוחר יותר.");
                    return View(settlement);
                }
            }
            return View(settlement);
        }

        // POST: Settlements/Edit
        [HttpPost]
        public IActionResult Edit(int id, string name)
        {
            // בדיקה אם יש ישוב עם אותו שם
            //if (_context.Settlements.Any(s => s.Name == name && s.Id != id))
            //{
            //    return Json(new { success = false, message = "ישוב עם שם זה כבר קיים" });
            //}
            if(sameName(name))
            {
                return Json(new { success = false, message = "ישוב עם שם זה כבר קיים" });

            }

            var settlement = _context.Settlements.Find(id);
            if (settlement != null)
            {
                settlement.Name = name;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "שגיאה בשמירת הנתונים" });
        }


        // GET: Settlements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var settlement = await _context.Settlements.FirstOrDefaultAsync(m => m.Id == id);
            if (settlement == null)
            {
                return NotFound();
            }

            return View(settlement);
        }

        // POST: Settlements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var settlement = await _context.Settlements.FindAsync(id);
            _context.Settlements.Remove(settlement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SettlementExists(int id)
        {
            return _context.Settlements.Any(e => e.Id == id);
        }
    }
}

