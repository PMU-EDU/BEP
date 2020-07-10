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
    public class TehsilsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TehsilsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tehsils
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tehsils.Include(t => t.District);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tehsils/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tehsil = await _context.Tehsils
                .Include(t => t.District)
                .FirstOrDefaultAsync(m => m.TehsilID == id);
            if (tehsil == null)
            {
                return NotFound();
            }

            return View(tehsil);
        }

        // GET: Tehsils/Create
        public IActionResult Create()
        {
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName");
            return View();
        }

        // POST: Tehsils/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TehsilID,DistrictID,TehsilName,Tehsilcode")] Tehsil tehsil)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tehsil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName", tehsil.DistrictID);
            return View(tehsil);
        }

        // GET: Tehsils/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tehsil = await _context.Tehsils.FindAsync(id);
            if (tehsil == null)
            {
                return NotFound();
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName", tehsil.DistrictID);
            return View(tehsil);
        }

        // POST: Tehsils/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TehsilID,DistrictID,TehsilName,Tehsilcode")] Tehsil tehsil)
        {
            if (id != tehsil.TehsilID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tehsil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TehsilExists(tehsil.TehsilID))
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
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName", tehsil.DistrictID);
            return View(tehsil);
        }

        // GET: Tehsils/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tehsil = await _context.Tehsils
                .Include(t => t.District)
                .FirstOrDefaultAsync(m => m.TehsilID == id);
            if (tehsil == null)
            {
                return NotFound();
            }

            return View(tehsil);
        }

        // POST: Tehsils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tehsil = await _context.Tehsils.FindAsync(id);
            _context.Tehsils.Remove(tehsil);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TehsilExists(int id)
        {
            return _context.Tehsils.Any(e => e.TehsilID == id);
        }
    }
}
