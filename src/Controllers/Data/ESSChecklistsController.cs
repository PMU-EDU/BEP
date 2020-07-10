using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.Data;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using BES.Services.Profile;



namespace BES.Controllers.Data
{
    public class ESSChecklistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ESSChecklistsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<string> GetCurrentUserId()
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            return (usr.RegionalAccess);
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        //public IncdicatorTrackingsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // GET: IncdicatorTrackings
        [Authorize(Roles = "Administrator,Development")]
        public async Task<IActionResult> Index2()
        {
           
            return View(await _context.Schools.Include(a=>a.UC).Include(a => a.UC.Tehsil).Include(a => a.UC.Tehsil.District).ToListAsync());
        }



        public async Task<IActionResult> Edit(short ActivityID, short StepID)
        {
            if (ActivityID == 0 || StepID == 0)
            {
                return NotFound();
            }

            var activityDetail = await _context.ActivityDetail.Where(a => a.ActivityID == ActivityID && a.StepID == StepID).FirstOrDefaultAsync();
            activityDetail.Activity = _context.Activity.Find(ActivityID);
            activityDetail.Step = _context.Step.Find(StepID);
            activityDetail.UpdatedBy = User.Identity.Name;
            ViewBag.PPName = _context.ProcurementPlan.Find(activityDetail.Step.ProcurementPlanID).Name;
            if (activityDetail == null)
            {
                return NotFound();
            }
            return View(activityDetail);
        }

               
        // GET: ESSChecklists
        public async Task<IActionResult> Index()
        {
            return View(await _context.ESSChecklist.Include(a=>a.School).ToListAsync());
        }

        // GET: ESSChecklists/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eSSChecklist = await _context.ESSChecklist
                .FirstOrDefaultAsync(m => m.ESMPSitingID == id);
            if (eSSChecklist == null)
            {
                return NotFound();
            }

            return View(eSSChecklist);
        }

        // GET: ESSChecklists/Create
        public IActionResult Create(int SchoolID)
        {
            ESSChecklist obj = new ESSChecklist();
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName");
            obj.Verifiedby = "Kiran Mushtaq";           
            return View();
           
        }

        // POST: ESSChecklists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ESMPSitingID,SchoolID,a1Landmutation,a2IsSoilErosion,a3IsFloodPath,a4IsSalineLand,b1ClusterTrees,b2HowManyTrees,c1IsHighway,c2IsTransmissionLine,c3TypeTransmissionLine,c4HeightTransmissionLine,c5ElectricityPolPermises,c6TypeOfPole,c7FunctionHandwash,c8WashingRepairs,d1WaterSchoolPremises,d2WaterSourceType,d3WaterSourceVicinity,d4TypeWaterVicinity,d5DistanceWaterSource,d5Date,Verified,Verifiedby")] ESSChecklist eSSChecklist)
        {
            if (ModelState.IsValid)
            {                
                _context.Add(eSSChecklist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", eSSChecklist.SchoolID);
            return View(eSSChecklist);
        }

        // GET: ESSChecklists/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eSSChecklist = await _context.ESSChecklist.FindAsync(id);
            if (eSSChecklist == null)
            {
                return NotFound();
            }
            return View(eSSChecklist);
        }

        // POST: ESSChecklists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("ESMPSitingID,SchoolID,a1Landmutation,a2IsSoilErosion,a3IsFloodPath,a4IsSalineLand,b1ClusterTrees,b2HowManyTrees,c1IsHighway,c2IsTransmissionLine,c3TypeTransmissionLine,c4HeightTransmissionLine,c5ElectricityPolPermises,c6TypeOfPole,c7FunctionHandwash,c8WashingRepairs,d1WaterSchoolPremises,d2WaterSourceType,d3WaterSourceVicinity,d4TypeWaterVicinity,d5DistanceWaterSource,d5Date,Verified,Verifiedby")] ESSChecklist eSSChecklist)
        {
            if (id != eSSChecklist.ESMPSitingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eSSChecklist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ESSChecklistExists(eSSChecklist.ESMPSitingID))
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
            return View(eSSChecklist);
        }

        // GET: ESSChecklists/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eSSChecklist = await _context.ESSChecklist
                .FirstOrDefaultAsync(m => m.ESMPSitingID == id);
            if (eSSChecklist == null)
            {
                return NotFound();
            }

            return View(eSSChecklist);
        }

        // POST: ESSChecklists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var eSSChecklist = await _context.ESSChecklist.FindAsync(id);
            _context.ESSChecklist.Remove(eSSChecklist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ESSChecklistExists(short id)
        {
            return _context.ESSChecklist.Any(e => e.ESMPSitingID == id);
        }
    }
}
