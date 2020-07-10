using System;
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
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using BES.Services.Profile;
using Microsoft.AspNetCore.Identity;
using System.Net.NetworkInformation;

namespace BES.Controllers.Data
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class IncdicatorTrackingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IncdicatorTrackingsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<string> GetCurrentUserId()
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            //if (usr.RegionalAccess != null)
            return (usr.RegionalAccess);
            //else
            //    return ("a");
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        //public IncdicatorTrackingsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // GET: IncdicatorTrackings

        [Authorize(Roles = "Administrator,Education,Development")]

        public async Task<IActionResult> Index(int id)
        {

            if (User.Identity.Name == null)
                return RedirectToAction("Login", "Account");
            if (User.IsInRole("Education"))
                id = 926982;
            else if (User.IsInRole("Development"))
                id = 352769;
            else if (User.IsInRole("Administrator"))
            { }
            else
                return RedirectToPage("/AccessDenied");
            ViewBag.SectionID = id;
            if (id == 926982)
            {
                ViewBag.Section = "Education Section";
            }
            else if (id == 352769)
            {
                ViewBag.Section = "Development Section";
            }
            else
            {
                return RedirectToAction("index", "BaselineGenerals");
            }
            var applicationDbContext = _context.Schools.Where(a => a.ProjectID == 1).Include(a => a.UC).Include(a => a.UC.Tehsil).Include(a => a.UC.Tehsil.District).OrderBy(a => a.RegName).ThenBy(a => a.ClusterBEMIS).ThenBy(a => a.type); ;
            try
            {
                string ra = await GetCurrentUserId();
                int[] regions = ra.Split(',').Select(int.Parse).ToArray();
                //int[] array= Array.ConvertAll(ra, int.Parse);
                Console.WriteLine(regions);
                if (regions.Any())
                {
                    //    applicationDbContext = from sch in _context.Schools
                    //                           where regions.Contains(sch.UC.Tehsil.District.RegionID)
                    //                           orderby sch.UC.Tehsil.District.RegionID,

                    //                           ;
                }
                switch (regions.Length)
                {
                    case 1:
                        applicationDbContext = applicationDbContext.Where(a => a.UC.Tehsil.District.RegionID == regions[0]).OrderBy(a => a.UC.Tehsil.District.RegionID).ThenBy(a => a.ClusterBEMIS).ThenBy(a => a.type); ;
                        break;
                    case 2:
                        applicationDbContext = applicationDbContext.Where(a => a.UC.Tehsil.District.RegionID == regions[0] || a.UC.Tehsil.District.RegionID == regions[1]).OrderBy(a => a.UC.Tehsil.District.RegionID).ThenBy(a => a.ClusterBEMIS).ThenBy(a => a.type); ;
                        break;
                    case 3:
                        applicationDbContext = applicationDbContext.Where(a => a.UC.Tehsil.District.RegionID == regions[0] || a.UC.Tehsil.District.RegionID == regions[1] || a.UC.Tehsil.District.RegionID == regions[2]).OrderBy(a => a.UC.Tehsil.District.RegionID).ThenBy(a => a.ClusterBEMIS).ThenBy(a => a.type); ;
                        break;
                    case 4:
                        applicationDbContext = applicationDbContext.Where(a => a.UC.Tehsil.District.RegionID == regions[0] || a.UC.Tehsil.District.RegionID == regions[1] || a.UC.Tehsil.District.RegionID == regions[2] || a.UC.Tehsil.District.RegionID == regions[3]).OrderBy(a => a.UC.Tehsil.District.RegionID).ThenBy(a => a.ClusterBEMIS).ThenBy(a => a.type); ;
                        break;
                    default: break;

                }
            }
            catch (Exception ex)
            { }
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: IncdicatorTrackings/Update
        public IActionResult Update(int id, int SecID)
        {
            int PId = SecID == 926982 ? 4 : 3;
            ViewBag.SecID = SecID;
            var sch = _context.Schools.Find(id);
            ViewBag.Sname = sch.SName;


            var indiTrack = _context.IncdicatorTracking.Where(a => a.SchoolID == id);
            var applicationDbContext = from Indicator in _context.Indicator
                                       join IncdicatorTracking in indiTrack on Indicator.IndicatorID equals IncdicatorTracking.IndicatorID into Proj_IncdicatorTracking_join
                                       from Proj_IncdicatorTracking in Proj_IncdicatorTracking_join.DefaultIfEmpty()
                                       where
                                         Indicator.PartnerID == PId && Indicator.SequenceNo > 0
                                       // && (Proj_IncdicatorTracking.SchoolID == id ||
                                       //Proj_IncdicatorTracking.SchoolID == null)

                                       orderby
                                         Indicator.SequenceNo
                                       select new IndicatorTracking
                                       {
                                           IndicatorID = Indicator.IndicatorID,
                                           Indicator = Indicator.IndicatorName,
                                           isEvidence = Indicator.IsEvidenceRequire,
                                           ImageURL = Proj_IncdicatorTracking.ImageURL,
                                           DateOfUpload = Proj_IncdicatorTracking.DateOfUpload,
                                           SchoolID = id,
                                           IsUpload = Proj_IncdicatorTracking.IsUpload,
                                           TotalFilesUploaded = Proj_IncdicatorTracking.TotalFilesUploaded,
                                           isPotential = Indicator.IsPotential,
                                           isFeeder = Indicator.IsFeeder,
                                           isNextLevel = Indicator.IsNextLevel,
                                           EvidanceType = Indicator.EvidanceType,
                                           ReUpload = Proj_IncdicatorTracking.ReUpload,
                                           School = sch

                                           //SchoolID = Proj_IncdicatorTracking.SchoolID == id ? id : Proj_IncdicatorTracking.SchoolID ==  null ? (int?)null : 0,

                                           // Proj_Indicator.SequenceNo
                                       };

            if (SecID == 926982)
            {
                ViewBag.Section = "Education Section";
                switch (sch.type)
                {
                    case 1:
                        applicationDbContext = applicationDbContext.Where(a => a.isPotential == true);
                        break;
                    case 2:
                        applicationDbContext = applicationDbContext.Where(a => a.isFeeder == true);
                        break;
                    case 3:
                        applicationDbContext = applicationDbContext.Where(a => a.isNextLevel == true);
                        break;
                }

            }
            else if (SecID == 352769)
            {
                ViewBag.Section = "Development Section";
                if (sch.NewConstruction == false)
                {   // remove Soil test and Master Plan
                    applicationDbContext = applicationDbContext.Where(a => a.IndicatorID != 27 && a.IndicatorID != 28);
                }
                if (sch.ExternalDevelopment == false)
                {
                    applicationDbContext = applicationDbContext.Where(a => a.IndicatorID < 42);
                }
            }
            //  applicationDbContext = applicationDbContext.Where(a => a.IndicatorID < 29 && a.IndicatorID>34);

            //Remove Andorid App Indicators


            //applicationDbContext = applicationDbContext.Where(a => a.SchoolID == id || a.SchoolID == null);
            //List<IndicatorTracking> indicatorTrackings = new List<IndicatorTracking>();
            //foreach(var indi in applicationDbContext)
            //{
            //    if(indi.SchoolID!=id ||)
            //}

            ViewData["ids"] = applicationDbContext.Select(a => a.IndicatorID).ToArray();
            return View(applicationDbContext.ToList());
        }
        public ActionResult Popup(int? id, int? iId)
        {
            ViewBag.PointOut = iId;

            string fpath = _context.IncdicatorTracking
                            .Where(a => a.SchoolID == id && a.IndicatorID == iId)
                            .Select(a => a.ImageURL).FirstOrDefault();

            var rootPath = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot" + fpath);
            DirectoryInfo dir = new DirectoryInfo(rootPath);
            FileInfo[] files = dir.GetFiles();
            ArrayList list = new ArrayList();
            List<string> filePaths = new List<string>();
            foreach (FileInfo file in files)
            {
                filePaths.Add(Path.GetFileName(file.FullName));
                //list.Add(file);
            }
            ViewData["filePaths"] = filePaths;
            ViewBag.fPath = fpath;
            return PartialView();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePost(int sID, int iID, DateTime EDate, bool reupload)
        {
            DateTime projectDate = DateTime.Parse("1/1/2017");
            if (EDate< projectDate)
            {
                EDate = DateTime.Now;
            }
            Indicator indicator = _context.Indicator.Find(iID);

            if (indicator.IsEvidenceRequire == true)
            { }
            // Save uploaded files
            var files = Request.Form.Files;

            string District = _context.Schools.Include(a => a.UC.Tehsil.District)
                                  .Where(a => a.SchoolID == sID)
                                .Select(a => a.UC.Tehsil.District.DistrictName).FirstOrDefault();
            //string 
            var rootPath = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot\\Documents\\IndicatorEvidences\\");

            //string sPath = Path.Combine(rootPath + District + "/" + iID + "/", sID.ToString());
            string sPath = Path.Combine(rootPath + District + "/" + iID + "/");
            if (!System.IO.Directory.Exists(sPath) && indicator.IsEvidenceRequire == true)
            {
                System.IO.Directory.CreateDirectory(sPath);
            }
            short i = 1;
            string fileName = sID.ToString() + "-";
            foreach (var file in files)
            {
                string FullPathWithFileName = Path.Combine(sPath, fileName + i++ + Path.GetExtension(file.FileName));
                using (var stream = new FileStream(FullPathWithFileName, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

            }
            try
            {
                //create record
                IndicatorTracking IndiTrack = new IndicatorTracking();
                IndiTrack.IndicatorID = iID;
                IndiTrack.SchoolID = sID;
                IndiTrack.IsUpload = true;
                IndiTrack.TotalFilesUploaded = (short)files.Count;
                IndiTrack.DateOfUpload = EDate;
                IndiTrack.ImageURL = indicator.IsEvidenceRequire ? Path.Combine("/Documents/IndicatorEvidences/", District + "//" + iID) : null;//Server Path

                if (reupload == true)
                {// update record
                    IndiTrack = _context.IncdicatorTracking.Where(a => a.IndicatorID == iID && a.SchoolID == sID).FirstOrDefault();
                    IndiTrack.TotalFilesUploaded = (short)files.Count;
                    IndiTrack.DateOfUpload = EDate;
                    IndiTrack.ImageURL = Path.Combine("/Documents/IndicatorEvidences/", District + "//" + iID);//Server Path
                    IndiTrack.ReUpload = false;
                    IndiTrack.Verified = false;
                    IndiTrack.UpdatedDate = DateTime.Now;
                    IndiTrack.UpdatedBy = User.Identity.Name;
                    _context.Update(IndiTrack);
                }
                else
                {
                    IndiTrack.CreateDate = DateTime.Now;
                    IndiTrack.CreatedBy = User.Identity.Name;
                    IndiTrack.ReUpload = false;
                    IndiTrack.Verified = false;
                    _context.Add(IndiTrack);

                }
                // IndiTrack.CreateDate = DateTime.Now;
                //IndiTrack.CreatedBy = User.Identity.Name;

                //update school table
                if (iID == 26 || iID == 35 || iID == 41)
                {
                    School school = _context.Schools.Find(sID);
                    switch (iID)
                    { case 26: school.NewConstruction = true; break;
                        case 35: school.RepairRennovation = true; break;
                        case 41: school.ExternalDevelopment = true; break;
                    }
                    _context.Update(school);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.InnerException.Message);
                return Json(new { success = false, responseText = ex.InnerException.Message });
            }
            //ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", incdicatorTracking.SchoolID);
            //return RedirectToAction(nameof(Update), new { id = sID, SecID = SecID });
            return Json(new { success = true, responseText = "Sucessfully Updated" }); //, sID = sID, SecID = SecID });
        }


        // GET: /IncdicatorTrackings/MneVerifyEdu
        [Authorize(Roles = "Administrator,M&E")]
        public async Task<IActionResult> MneVerifyEdu(int id)
        {
            ViewBag.id = id;
            int PId = id == 926982 ? 4 : 3;
            ViewBag.SectionID = id;
            if (id == 926982)
            {
                ViewBag.Section = "Education ";
            }
            else if (id == 352769)
            {
                ViewBag.Section = "Development ";
            }
            else
            {
                return RedirectToAction("index", "BaselineGenerals");
            }
            var applicationDbContext = (from Schools in _context.Schools
                                        join Proj_IncdicatorTracking in _context.IncdicatorTracking on Schools.SchoolID equals Proj_IncdicatorTracking.SchoolID
                                        join Indicators in _context.Indicator on Proj_IncdicatorTracking.IndicatorID equals Indicators.IndicatorID
                                        join Ucs in _context.UCs on Schools.UCID equals Ucs.UCID
                                        join Tehsils in _context.Tehsils
                                              on new { Ucs.TehsilID, Column1 = Ucs.TehsilID }
                                          equals new { Tehsils.TehsilID, Column1 = Tehsils.TehsilID }
                                        join Districts in _context.Districts
                                              on new { Tehsils.DistrictID, Column1 = Tehsils.DistrictID }
                                          equals new { Districts.DistrictID, Column1 = Districts.DistrictID }
                                        where
                                          Proj_IncdicatorTracking.Verified == false && Proj_IncdicatorTracking.ReUpload == false && Indicators.PartnerID == PId
                                        group new { Schools, Districts } by new
                                        {
                                            Schools.SchoolID,
                                            Schools.SName,
                                            Schools.ClusterBEMIS,
                                            Schools.type,
                                            Districts.RegionID,
                                            Districts.DistrictName
                                        } into g
                                        orderby
                                          g.Key.RegionID,
                                          g.Key.DistrictName,
                                          g.Key.type
                                        select new School
                                        {
                                            RegName = g.Key.RegionID.ToString(),
                                            DisName = g.Key.DistrictName,
                                            SchoolID = g.Key.SchoolID,
                                            SName = g.Key.SName,
                                            ClusterBEMIS = g.Key.ClusterBEMIS,
                                            type = g.Key.type
                                        });

            try
            {
                string ra = await GetCurrentUserId();
                //int[] regions = ra.Split(',').Select(int.Parse).ToArray();
                //string[] regions = ra.Split(','); //.ToArray();
                //int[] array= Array.ConvertAll(ra, int.Parse);
                //Console.WriteLine(regions);
                if (ra.Length > 0)
                {
                    applicationDbContext = applicationDbContext.Where(e => e.RegName.Any(r => ra.Contains(r)));
                }
            }
            catch (Exception ex)
            { }

            return View(await applicationDbContext.ToListAsync());
        }
        //public async Task<IActionResult> MneVerifyIndicators(short id, int SecID)

        // GET: /IncdicatorTrackings/ReuploadEvidence
        [Authorize(Roles = "Administrator,M&E,Education,Development")]
        public async Task<IActionResult> ReuploadEvidence(int id)
        {
            int PId = 0;// id == 926982 ? 4 : 3;
            if (User.Identity.Name == null)
                return RedirectToAction("Login", "Account");
            if (User.IsInRole("Education"))
                id = 926982;
            else if (User.IsInRole("Development"))
                id = 352769;
            else if (User.IsInRole("Administrator") || User.IsInRole("M&E"))
            { }
            else
                return RedirectToPage("/AccessDenied");
            ViewBag.id = id;
            if (id == 926982)
            {
                ViewBag.Section = "Education Section";
                PId = 4;
            }
            else if (id == 352769)
            {
                ViewBag.Section = "Development Section";
                PId = 3;
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            // var IndicatorsFilter = _context.Indicator.Where(a => a.PartnerID == PId);
            var applicationDbContext = (from Schools in _context.Schools
                                        join Proj_IncdicatorTracking in _context.IncdicatorTracking on Schools.SchoolID equals Proj_IncdicatorTracking.SchoolID
                                        join Indicators in _context.Indicator on Proj_IncdicatorTracking.IndicatorID equals Indicators.IndicatorID
                                        join Ucs in _context.UCs on Schools.UCID equals Ucs.UCID
                                        join Tehsils in _context.Tehsils
                                              on new { Ucs.TehsilID, Column1 = Ucs.TehsilID }
                                          equals new { Tehsils.TehsilID, Column1 = Tehsils.TehsilID }
                                        join Districts in _context.Districts
                                              on new { Tehsils.DistrictID, Column1 = Tehsils.DistrictID }
                                          equals new { Districts.DistrictID, Column1 = Districts.DistrictID }
                                        where
                                           Proj_IncdicatorTracking.ReUpload == true && Indicators.PartnerID == PId
                                        group new { Schools, Districts } by new
                                        {
                                            Schools.SchoolID,
                                            Schools.SName,
                                            Schools.ClusterBEMIS,
                                            Schools.type,
                                            Districts.RegionID,
                                            Districts.DistrictName
                                        } into g
                                        orderby
                                          g.Key.RegionID,
                                          g.Key.DistrictName,
                                          g.Key.type
                                        select new School
                                        {
                                            RegName = g.Key.RegionID.ToString(),
                                            DisName = g.Key.DistrictName,
                                            SchoolID = g.Key.SchoolID,
                                            SName = g.Key.SName,
                                            ClusterBEMIS = g.Key.ClusterBEMIS,
                                            type = g.Key.type
                                        });

            try
            {
                string ra = await GetCurrentUserId();
                //int[] regions = ra.Split(',').Select(int.Parse).ToArray();
                //string[] regions = ra.Split(','); //.ToArray();
                //int[] array= Array.ConvertAll(ra, int.Parse);
                //Console.WriteLine(regions);
                if (ra.Length > 0)
                {
                    applicationDbContext = applicationDbContext.Where(e => e.RegName.Any(r => ra.Contains(r)));
                }
            }
            catch (Exception ex)
            { }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: /IncdicatorTrackings/VerifidEvidence
        [Authorize(Roles = "Administrator,Education,Development,M&E")]
        public async Task<IActionResult> VerifiedEvidence(int id)
        {

            int PId = 0;// id == 926982 ? 4 : 3;
            if (User.Identity.Name == null)
                return RedirectToAction("Login", "Account");
            if (User.IsInRole("Education"))
                id = 926982;
            else if (User.IsInRole("Development"))
                id = 352769;
            else if (User.IsInRole("Administrator") || User.IsInRole("M&E"))
            { }
            else
                return RedirectToPage("/AccessDenied");
            ViewBag.SectionID = id;
            if (id == 926982)
            {
                ViewBag.Section = "Education Section";
                PId = 4;
            }
            else if (id == 352769)
            {
                ViewBag.Section = "Development Section";
                PId = 3;
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            var applicationDbContext = (from Schools in _context.Schools
                                        join Proj_IncdicatorTracking in _context.IncdicatorTracking on Schools.SchoolID equals Proj_IncdicatorTracking.SchoolID
                                        join Indicators in _context.Indicator on Proj_IncdicatorTracking.IndicatorID equals Indicators.IndicatorID
                                        join Ucs in _context.UCs on Schools.UCID equals Ucs.UCID
                                        join Tehsils in _context.Tehsils
                                              on new { Ucs.TehsilID, Column1 = Ucs.TehsilID }
                                          equals new { Tehsils.TehsilID, Column1 = Tehsils.TehsilID }
                                        join Districts in _context.Districts
                                              on new { Tehsils.DistrictID, Column1 = Tehsils.DistrictID }
                                          equals new { Districts.DistrictID, Column1 = Districts.DistrictID }
                                        where
                                           Proj_IncdicatorTracking.Verified == true && Indicators.PartnerID == PId
                                        group new { Schools, Districts } by new
                                        {
                                            Schools.SchoolID,
                                            Schools.SName,
                                            Schools.ClusterBEMIS,
                                            Schools.type,
                                            Districts.RegionID,
                                            Districts.DistrictName
                                        } into g
                                        orderby
                                          g.Key.RegionID,
                                          g.Key.DistrictName,
                                          g.Key.type
                                        select new School
                                        {
                                            RegName = g.Key.RegionID.ToString(),
                                            DisName = g.Key.DistrictName,
                                            SchoolID = g.Key.SchoolID,
                                            SName = g.Key.SName,
                                            ClusterBEMIS = g.Key.ClusterBEMIS,
                                            type = g.Key.type
                                        });


            try
            {
                string ra = await GetCurrentUserId();
                //int[] regions = ra.Split(',').Select(int.Parse).ToArray();
                //string[] regions = ra.Split(','); //.ToArray();
                //int[] array= Array.ConvertAll(ra, int.Parse);
                //Console.WriteLine(regions);
                if (ra.Length > 0)
                {
                    applicationDbContext = applicationDbContext.Where(e => e.RegName.Any(r => ra.Contains(r)));
                }
            }
            catch (Exception ex)
            { }

            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Administrator,M&E")]
        public IActionResult MneVerifyIndicators(int id, int SecID)
        {
            int PId = SecID == 926982 ? 4 : 3;
            ViewBag.SecID = SecID;
            var sch = _context.Schools.Find(id);
            ViewBag.Sname = sch.SName;

            if (SecID == 926982)
            {
                ViewBag.Section = "Education Section";
            }
            else if (SecID == 352769)
            {
                ViewBag.Section = "Development Section";
            }
            var indiTrack = _context.IncdicatorTracking.Where(a => a.SchoolID == id);
            var applicationDbContext = from Proj_Indicator in _context.Indicator
                                       join Proj_IncdicatorTracking in indiTrack on Proj_Indicator.IndicatorID equals Proj_IncdicatorTracking.IndicatorID into Proj_IncdicatorTracking_join
                                       from Proj_IncdicatorTracking in Proj_IncdicatorTracking_join.DefaultIfEmpty()
                                       where
                                         Proj_Indicator.PartnerID == PId
                                       // && (Proj_IncdicatorTracking.SchoolID == id ||
                                       //Proj_IncdicatorTracking.SchoolID == null)

                                       orderby
                                         Proj_Indicator.SequenceNo
                                       select new IndicatorTracking
                                       {
                                           IndicatorID = Proj_Indicator.IndicatorID,
                                           Indicator = Proj_Indicator.IndicatorName,
                                           isEvidence = Proj_Indicator.IsEvidenceRequire,
                                           ImageURL = Proj_IncdicatorTracking.ImageURL,
                                           DateOfUpload = Proj_IncdicatorTracking.DateOfUpload,
                                           SchoolID = id,
                                           IsUpload = Proj_IncdicatorTracking.IsUpload,
                                           TotalFilesUploaded = Proj_IncdicatorTracking.TotalFilesUploaded,
                                           isPotential = Proj_Indicator.IsPotential,
                                           isFeeder = Proj_Indicator.IsFeeder,
                                           isNextLevel = Proj_Indicator.IsNextLevel,
                                           EvidanceType = Proj_Indicator.EvidanceType,
                                           ReUpload = Proj_IncdicatorTracking.ReUpload,
                                           Verified = Proj_IncdicatorTracking.Verified,
                                           //SchoolID = Proj_IncdicatorTracking.SchoolID == id ? id : Proj_IncdicatorTracking.SchoolID ==  null ? (int?)null : 0,

                                           // Proj_Indicator.SequenceNo
                                       };

            if (SecID == 926982)
            {
                ViewBag.Section = "Education Section";
                switch (sch.type)
                {
                    case 1:
                        applicationDbContext = applicationDbContext.Where(a => a.isPotential == true);
                        break;
                    case 2:
                        applicationDbContext = applicationDbContext.Where(a => a.isFeeder == true);
                        break;
                    case 3:
                        applicationDbContext = applicationDbContext.Where(a => a.isNextLevel == true);
                        break;
                }

            }
            else if (SecID == 352769)
            {
                ViewBag.Section = "Development Section";
                if (sch.NewConstruction == false)
                {   // remove Soil test and Master Plan
                    applicationDbContext = applicationDbContext.Where(a => a.IndicatorID != 26 && a.IndicatorID != 27);
                }
                if (sch.ExternalDevelopment == false)
                {
                    applicationDbContext = applicationDbContext.Where(a => a.IndicatorID < 31);
                }
            }

            //applicationDbContext = applicationDbContext.Where(a => a.SchoolID == id || a.SchoolID == null);
            //List<IndicatorTracking> indicatorTrackings = new List<IndicatorTracking>();
            //foreach(var indi in applicationDbContext)
            //{
            //    if(indi.SchoolID!=id ||)
            //}
            ViewData["ids"] = applicationDbContext.Select(a => a.IndicatorID).ToArray();
            return View(applicationDbContext.ToList());
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> MneVerifyIndicatorsPost(int sID, int iID, bool verified, bool reUpload)
        {

            try
            {
                //create record
                IndicatorTracking IndiTrack = await _context.IncdicatorTracking.Where(a => a.SchoolID == sID && a.IndicatorID == iID).FirstOrDefaultAsync();
                // IndiTrack.IndicatorID = iID;
                //IndiTrack.SchoolID = sID;
                // IndiTrack.IsUpload = true;
                //IndiTrack.TotalFilesUploaded = (short)files.Count;
                // IndiTrack.DateOfUpload = EDate;
                //  IndiTrack.ImageURL = Path.Combine("/Documents/IndicatorEvidences/", District + "//" + iID);//Server Path
                // // IndiTrack.CreateDate = DateTime.Now;
                //  IndiTrack.CreatedBy = User.Identity.Name;
                IndiTrack.Verified = verified;
                IndiTrack.ReUpload = reUpload;
                if (IndiTrack.Verified == true)
                {
                    IndiTrack.VerifiedBy = User.Identity.Name;
                    IndiTrack.VerifiedDate = DateTime.Now;
                }
                //IndiTrack.Verified = false;

                _context.Update(IndiTrack);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.InnerException.Message);
                return Json(new { success = false, responseText = ex.InnerException.Message });
            }
            //ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", incdicatorTracking.SchoolID);
            //return RedirectToAction(nameof(Update), new { id = sID, SecID = SecID });
            return Json(new { success = true, responseText = "Sucessfully Updated" }); //, sID = sID, SecID = SecID });
        }

        [Authorize(Roles = "Administrator,M&E")]
        public async Task<IActionResult> MneUnVerifyEdu(int id = 926982)
        {
            if (User.IsInRole("M&E") && User.Identity.Name!="Naseeb")
            {
                return  LocalRedirect("/Account/AccessDenied");
            }
            ViewBag.id = id;
            int PId = id == 926982 ? 4 : 3;
            ViewBag.SectionID = id;
            if (id == 926982)
            {
                ViewBag.Section = "Education ";
            }
            else if (id == 352769)
            {
                ViewBag.Section = "Development ";
            }
            else
            {
                return RedirectToAction("index", "BaselineGenerals");
            }
            var applicationDbContext = (from Schools in _context.Schools
                                        join Proj_IncdicatorTracking in _context.IncdicatorTracking on Schools.SchoolID equals Proj_IncdicatorTracking.SchoolID
                                        join Indicators in _context.Indicator on Proj_IncdicatorTracking.IndicatorID equals Indicators.IndicatorID
                                        join Ucs in _context.UCs on Schools.UCID equals Ucs.UCID
                                        join Tehsils in _context.Tehsils
                                              on new { Ucs.TehsilID, Column1 = Ucs.TehsilID }
                                          equals new { Tehsils.TehsilID, Column1 = Tehsils.TehsilID }
                                        join Districts in _context.Districts
                                              on new { Tehsils.DistrictID, Column1 = Tehsils.DistrictID }
                                          equals new { Districts.DistrictID, Column1 = Districts.DistrictID }
                                        //where
                                          /*Proj_IncdicatorTracking.Verified == false && Proj_IncdicatorTracking.ReUpload == false && */
                                         // Indicators.PartnerID == PId
                                        group new { Schools, Districts } by new
                                        {
                                            Schools.SchoolID,
                                            Schools.SName,
                                            Schools.ClusterBEMIS,
                                            Schools.type,
                                            Districts.RegionID,
                                            Districts.DistrictName
                                        } into g
                                        orderby
                                          g.Key.RegionID,
                                          g.Key.DistrictName,
                                          g.Key.type
                                        select new School
                                        {
                                            RegName = g.Key.RegionID.ToString(),
                                            DisName = g.Key.DistrictName,
                                            SchoolID = g.Key.SchoolID,
                                            SName = g.Key.SName,
                                            ClusterBEMIS = g.Key.ClusterBEMIS,
                                            type = g.Key.type
                                        });

            try
            {
                string ra = await GetCurrentUserId();
                //int[] regions = ra.Split(',').Select(int.Parse).ToArray();
                //string[] regions = ra.Split(','); //.ToArray();
                //int[] array= Array.ConvertAll(ra, int.Parse);
                //Console.WriteLine(regions);
                if (ra.Length > 0)
                {
                    applicationDbContext = applicationDbContext.Where(e => e.RegName.Any(r => ra.Contains(r)));
                }
            }
            catch (Exception ex)
            { }

            return View(await applicationDbContext.ToListAsync());
        }
        [Authorize(Roles = "Administrator,M&E")]
        public IActionResult MneUnVerifyIndicators(int id, int SecID)
        {
            if (User.IsInRole("M&E") && User.Identity.Name != "Naseeb")
            {
                return LocalRedirect("/Account/AccessDenied");
            }
            int PId = SecID == 926982 ? 4 : 3;
            ViewBag.SecID = SecID;
            var sch = _context.Schools.Find(id);
            ViewBag.Sname = sch.SName;

            if (SecID == 926982)
            {
                ViewBag.Section = "Education Section";
            }
            else if (SecID == 352769)
            {
                ViewBag.Section = "Development Section";
            }
            var indiTrack = _context.IncdicatorTracking.Where(a => a.SchoolID == id);
            var applicationDbContext = from Proj_Indicator in _context.Indicator
                                       join Proj_IncdicatorTracking in indiTrack on Proj_Indicator.IndicatorID equals Proj_IncdicatorTracking.IndicatorID into Proj_IncdicatorTracking_join
                                       from Proj_IncdicatorTracking in Proj_IncdicatorTracking_join.DefaultIfEmpty()
                                       where
                                         Proj_Indicator.PartnerID == PId
                                       // && (Proj_IncdicatorTracking.SchoolID == id ||
                                       //Proj_IncdicatorTracking.SchoolID == null)

                                       orderby
                                         Proj_Indicator.SequenceNo
                                       select new IndicatorTracking
                                       {
                                           IndicatorID = Proj_Indicator.IndicatorID,
                                           Indicator = Proj_Indicator.IndicatorName,
                                           isEvidence = Proj_Indicator.IsEvidenceRequire,
                                           ImageURL = Proj_IncdicatorTracking.ImageURL,
                                           DateOfUpload = Proj_IncdicatorTracking.DateOfUpload,
                                           SchoolID = id,
                                           IsUpload = Proj_IncdicatorTracking.IsUpload,
                                           TotalFilesUploaded = Proj_IncdicatorTracking.TotalFilesUploaded,
                                           isPotential = Proj_Indicator.IsPotential,
                                           isFeeder = Proj_Indicator.IsFeeder,
                                           isNextLevel = Proj_Indicator.IsNextLevel,
                                           EvidanceType = Proj_Indicator.EvidanceType,
                                           ReUpload = Proj_IncdicatorTracking.ReUpload,
                                           Verified = Proj_IncdicatorTracking.Verified,
                                           //SchoolID = Proj_IncdicatorTracking.SchoolID == id ? id : Proj_IncdicatorTracking.SchoolID ==  null ? (int?)null : 0,

                                           // Proj_Indicator.SequenceNo
                                       };

            if (SecID == 926982)
            {
                ViewBag.Section = "Education Section";
                switch (sch.type)
                {
                    case 1:
                        applicationDbContext = applicationDbContext.Where(a => a.isPotential == true);
                        break;
                    case 2:
                        applicationDbContext = applicationDbContext.Where(a => a.isFeeder == true);
                        break;
                    case 3:
                        applicationDbContext = applicationDbContext.Where(a => a.isNextLevel == true);
                        break;
                }

            }
            else if (SecID == 352769)
            {
                ViewBag.Section = "Development Section";
                if (sch.NewConstruction == false)
                {   // remove Soil test and Master Plan
                    applicationDbContext = applicationDbContext.Where(a => a.IndicatorID != 26 && a.IndicatorID != 27);
                }
                if (sch.ExternalDevelopment == false)
                {
                    applicationDbContext = applicationDbContext.Where(a => a.IndicatorID < 31);
                }
            }

            //applicationDbContext = applicationDbContext.Where(a => a.SchoolID == id || a.SchoolID == null);
            //List<IndicatorTracking> indicatorTrackings = new List<IndicatorTracking>();
            //foreach(var indi in applicationDbContext)
            //{
            //    if(indi.SchoolID!=id ||)
            //}
            ViewData["ids"] = applicationDbContext.Select(a => a.IndicatorID).ToArray();
            return View(applicationDbContext.ToList());
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> MneUnVerifyIndicatorsPost(int sID, int iID, bool verified, bool reUpload)
        {
            try
            {
                //create record
                IndicatorTracking IndiTrack = await _context.IncdicatorTracking.Where(a => a.SchoolID == sID && a.IndicatorID == iID).FirstOrDefaultAsync();
                // IndiTrack.IndicatorID = iID;
                //IndiTrack.SchoolID = sID;
                // IndiTrack.IsUpload = true;
                //IndiTrack.TotalFilesUploaded = (short)files.Count;
                // IndiTrack.DateOfUpload = EDate;
                //  IndiTrack.ImageURL = Path.Combine("/Documents/IndicatorEvidences/", District + "//" + iID);//Server Path
                // // IndiTrack.CreateDate = DateTime.Now;
                //  IndiTrack.CreatedBy = User.Identity.Name;
                IndiTrack.Verified = verified;
                IndiTrack.ReUpload = reUpload;
                if (IndiTrack.Verified == true)
                {
                    IndiTrack.VerifiedBy = User.Identity.Name;
                    IndiTrack.VerifiedDate = DateTime.Now;
                }
                //IndiTrack.Verified = false;
                //Logging user details
                IndicatorTrackUnVerify trackUnVerify = new IndicatorTrackUnVerify
                {
                    IndicatorID = IndiTrack.IndicatorID,
                    SchoolID = (int)IndiTrack.SchoolID,
                    Datetime = DateTime.Now,
                    MacAddress = NetworkInterface
                                .GetAllNetworkInterfaces()
                                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                .Select(nic => nic.GetPhysicalAddress().ToString())
                                .FirstOrDefault(),
                    MachineName = Environment.MachineName

                };
                _context.Add(trackUnVerify);

                _context.Update(IndiTrack);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.InnerException.Message);
                return Json(new { success = false, responseText = ex.InnerException.Message });
            }
            //ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", incdicatorTracking.SchoolID);
            //return RedirectToAction(nameof(Update), new { id = sID, SecID = SecID });
            return Json(new { success = true, responseText = "Sucessfully Updated" }); //, sID = sID, SecID = SecID });
        }

        public async Task<IActionResult> DevInfo(int id)
        {
            SchoolDevIndicator schoolDevIndicator = new SchoolDevIndicator();
            School school = _context.Schools.Find(id);
            schoolDevIndicator.SchoolID = id;
            schoolDevIndicator.NewRooms = school.NewRooms;
            schoolDevIndicator.RepairRooms = school.RepairRooms;
            schoolDevIndicator.NewToilets = school.NewToilets;
            schoolDevIndicator.RepairToilets = school.RepairToilets;

            return PartialView(schoolDevIndicator);
        }
        [HttpPost]
        public async Task<IActionResult> DevInfo(int id, short nr,short rr, short nt, short rt)
        {
            try
            {
                School school = _context.Schools.Find(id);
                school.NewRooms = nr;
                school.RepairRooms = rr;
                school.NewToilets = nt;
                school.RepairToilets = rt;
                _context.Update(school);
                await _context.SaveChangesAsync();
        }
            catch (Exception ex)
            {
                Console.Write(ex.InnerException.Message);
                return Json(new { success = false, responseText = ex.InnerException.Message
    });
            }
            //ViewData["SchoolID"] = new SelectList(_context.Schools, "SchoolID", "SName", incdicatorTracking.SchoolID);
            //return RedirectToAction(nameof(Update), new { id = sID, SecID = SecID });
            return Json(new { success = true, responseText = "Sucessfully Updated" }); //, sID = sID, SecID = SecID });
       
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
