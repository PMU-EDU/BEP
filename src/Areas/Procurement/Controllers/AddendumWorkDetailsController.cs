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
    public class AddendumWorkDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddendumWorkDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Procurement/AddendumWorkDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AddendumWorkDetail.Include(a => a.AddendumWorks).Include(a => a.School);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Procurement/AddendumWorkDetails/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendumWorkDetail = await _context.AddendumWorkDetail
                .Include(a => a.AddendumWorks)
                .Include(a => a.School)
                .FirstOrDefaultAsync(m => m.AddendumWorkDetailId == id);
            if (addendumWorkDetail == null)
            {
                return NotFound();
            }

            return View(addendumWorkDetail);
        }

        // GET: Procurement/AddendumWorkDetails/Create
        public IActionResult Create()
        {
            ViewData["AddendumId"] = new SelectList(_context.AddendumWorks, "AddendumId", "AddendumId");
            ViewData["SchoolId"] = new SelectList(_context.Schools, "SchoolID", "SName");
            return View();
        }
        public IActionResult CreateList(short id, short AddendumId)
        {            
            var SchoolList = _context.WorkSchool.Include(a=>a.School).Where(a => a.ActivityDetailWorkID == id).ToList();
            List<AddendumWorkDetail> AddWorkDetailObjList = new List<AddendumWorkDetail>();
            foreach(var SchoolObj in SchoolList)
            {
                AddendumWorkDetail Obj = new AddendumWorkDetail();                
                Obj.AddendumId = AddendumId;
                Obj.SchoolId = SchoolObj.SchoolID;
                Obj.ActualCost = SchoolObj.ActualCost ?? 0;                
                Obj.School = new School();
                Obj.School = SchoolObj.School;
                AddWorkDetailObjList.Add(Obj);
            }            
            return View(AddWorkDetailObjList);
        }
        // POST: Procurement/AddendumWorkDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AddendumWorkDetailId,Amount,ActualCost,Sign,AddendumId,SchoolId")] AddendumWorkDetail addendumWorkDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(addendumWorkDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddendumId"] = new SelectList(_context.AddendumWorks, "AddendumId", "AddendumId", addendumWorkDetail.AddendumId);
            ViewData["SchoolId"] = new SelectList(_context.Schools, "SchoolID", "SName", addendumWorkDetail.SchoolId);
            return View(addendumWorkDetail);
        }

        // GET: Procurement/AddendumWorkDetails/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendumWorkDetail = await _context.AddendumWorkDetail.FindAsync(id);
            if (addendumWorkDetail == null)
            {
                return NotFound();
            }
            ViewData["AddendumId"] = new SelectList(_context.AddendumWorks, "AddendumId", "AddendumId", addendumWorkDetail.AddendumId);
            ViewData["SchoolId"] = new SelectList(_context.Schools, "SchoolID", "SName", addendumWorkDetail.SchoolId);
            return View(addendumWorkDetail);
        }

        // POST: Procurement/AddendumWorkDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("AddendumWorkDetailId,Amount,ActualCost,Sign,AddendumId,SchoolId")] AddendumWorkDetail addendumWorkDetail)
        {
            if (id != addendumWorkDetail.AddendumWorkDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addendumWorkDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddendumWorkDetailExists(addendumWorkDetail.AddendumWorkDetailId))
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
            ViewData["AddendumId"] = new SelectList(_context.AddendumWorks, "AddendumId", "AddendumId", addendumWorkDetail.AddendumId);
            ViewData["SchoolId"] = new SelectList(_context.Schools, "SchoolID", "SName", addendumWorkDetail.SchoolId);
            return View(addendumWorkDetail);
        }

        // GET: Procurement/AddendumWorkDetails/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendumWorkDetail = await _context.AddendumWorkDetail
                .Include(a => a.AddendumWorks)
                .Include(a => a.School)
                .FirstOrDefaultAsync(m => m.AddendumWorkDetailId == id);
            if (addendumWorkDetail == null)
            {
                return NotFound();
            }

            return View(addendumWorkDetail);
        }

        // POST: Procurement/AddendumWorkDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var addendumWorkDetail = await _context.AddendumWorkDetail.FindAsync(id);
            _context.AddendumWorkDetail.Remove(addendumWorkDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddendumWorkDetailExists(short id)
        {
            return _context.AddendumWorkDetail.Any(e => e.AddendumWorkDetailId == id);
        }
    }
}
