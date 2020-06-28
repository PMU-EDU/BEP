using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.Reports;

namespace BES.Controllers.Reports
{
    public class CumulativeRFsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CumulativeRFsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CumulativeRFs
        public async Task<IActionResult> Index()
        {
            return View(await _context.CumulativeRF.ToListAsync());
        }

        // GET: CumulativeRFs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cumulativeRF = await _context.CumulativeRF
                .FirstOrDefaultAsync(m => m.IndiID == id);
            if (cumulativeRF == null)
            {
                return NotFound();
            }

            return View(cumulativeRF);
        }

        // GET: CumulativeRFs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CumulativeRFs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IndiID,IndicatorName,GPEOrginalTarget,GPERevisedTarget,GPERevisedAchieved,GPEUnMetTarget,EUTarget,EUAchieved,CumulativeTarget")] CumulativeRF cumulativeRF)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cumulativeRF);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cumulativeRF);
        }

        // GET: CumulativeRFs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cumulativeRF = await _context.CumulativeRF.FindAsync(id);
            if (cumulativeRF == null)
            {
                return NotFound();
            }
            return View(cumulativeRF);
        }

        // POST: CumulativeRFs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IndiID,IndicatorName,GPEOrginalTarget,GPERevisedTarget,GPERevisedAchieved,GPEUnMetTarget,EUTarget,EUAchieved,CumulativeTarget")] CumulativeRF cumulativeRF)
        {
            if (id != cumulativeRF.IndiID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cumulativeRF);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CumulativeRFExists(cumulativeRF.IndiID))
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
            return View(cumulativeRF);
        }

        // GET: CumulativeRFs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cumulativeRF = await _context.CumulativeRF
                .FirstOrDefaultAsync(m => m.IndiID == id);
            if (cumulativeRF == null)
            {
                return NotFound();
            }

            return View(cumulativeRF);
        }

        // POST: CumulativeRFs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cumulativeRF = await _context.CumulativeRF.FindAsync(id);
            _context.CumulativeRF.Remove(cumulativeRF);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CumulativeRFExists(int id)
        {
            return _context.CumulativeRF.Any(e => e.IndiID == id);
        }
    }
}
