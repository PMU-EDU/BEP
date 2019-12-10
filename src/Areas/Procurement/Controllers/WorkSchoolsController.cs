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
    public class WorkSchoolsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkSchoolsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Procurement/WorkSchools
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WorkSchool.Include(w => w.ActivityDetailWork).Include(w => w.School);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Procurement/WorkSchools
        public async Task<IActionResult> Index2(short id)
        {
            var applicationDbContext = _context.WorkSchool.Include(w => w.ActivityDetailWork).Include(w => w.School.UC.Tehsil.District).Where(a=>a.ActivityDetailWorkID == id);
            return PartialView(await applicationDbContext.ToListAsync());
        }
        // GET: Procurement/WorkSchools/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSchool = await _context.WorkSchool
                .Include(w => w.ActivityDetailWork)
                .Include(w => w.School)
                .FirstOrDefaultAsync(m => m.WorkSchoolID == id);
            if (workSchool == null)
            {
                return NotFound();
            }

            return View(workSchool);
        }
        public JsonResult GetSchools(int DistrictID)
        {           
            return Json(new SelectList(_context.Schools.Include(a=>a.UC.Tehsil.District).Where(a=>a.UC.Tehsil.DistrictID == DistrictID), "SchoolID", "SName"));
        }
        // GET: Procurement/WorkSchools/Create
        public IActionResult Create(short id, bool? State)
        {
            WorkSchool Obj = new WorkSchool();
            Obj.ActivityDetailWorkID = _context.ActivityDetailWork.Where(a => a.ActivityID == id).Select(a => a.ActivityDetailWorkID).FirstOrDefault();
            ViewBag.AName = _context.Activity.Find(id).Name;            
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName");
            ViewData["SchoolID"] = new SelectList(_context.Schools.Include(a=>a.UC.Tehsil.District).Where(a=>a.UC.Tehsil.DistrictID == 1), "SchoolID", "SName");

            
            int SchoolTotal = _context.Activity.Find(id).SchoolTotal;
            int CurrentSchool = _context.WorkSchool.Count(a => a.ActivityDetailWorkID == Obj.ActivityDetailWorkID);
            ViewBag.SchoolTotal = SchoolTotal;
            ViewBag.CurrentSchool = CurrentSchool;
            ViewBag.NextSchool = CurrentSchool + 1;
            ViewBag.RemainingSchool = SchoolTotal - CurrentSchool;
            if (State ?? false)
            {
                ViewBag.Error = "This school has been already selected!";
            }
            ViewBag.AID = id;
            return View(Obj);
        }
        //[Authorize(Roles = "Procurement")]
        [HttpPost]
        public async Task<IActionResult> AssignAmount(short WorkSchoolId, Int64 EAmount, Int64 AAmount)
        {
            var Obj = await _context.WorkSchool.FindAsync(WorkSchoolId);
            var AID = _context.ActivityDetailWork.Find(Obj.ActivityDetailWorkID).ActivityID;
            var AObj = _context.Activity.Find(AID);
            AObj.EstimatedCost +=(int) EAmount;
            AObj.ActualCost += (int)AAmount;
            Obj.EstimatedCost = EAmount;
            Obj.ActualCost = AAmount;
            Obj.CurrentDate = DateTime.Now.Date;            
            _context.Update(Obj);
            _context.Update(AObj);
            await _context.SaveChangesAsync();
            return Json(new { success = true, responseText = "Your message successfuly sent!" });
        }
        // POST: Procurement/WorkSchools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(short id,[Bind("WorkSchoolID,ActivityDetailWorkID,SchoolID,EstimatedCost,ActualCost,CurrentDate")] WorkSchool workSchool)
        {
            if (ModelState.IsValid)
            {
                short IsExist = (short)_context.WorkSchool.Count(a => a.SchoolID == workSchool.SchoolID);
                if(IsExist == 0)
                {                    
                    workSchool.CurrentDate = DateTime.Now.Date;
                    _context.Add(workSchool);
                    await _context.SaveChangesAsync();                    
                    return RedirectToAction(nameof(Create), new { id });
                }
                else
                {                    
                    return RedirectToAction(nameof(Create), new { id, State = true });
                }              
            }
            ViewData["DistrictID"] = new SelectList(_context.Districts, "DistrictID", "DistrictName");
            ViewData["SchoolID"] = new SelectList(_context.Schools.Include(a => a.UC.Tehsil.District).Where(a => a.UC.Tehsil.DistrictID == 1), "SchoolID", "SName");
            return View(workSchool);
        }

        // GET: Procurement/WorkSchools/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSchool = await _context.WorkSchool.FindAsync(id);
            if (workSchool == null)
            {
                return NotFound();
            }
            ViewData["ActivityDetailWorkID"] = new SelectList(_context.ActivityDetailWork, "ActivityDetailWorkID", "ActivityDetailWorkID", workSchool.ActivityDetailWorkID);
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", workSchool.SchoolID);
            return View(workSchool);
        }

        // POST: Procurement/WorkSchools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("WorkSchoolID,ActivityDetailWorkID,SchoolID,EstimatedCost,ActualCost,CurrentDate")] WorkSchool workSchool)
        {
            if (id != workSchool.WorkSchoolID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workSchool);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkSchoolExists(workSchool.WorkSchoolID))
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
            ViewData["ActivityDetailWorkID"] = new SelectList(_context.ActivityDetailWork, "ActivityDetailWorkID", "ActivityDetailWorkID", workSchool.ActivityDetailWorkID);
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", workSchool.SchoolID);
            return View(workSchool);
        }

        // GET: Procurement/WorkSchools/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSchool = await _context.WorkSchool
                .Include(w => w.ActivityDetailWork)
                .Include(w => w.School)
                .FirstOrDefaultAsync(m => m.WorkSchoolID == id);
            if (workSchool == null)
            {
                return NotFound();
            }

            return View(workSchool);
        }

        // POST: Procurement/WorkSchools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var workSchool = await _context.WorkSchool.FindAsync(id);
            _context.WorkSchool.Remove(workSchool);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkSchoolExists(short id)
        {
            return _context.WorkSchool.Any(e => e.WorkSchoolID == id);
        }
    }
}
