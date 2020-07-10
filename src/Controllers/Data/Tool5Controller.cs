using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.Data;

namespace BES.Controllers.Data
{
    public class Tool5Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Tool5Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tool5
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tool5.Include(t => t.School);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> SchoolList()
        {
            var applicationDbContext = _context.Tool5.Where(a => a.Quarter != 1).Include(t => t.School);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Tool5/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool5 = await _context.Tool5
                .Include(t => t.School)
                .FirstOrDefaultAsync(m => m.SchoolID == id);
            if (tool5 == null)
            {
                return NotFound();
            }

            return View(tool5);
        }

        // GET: Tool5/Create
        public IActionResult Create()
        {
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName");
            return View();
        }

        // POST: Tool5/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SchoolID,Quarter,Year,ProjectYear,Date,VisitorName,ReCollectData,Verified,VerifiedBy,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] Tool5 tool5)
        {
            //Identify Duplicate
            var t5 = _context.Tool5.Where(a => a.SchoolID == tool5.SchoolID && a.Quarter == tool5.Quarter);
            if (t5.Any())
            {
                ModelState.AddModelError("", "Recored Already Exist against this school in Quarter " + tool5.Quarter);
                return View(tool5);
            }
            if (ModelState.IsValid)
            {


                tool5.CreatedBy = User.Identity.Name;
                tool5.CreatedDate = DateTime.Now.Date;

                _context.Add(tool5);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Create", "Tool5Detail", new { id = tool5.SchoolID, q = tool5.Quarter });
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", tool5.SchoolID);
            return View(tool5);
        }

        // GET: Tool5/Edit/5
        public async Task<IActionResult> Edit(int? id, short q)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.SchoolName = await _context.Schools.Where(a => a.SchoolID == id).Select(a => a.SName).FirstOrDefaultAsync();

            var tool5 = await _context.Tool5.Where(a => a.SchoolID == id && a.Quarter == q).Include(a=>a.School) .FirstOrDefaultAsync();
            if (tool5 == null)
            {
                return NotFound();
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", tool5.SchoolID);
            return View(tool5);
        }

        // POST: Tool5/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SchoolID,Quarter,Year,ProjectYear,Date,VisitorName,ReCollectData,Verified,VerifiedBy,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] Tool5 tool5)
        {
            if (id != tool5.SchoolID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tool5);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Tool5Exists(tool5.SchoolID))
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
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", tool5.SchoolID);
            return View(tool5);
        }

        // GET: Tool5/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool5 = await _context.Tool5
                .Include(t => t.School)
                .FirstOrDefaultAsync(m => m.SchoolID == id);
            if (tool5 == null)
            {
                return NotFound();
            }

            return View(tool5);
        }

        // POST: Tool5/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tool5 = await _context.Tool5.FindAsync(id);
            _context.Tool5.Remove(tool5);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Tool5Exists(int id)
        {
            return _context.Tool5.Any(e => e.SchoolID == id);
        }
    }
}
