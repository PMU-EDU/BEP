using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Areas.Procurement.Models;
using BES.Data;
using Microsoft.AspNetCore.Authorization;

namespace BES.Areas.Procurement.Controllers
{
    [Area("Procurement")]
    public class ProcurementPlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProcurementPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Procurement/ProcurementPlans
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProcurementPlan.ToListAsync());
        }

        public ActionResult Dashboard()
        {
            return View();
        }
        // GET: Procurement/ProcurementPlans/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procurementPlan = await _context.ProcurementPlan
                .FirstOrDefaultAsync(m => m.ProcurementPlanID == id);
            if (procurementPlan == null)
            {
                return NotFound();
            }

            return View(procurementPlan);
        }

        // GET: Procurement/ProcurementPlans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Procurement/ProcurementPlans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProcurementPlanID,Name,Description")] ProcurementPlan procurementPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(procurementPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(procurementPlan);
        }

        // GET: Procurement/ProcurementPlans/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procurementPlan = await _context.ProcurementPlan.FindAsync(id);
            if (procurementPlan == null)
            {
                return NotFound();
            }
            return View(procurementPlan);
        }

        // POST: Procurement/ProcurementPlans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("ProcurementPlanID,Name,Description")] ProcurementPlan procurementPlan)
        {
            if (id != procurementPlan.ProcurementPlanID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(procurementPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcurementPlanExists(procurementPlan.ProcurementPlanID))
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
            return View(procurementPlan);
        }

        // GET: Procurement/ProcurementPlans/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procurementPlan = await _context.ProcurementPlan
                .FirstOrDefaultAsync(m => m.ProcurementPlanID == id);
            if (procurementPlan == null)
            {
                return NotFound();
            }

            return View(procurementPlan);
        }

        // POST: Procurement/ProcurementPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var procurementPlan = await _context.ProcurementPlan.FindAsync(id);
            _context.ProcurementPlan.Remove(procurementPlan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProcurementPlanExists(short id)
        {
            return _context.ProcurementPlan.Any(e => e.ProcurementPlanID == id);
        }
    }
}
