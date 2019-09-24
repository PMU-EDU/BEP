﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.Data;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BES.Controllers.Data
{
    public class IncdicatorTrackingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncdicatorTrackingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IncdicatorTrackings
        public async Task<IActionResult> Index(int id)
        {
            ViewBag.SectionID=id;
            if(id==926982)
            {
                ViewBag.Section = "Education Section";
            }
            else if(id==352769)
            {
                ViewBag.Section = "Development Section";
            }
            var applicationDbContext = _context.Schools.Where(a=>a.SchoolOf==2).Include(a=>a.UC).Include(a=>a.UC.Tehsil).Include(a => a.UC.Tehsil.District);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: IncdicatorTrackings/Update
        public IActionResult Update(int id, int SecID)
        {
            int PId = SecID == 926982 ? 4 : 3;
            ViewBag.SecID = SecID;
            var applicationDbContext = from Proj_Indicator in _context.Indicator
                                       join Proj_IncdicatorTracking in _context.IncdicatorTracking on Proj_Indicator.IndicatorID equals Proj_IncdicatorTracking.IndicatorID into Proj_IncdicatorTracking_join
                                       from Proj_IncdicatorTracking in Proj_IncdicatorTracking_join.DefaultIfEmpty()
                                       where
                                         Proj_Indicator.PartnerID==PId
                                       orderby
                                         Proj_Indicator.SequenceNo
                                       select new IndicatorTracking
                                       {
                                           IndicatorID = Proj_Indicator.IndicatorID,
                                           IndicatorName= Proj_Indicator.IndicatorName,
                                           isEvidence= Proj_Indicator.IsEvidenceRequire,
                                           ImageURL = Proj_IncdicatorTracking.ImageURL,
                                           DateOfUpload = Proj_IncdicatorTracking.DateOfUpload,
                                           SchoolID = id,
                                          // Proj_Indicator.SequenceNo
                                       };

            return View(applicationDbContext.ToList());
        }

        // POST: IncdicatorTrackings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int sID,int iID, DateTime EDate, int SecID)
        {
            // Save uploaded files
            var files = Request.Form.Files;

            string District = _context.Schools.Include(a => a.UC.Tehsil.District)
                                  .Where(a=>a.SchoolID==sID)
                                .Select(a => a.UC.Tehsil.District.DistrictName).FirstOrDefault();

            var rootPath = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot\\Documents\\IndicatorEvidences\\");

            string sPath = Path.Combine(rootPath + District + "/" + iID + "/", sID.ToString());
            if (!System.IO.Directory.Exists(sPath))
            {
                System.IO.Directory.CreateDirectory(sPath);
            }
            short i = 1;
            string fileName = sID.ToString()+"-";
            foreach(var file in files)
            {
                string FullPathWithFileName = Path.Combine(sPath, fileName+i++ + Path.GetExtension(file.FileName));
                using (var stream = new FileStream(FullPathWithFileName, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
            }
            IndicatorTracking IndiTrack = new IndicatorTracking();
            IndiTrack.IndicatorID = iID;
            IndiTrack.SchoolID = sID;
            IndiTrack.IsUpload = true;
            IndiTrack.DateOfUpload = EDate;
            IndiTrack.ImageURL= Path.Combine("/Documents/IndicatorEvidences/", District +  "//" + iID + "//" + sID);//Server Path
            IndiTrack.CreateDate = DateTime.Now;
            IndiTrack.CreatedBy = User.Identity.Name;

            
            IndiTrack.Verified = false;

            _context.Add(IndiTrack);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.Write(ex.InnerException);
                return View();
            }
            //ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", incdicatorTracking.SchoolID);
            return RedirectToAction("Update", new { id = sID, SecID = SecID });
        }
        // GET: IncdicatorTrackings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incdicatorTracking = await _context.IncdicatorTracking
                .Include(i => i.School)
                .FirstOrDefaultAsync(m => m.SchoolID == id);
            if (incdicatorTracking == null)
            {
                return NotFound();
            }

            return View(incdicatorTracking);
        }

        // GET: IncdicatorTrackings/Create
        public IActionResult Create()
        {
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName");
            return View();
        }

        // POST: IncdicatorTrackings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IndicatorID,SchoolID,ImageURL,Verified,IsUpload,DateOfUpload,CreatedBy,CreateDate,UpdatedBy,UpdatedDate,VerifiedBy,VerifiedDate")] IndicatorTracking incdicatorTracking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(incdicatorTracking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", incdicatorTracking.SchoolID);
            return View(incdicatorTracking);
        }

        // GET: IncdicatorTrackings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incdicatorTracking = await _context.IncdicatorTracking.FindAsync(id);
            if (incdicatorTracking == null)
            {
                return NotFound();
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", incdicatorTracking.SchoolID);
            return View(incdicatorTracking);
        }

        // POST: IncdicatorTrackings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IndicatorID,SchoolID,ImageURL,Verified,IsUpload,DateOfUpload,CreatedBy,CreateDate,UpdatedBy,UpdatedDate,VerifiedBy,VerifiedDate")] IndicatorTracking incdicatorTracking)
        {
            if (id != incdicatorTracking.SchoolID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incdicatorTracking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!IncdicatorTrackingExists(incdicatorTracking.SchoolID))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", incdicatorTracking.SchoolID);
            return View(incdicatorTracking);
        }

        // GET: IncdicatorTrackings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incdicatorTracking = await _context.IncdicatorTracking
                .Include(i => i.School)
                .FirstOrDefaultAsync(m => m.SchoolID == id);
            if (incdicatorTracking == null)
            {
                return NotFound();
            }

            return View(incdicatorTracking);
        }

        // POST: IncdicatorTrackings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incdicatorTracking = await _context.IncdicatorTracking.FindAsync(id);
            _context.IncdicatorTracking.Remove(incdicatorTracking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IncdicatorTrackingExists(int id)
        {
            return _context.IncdicatorTracking.Any(e => e.SchoolID == id);
        }
    }
}
