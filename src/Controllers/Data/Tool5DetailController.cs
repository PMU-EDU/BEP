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
    public class Tool5DetailController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Tool5DetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tool5Detail
        public async Task<IActionResult> Index(int id, short q)
        {
            var applicationDbContext = _context.Tool5Detail.Where(a=>a.SchoolID==id && a.Quarter==q ).Include(t => t.School);
            return PartialView(await applicationDbContext.ToListAsync());
        }

        // GET: Tool5Detail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool5Detail = await _context.Tool5Detail
                .Include(t => t.School)
               // .Include(t => t.SchoolClass)
                .FirstOrDefaultAsync(m => m.SchoolID == id);
            if (tool5Detail == null)
            {
                return NotFound();
            }

            return View(tool5Detail);
        }

        // GET: Tool5Detail/Create
        public async Task<IActionResult> Create(int id, short q)
        {
            ViewBag.SchoolName = await _context.Schools.Where(a => a.SchoolID == id).Select(a => a.SName).FirstOrDefaultAsync();

            Tool5Detail tool5Detail = new Tool5Detail();
            tool5Detail.SchoolID = id;
            tool5Detail.Quarter = q;
            tool5Detail.ClassID = Convert.ToInt16( _context.Tool5Detail.Count(a => a.SchoolID == id && a.Quarter == q));

            ViewBag.MaxClass = 5;
            switch (_context.Schools.Where(a => a.SchoolID == id).Select(a => a.SLevel).FirstOrDefault())
            {
                case 2:
                    ViewBag.MaxClass = 8;
                    break;
                case 3:
                    ViewBag.MaxClass = 10;
                    break;
                default:
                    ViewBag.MaxClass = 5;
                    break;
            }

            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName");
            ViewData["ClassID"] = new SelectList(_context.SchoolClasses, "ClassID", "ClassID");
            return View(tool5Detail);
        }

        // POST: Tool5Detail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SchoolID,Quarter,ClassID,NewEnrolltGirls,NewEnrollBoys,AttendRegGirls,AttendRegBoys,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] Tool5Detail tool5Detail)
        {
            if (ModelState.IsValid)
            {
                //tool5Detail.ClassID = Convert.ToInt16(_context.Tool5Detail.Count(a => a.SchoolID == tool5Detail.SchoolID && a.Quarter == tool5Detail.Quarter));
                tool5Detail.CreatedBy = User.Identity.Name;
                tool5Detail.CreatedDate = DateTime.Now;
                _context.Add(tool5Detail);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Tool5Detail", new { id = tool5Detail.SchoolID, q = tool5Detail.Quarter });
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", tool5Detail.SchoolID);
            ViewData["ClassID"] = new SelectList(_context.SchoolClasses, "ClassID", "ClassID", tool5Detail.ClassID);
            return View(tool5Detail);
        }

        // GET: Tool5Detail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool5Detail = await _context.Tool5Detail.FindAsync(id);
            if (tool5Detail == null)
            {
                return NotFound();
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", tool5Detail.SchoolID);
            ViewData["ClassID"] = new SelectList(_context.SchoolClasses, "ClassID", "ClassID", tool5Detail.ClassID);
            return View(tool5Detail);
        }

        // POST: Tool5Detail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SchoolID,Quarter,ClassID,NewEnrolltGirls,NewEnrollBoys,AttendRegGirls,AttendRegBoys,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] Tool5Detail tool5Detail)
        {
            if (id != tool5Detail.SchoolID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tool5Detail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Tool5DetailExists(tool5Detail.SchoolID))
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
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", tool5Detail.SchoolID);
            ViewData["ClassID"] = new SelectList(_context.SchoolClasses, "ClassID", "ClassID", tool5Detail.ClassID);
            return View(tool5Detail);
        }

        // GET: Tool5Detail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool5Detail = await _context.Tool5Detail
                .Include(t => t.School)
               // .Include(t => t.SchoolClass)
                .FirstOrDefaultAsync(m => m.SchoolID == id);
            if (tool5Detail == null)
            {
                return NotFound();
            }

            return View(tool5Detail);
        }

        // POST: Tool5Detail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tool5Detail = await _context.Tool5Detail.FindAsync(id);
            _context.Tool5Detail.Remove(tool5Detail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Tool5DetailExists(int id)
        {
            return _context.Tool5Detail.Any(e => e.SchoolID == id);
        }
    }
}
