using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Models.Data;
using BES.Data;
using BES.Models.Reports;
using Microsoft.AspNetCore.Authorization;

namespace BES.Controllers.Data
{
    public class BaselineGeneralsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaselineGeneralsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BaselineGenerals
        [Authorize(Roles = "Administrator,M&E,Education,Development")]
        public IActionResult Index()
        {
            List<Region> RegionList = new List<Region>();
            RegionList = _context.Regions.ToList();
            RegionList.Insert(0, new Region { RegionID = 0, RegionName = "All" });
            RegionList.RemoveAt(RegionList.Count-1);
            ViewData["RegionID"] = new SelectList(RegionList, "RegionID", "RegionName");

            //var applicationDbContext = _context.BaselineGenerals.Include(b => b.School);
            return View();
        }
        public JsonResult GetDistrict(int RegionID)
        {
            List<District> districtList = new List<District>();
            districtList = _context.Districts.Where(a => a.RegionID == RegionID).ToList();
            districtList.Insert(0, new District { DistrictID = 0, DistrictName = "All" });
            return Json(new SelectList(districtList, "DistrictID", "DistrictName"));
        }

        public async Task<IActionResult> FilterView(short? RID, short? DID, short? TID, short? UID, short? LID)
        {
            var applicationDbContext = _context.BaselineGenerals.Include(s => s.School).Include(u=>u.School.UC).Include(t=>t.School.UC.Tehsil).Include(d=>d.School.UC.Tehsil.District)
                .Where(b=>b.School.ProjectID==1);

            if (UID > 0)
                applicationDbContext = applicationDbContext.Where(q => q.School.UCID == UID);
            else if (TID > 0)
                applicationDbContext = applicationDbContext.Where(q => q.School.UC.TehsilID == TID);
            else if (DID > 0)
                applicationDbContext = applicationDbContext.Where(q => q.School.UC.Tehsil.DistrictID == DID);
            else if (RID > 0)
                applicationDbContext = applicationDbContext.Where(q => q.School.UC.Tehsil.District.RegionID == RID);

            applicationDbContext = applicationDbContext.OrderBy(a => a.School.UC.Tehsil.District.RegionID).ThenBy(a => a.ClusterBEMISCode).ThenBy(a => a.SchoolType);

            return PartialView(await applicationDbContext.ToListAsync());
        }

        public ActionResult Dashboard()
        {

            //if (!checkAuthentication())
            //{
            //    return RedirectToAction("Login", "Account", new { area = "" });
            //}
            var bLDashboardView = new BLDashboardView
            {
                filter = 0,
                blV = _context.BLEUDetailViews.ToList()

            };

            return View(bLDashboardView);
        }

        [HttpPost]
        public ActionResult Dashboard(BLDashboardView bLDashboardView)
        {
            //var bLEUDetailViews = db.BLEUDetailViews.Where(a => a.SchoolID > 0);
            switch (bLDashboardView.filter)
            {
                case 1:
                    bLDashboardView.blV = _context.BLEUDetailViews.Where(a => a.SchoolType == "Potentail").ToList();
                    break;
                case 2:
                    bLDashboardView.blV = _context.BLEUDetailViews.Where(a => a.SchoolType == "Feeder").ToList(); ;
                    break;
                case 3:
                    bLDashboardView.blV = _context.BLEUDetailViews.Where(a => a.SchoolType == "Next Level").ToList(); ;
                    break;
                default:
                    bLDashboardView.blV = _context.BLEUDetailViews.ToList(); ;
                    break;
            }
            //bLDashboardView.blV = bLEUDetailViews.ToList();
            return View(bLDashboardView);
        }
        public ActionResult DashboardFilter(short? st)
        {
            var bLEUDetailViews = _context.BLEUDetailViews;

            ViewBag.PotentailTotal = bLEUDetailViews.Count(a => a.SchoolType == "Potentail");

            return PartialView(bLEUDetailViews.ToList());

        }
        // GET: BaselineGenerals/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baselineGeneral = await _context.BaselineGenerals
                .Include(b => b.School)
                .FirstOrDefaultAsync(m => m.BLGeneralID == id);
            if (baselineGeneral == null)
            {
                return NotFound();
            }

            return View(baselineGeneral);
        }
        //Raw data
        public async Task<IActionResult> RawData()
        {
            return View(await _context.BLEUDetailViews.OrderBy(a => a.Region).ThenBy(a => a.SchoolID).ThenBy(a => a.TypeSort).ToListAsync());
        }
        // GET: BaselineGenerals/Create
        public IActionResult Create()
        {
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName");
            return View();
        }

        // POST: BaselineGenerals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BLGeneralID,UCID,SchoolID,SchoolType,ClusterBEMISCode,BEMISCode,SName,Type,Latitude,Longitude,VisitorName,Date,varified,VarifiedBy,Remarks")] BaselineGeneral baselineGeneral)
        {
            if (ModelState.IsValid)
            {
                _context.Add(baselineGeneral);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", baselineGeneral.SchoolID);
            return View(baselineGeneral);
        }

        // GET: BaselineGenerals/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baselineGeneral = await _context.BaselineGenerals.FindAsync(id);

            ViewBag.NullableBool = new SelectList(new[]
                   {
                         new { Id = "", Name = "" },
                        new { Id = "True", Name = "Yes" },
                        new { Id = "False", Name = "No" }
                    }, "Id", "Name");
           // BaselineGeneral baselinegeneral = _context.BaselineGenerals.Find(id);
            //if (baselineGeneral.School.UC.Tehsil.District.RegionID != loginUser.RegionID && !User.IsInRole("ICT"))
            //{
            //    return RedirectToAction("Login", "Account", new { area = "" });
            //}
            baselineGeneral.School = _context.Schools.Find(baselineGeneral.SchoolID);

            

            var modelCollection = new ModelCollection();
            modelCollection.baselineGeneral = baselineGeneral;
            modelCollection.blEnrollmentList = _context.BLEnrollments.Where(a => a.BaselineGenerals.BLGeneralID == id).ToList();
            modelCollection.blFacilitiesInfo = _context.BLFacilitiesInfoes.Where(a => a.BaselineGenerals.BLGeneralID == id).FirstOrDefault();
            modelCollection.blLandAvailable = _context.BLLandAvailables.Where(a => a.BaselineGeneral.BLGeneralID == id).FirstOrDefault();
            modelCollection.blPTSMCInfo = _context.BLPTSMCInfoes.Where(a => a.BaselineGenerals.BLGeneralID == id).FirstOrDefault();
            modelCollection.blTeacherSectionList = _context.BLTeacherSections.Where(a => a.BaselineGenerals.BLGeneralID == id).ToList();
            modelCollection.newSchool = _context.Schools.Where(a => a.SchoolID == baselineGeneral.SchoolID).FirstOrDefault();

            if (Convert.ToInt16(baselineGeneral.Type) > 1 && modelCollection.blEnrollmentList.Count() < 9)
            {
                modelCollection.blEnrollmentList.Add(new BLEnrollment());
                modelCollection.blEnrollmentList.Add(new BLEnrollment());
                modelCollection.blEnrollmentList.Add(new BLEnrollment());
            }
            if (Convert.ToInt16(baselineGeneral.Type) > 2 && modelCollection.blEnrollmentList.Count() < 11)
            {
                modelCollection.blEnrollmentList.Add(new BLEnrollment());
                modelCollection.blEnrollmentList.Add(new BLEnrollment());
            }

            ViewBag.UCID = new SelectList(_context.UCs.Where(a => a.TehsilID == _context.UCs.Where(b => b.UCID == baselineGeneral.UCID).Select(b => b.TehsilID).FirstOrDefault()), "UCID", "UCName", baselineGeneral.UCID);

            //modelCollection.baselineGeneral.BEMISCode = modelCollection.newSchool.BEMIS.ToString();
            //modelCollection.baselineGeneral.Latitude = modelCollection.newSchool.Latitude;
            //modelCollection.baselineGeneral.Longitude = modelCollection.newSchool.Longitude;
            //modelCollection.baselineGeneral.Type = modelCollection.newSchool.Level.ToString();
            //modelCollection.baselineGeneral.VarifiedBy = User.Identity.Name;
            //modelCollection.baselineGeneral.School.Latitude = modelCollection.newSchool.Latitude;
            //modelCollection.baselineGeneral.School.Longitude = modelCollection.newSchool.Longitude;
            //modelCollection.baselineGeneral.School.BEMIS = modelCollection.newSchool.BEMIS;
            return View(modelCollection);
            ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", baselineGeneral.SchoolID);
            return View(baselineGeneral);
        }

        // POST: BaselineGenerals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelCollection modelCollection)
        {
            //modelCollection.baselineGeneral.School = _context.Schools.Find(modelCollection.baselineGeneral.SchoolID);
            var Error1 = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
            int temp1 = 0;
            for (int h = 0; h < Error1.Count; h++)
            {
                if (Error1[h][0].ErrorMessage != "")
                {
                    temp1 = 1;
                }
            }



            if (ModelState.IsValid || temp1 == 0)
            {

                var CurrentSchoolID = modelCollection.newSchool.SchoolID;
                int ClusterSchoolID = 0;
                if (modelCollection.baselineGeneral.SchoolType != 1)
                {
                    //ClusterSchoolID = _context.Schools.Where(a => a.BEMIS == modelCollection.newSchool.ClusterBEMIS && a.ProjectID == 1).Select(a => a.SchoolID).FirstOrDefault();
                    //if (ClusterSchoolID == 0)
                    //{
                        ClusterSchoolID = _context.Schools.Where(a => a.ClusterBEMIS == modelCollection.baselineGeneral.ClusterBEMISCode && a.type == 1).Select(a => a.SchoolID).FirstOrDefault();
                        if (ClusterSchoolID == 0)
                        {
                            ViewBag.Error = "Error: No Potentail school Exist against BEMIS Code " + modelCollection.baselineGeneral.ClusterBEMISCode;
                            ViewBag.UCID = new SelectList(_context.UCs, "UCID", "UCName", modelCollection.baselineGeneral.UCID);
                            return View(modelCollection);

                        }
                   // }
                }
                //else
                //{
                    modelCollection.newSchool.UCID = modelCollection.UCID;
                    modelCollection.newSchool.type = (short) modelCollection.baselineGeneral.SchoolType;
                    modelCollection.newSchool.BEMIS = modelCollection.baselineGeneral.BEMISCode;
                    modelCollection.newSchool.ClusterBEMIS = (int) modelCollection.baselineGeneral.ClusterBEMISCode;
                modelCollection.newSchool.SName = modelCollection.baselineGeneral.SName;
                    modelCollection.newSchool.Latitude = modelCollection.baselineGeneral.Latitude;
                    modelCollection.newSchool.Longitude = modelCollection.baselineGeneral.Longitude;
                    modelCollection.newSchool.SLevel = Convert.ToInt16(modelCollection.baselineGeneral.Type);
                    _context.Entry(modelCollection.newSchool).State = EntityState.Modified;
                    _context.SaveChanges();
                //}

                modelCollection.baselineGeneral.VarifiedBy = User.Identity.Name;
                modelCollection.baselineGeneral.UCID = modelCollection.UCID;
                _context.Entry(modelCollection.baselineGeneral).State = EntityState.Modified;
                _context.SaveChanges();
                var CurrentBaselineID = modelCollection.baselineGeneral.BLGeneralID;
                short Counter = 0;
                if (modelCollection.baselineGeneral.Type == "1")
                {
                    Counter = 6;
                }
                else if (modelCollection.baselineGeneral.Type == "2")
                {
                    Counter = 9;
                }
                else
                {
                    Counter = 11;
                }
                for (short i = 0; i < Counter; i++)
                {
                    modelCollection.blEnrollmentList[i].BLGeneralID = CurrentBaselineID;
                    modelCollection.blEnrollmentList[i].BLEnrollmentID = _context.BLEnrollments.Where(a => a.BLGeneralID == CurrentBaselineID && a.ClassID == i).Select(a => a.BLEnrollmentID).FirstOrDefault();
                    modelCollection.blEnrollmentList[i].ClassID = i;
                    if (modelCollection.blEnrollmentList[i].BLEnrollmentID == 0)
                    {
                        //modelCollection.blEnrollmentList[i].
                        _context.BLEnrollments.Add(modelCollection.blEnrollmentList[i]);

                    }
                    else
                    {
                        _context.Entry(modelCollection.blEnrollmentList[i]).State = EntityState.Modified;
                    }
                    _context.SaveChanges();
                }
                short temp = 0;
                for (short i = 0; i < 9; i++)
                {
                    temp = i;
                    temp++;
                    modelCollection.blTeacherSectionList[i].BLGeneralID = CurrentBaselineID;
                    modelCollection.blTeacherSectionList[i].BLTeacherSectionID = (short)_context.BLTeacherSections.Where(a => a.BLGeneralID == CurrentBaselineID && a.BLTeacherPostID == temp).Select(a => a.BLTeacherSectionID).FirstOrDefault();
                    modelCollection.blTeacherSectionList[i].BLTeacherPostID = temp;
                    _context.Entry(modelCollection.blTeacherSectionList[i]).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                modelCollection.blLandAvailable.BLGeneralID = CurrentBaselineID;
                _context.Entry(modelCollection.blLandAvailable).State = EntityState.Modified;
                _context.SaveChanges();
                modelCollection.blPTSMCInfo.BLGeneralID = CurrentBaselineID;
                _context.Entry(modelCollection.blPTSMCInfo).State = EntityState.Modified;
                _context.SaveChanges();
                modelCollection.blFacilitiesInfo.BLGeneralID = CurrentBaselineID;
                _context.Entry(modelCollection.blFacilitiesInfo).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            var Error = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
            List<string> errorList = new List<string>();
            for (int z = 0; z < Error.Count; z++)
            {
                ModelState.AddModelError("", Error[z][0].ErrorMessage);
                //    errorList.Add(Error[z][0].ErrorMessage);
            }
            ViewBag.UCID = new SelectList(_context.UCs.Where(a => a.TehsilID == _context.UCs.Where(b => b.UCID == modelCollection.baselineGeneral.UCID).Select(b => b.TehsilID).FirstOrDefault()), "UCID", "UCName", modelCollection.baselineGeneral.UCID);
            return View(modelCollection);
        }
            // GET: BaselineGenerals/Delete/5
            public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baselineGeneral = await _context.BaselineGenerals
                .Include(b => b.School)
                .FirstOrDefaultAsync(m => m.BLGeneralID == id);
            if (baselineGeneral == null)
            {
                return NotFound();
            }

            return View(baselineGeneral);
        }

        // POST: BaselineGenerals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var baselineGeneral = await _context.BaselineGenerals.FindAsync(id);
            _context.BaselineGenerals.Remove(baselineGeneral);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaselineGeneralExists(short id)
        {
            return _context.BaselineGenerals.Any(e => e.BLGeneralID == id);
        }
    }
}
