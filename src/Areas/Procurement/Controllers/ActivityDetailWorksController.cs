using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Areas.Procurement.Models;
using BES.Data;
using BES.Models.Data;

namespace BES.Areas.Procurement.Controllers
{
    [Area("Procurement")]
    public class ActivityDetailWorksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivityDetailWorksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Procurement/ActivityDetailWorks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ActivityDetailWork.Include(a => a.Activity);
            return View(await applicationDbContext.ToListAsync());
        }
        public ActionResult AssignWorkActivity(int id)
        {            
            var applicationDbContext = _context.WorkSchool.Include(a=>a.ActivityDetailWork.Activity).Include(a=>a.School.UC.Tehsil.District).Where(a => a.ActivityDetailWork.ActivityID == id);
            
            return PartialView(applicationDbContext);
        }
        // GET: Procurement/ActivityDetailWorks/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityDetailWork = await _context.ActivityDetailWork
                .Include(a => a.Activity)
                .FirstOrDefaultAsync(m => m.ActivityDetailWorkID == id);
            if (activityDetailWork == null)
            {
                return NotFound();
            }

            return View(activityDetailWork);
        }

        // GET: Procurement/ActivityDetailWorks/Create
        public IActionResult Create()
        {
            ViewData["ActivityID"] = new SelectList(_context.Activity, "ActivityID", "Description");
            return View();
        }

        // POST: Procurement/ActivityDetailWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActivityDetailWorkID,ActivityID,TotalSchool,ContractorID,ExpiryDate,Attachment")] ActivityDetailWork activityDetailWork)
        {
            if (ModelState.IsValid)
            {
                _context.Add(activityDetailWork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityID"] = new SelectList(_context.Activity, "ActivityID", "Description", activityDetailWork.ActivityID);
            return View(activityDetailWork);
        }

        // GET: Procurement/ActivityDetailWorks/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityDetailWork = await _context.ActivityDetailWork.FindAsync(id);
            if (activityDetailWork == null)
            {
                return NotFound();
            }
            ViewData["ActivityID"] = new SelectList(_context.Activity, "ActivityID", "Description", activityDetailWork.ActivityID);
            return View(activityDetailWork);
        }

        // POST: Procurement/ActivityDetailWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("ActivityDetailWorkID,ActivityID,TotalSchool,ContractorID,ExpiryDate,Attachment")] ActivityDetailWork activityDetailWork)
        {
            if (id != activityDetailWork.ActivityDetailWorkID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activityDetailWork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityDetailWorkExists(activityDetailWork.ActivityDetailWorkID))
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
            ViewData["ActivityID"] = new SelectList(_context.Activity, "ActivityID", "Description", activityDetailWork.ActivityID);
            return View(activityDetailWork);
        }

        // GET: Procurement/ActivityDetailWorks/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityDetailWork = await _context.ActivityDetailWork
                .Include(a => a.Activity)
                .FirstOrDefaultAsync(m => m.ActivityDetailWorkID == id);
            if (activityDetailWork == null)
            {
                return NotFound();
            }

            return View(activityDetailWork);
        }

        // POST: Procurement/ActivityDetailWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var activityDetailWork = await _context.ActivityDetailWork.FindAsync(id);
            _context.ActivityDetailWork.Remove(activityDetailWork);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityDetailWorkExists(short id)
        {
            return _context.ActivityDetailWork.Any(e => e.ActivityDetailWorkID == id);
        }
    }
}
