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
    public class ScorecardActivityStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScorecardActivityStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ScorecardActivityStatus
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ScorecardActivityStatus.Include(s => s.ScorecardActivity);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ScorecardActivityStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scorecardActivityStatus = await _context.ScorecardActivityStatus
                .Include(s => s.ScorecardActivity)
                .FirstOrDefaultAsync(m => m.ScorecardActivityStatusID == id);
            if (scorecardActivityStatus == null)
            {
                return NotFound();
            }

            return View(scorecardActivityStatus);
        }

        // GET: ScorecardActivityStatus/Create
        public IActionResult Create()
        {
            ViewData["ScorecardActivityID"] = new SelectList(_context.ScorecardActivity, "ScorecardActivityID", "ScorecardActivityID");
            return View();
        }

        // POST: ScorecardActivityStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScorecardActivityStatusID,ScoreCardMeetingScheduleID,ScorecardActivityID,CurrentStatus,Comments,UpdatedBy,CurrentDate")] ScorecardActivityStatus scorecardActivityStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scorecardActivityStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ScorecardActivityID"] = new SelectList(_context.ScorecardActivity, "ScorecardActivityID", "ScorecardActivityID", scorecardActivityStatus.ScorecardActivityID);
            return View(scorecardActivityStatus);
        }

        // GET: ScorecardActivityStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scorecardActivityStatus = await _context.ScorecardActivityStatus.FindAsync(id);
            if (scorecardActivityStatus == null)
            {
                return NotFound();
            }
            ViewData["ScorecardActivityID"] = new SelectList(_context.ScorecardActivity, "ScorecardActivityID", "ScorecardActivityID", scorecardActivityStatus.ScorecardActivityID);
            return View(scorecardActivityStatus);
        }

        // POST: ScorecardActivityStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScorecardActivityStatusID,ScoreCardMeetingScheduleID,ScorecardActivityID,CurrentStatus,Comments,UpdatedBy,CurrentDate")] ScorecardActivityStatus scorecardActivityStatus)
        {
            if (id != scorecardActivityStatus.ScorecardActivityStatusID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scorecardActivityStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScorecardActivityStatusExists(scorecardActivityStatus.ScorecardActivityStatusID))
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
            ViewData["ScorecardActivityID"] = new SelectList(_context.ScorecardActivity, "ScorecardActivityID", "ScorecardActivityID", scorecardActivityStatus.ScorecardActivityID);
            return View(scorecardActivityStatus);
        }

        // GET: ScorecardActivityStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scorecardActivityStatus = await _context.ScorecardActivityStatus
                .Include(s => s.ScorecardActivity)
                .FirstOrDefaultAsync(m => m.ScorecardActivityStatusID == id);
            if (scorecardActivityStatus == null)
            {
                return NotFound();
            }

            return View(scorecardActivityStatus);
        }

        // POST: ScorecardActivityStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scorecardActivityStatus = await _context.ScorecardActivityStatus.FindAsync(id);
            _context.ScorecardActivityStatus.Remove(scorecardActivityStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScorecardActivityStatusExists(int id)
        {
            return _context.ScorecardActivityStatus.Any(e => e.ScorecardActivityStatusID == id);
        }
    }
}
