using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BES.Data;
using BES.Models.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BES.Areas.DevApp.Controllers
{
    public class ImagesList
    {
        public IQueryable <IndicatorDevApp> devAppList { get; set; }
    }
    [Area("DevApp")]
    public class VerifyController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        ImagesList imagesList;
        public VerifyController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            imagesList = new ImagesList();
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
        // GET: Verify
        public async Task<IActionResult> SchoolList()
        {
            
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
                                          Proj_IncdicatorTracking.ReVerified == false && Proj_IncdicatorTracking.ReUpload == false && Indicators.IndicatorID > 28 && Indicators.IndicatorID !=35  && Indicators.IndicatorID < 40
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


            //applicationDbContext= applicationDbContext.Where(a)
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult>IndicatorList(int id)
        {
            var indiTrack = _context.IncdicatorTracking.Where(a => a.SchoolID == id);
            var applicationDbContext = from Proj_Indicator in _context.Indicator
                                       join Proj_IncdicatorTracking in _context.IncdicatorTracking on Proj_Indicator.IndicatorID equals Proj_IncdicatorTracking.IndicatorID into Proj_IncdicatorTracking_join
                                       from Proj_IncdicatorTracking in Proj_IncdicatorTracking_join.DefaultIfEmpty()
                                       where
                                         Proj_Indicator.IndicatorID>28
                                        && (Proj_IncdicatorTracking.SchoolID == id )
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
            ViewBag.SchoolName = (await _context.Schools.FindAsync(id)).SName;
            return View(await applicationDbContext.ToListAsync());
        }
        
        public async Task<IActionResult> Indicator(int id, int iid)
        {
            var applicationDbContext = _context.IndicatorDevApp.Where(a => a.SchoolID == id & a.IndicatorID == iid);
           // imagesList.devAppList =  _context.IndicatorDevApp.Where(a => a.SchoolID == id & a.IndicatorID == iid);
            return View(await applicationDbContext.ToListAsync ());

        }
        [HttpPost]
        public async Task<IActionResult> Indicator(List<IndicatorDevApp> indicatorDevs)
        {
            int id=0, iid=0;
            foreach (var devApp in indicatorDevs.Where(a=>a.VerifyRE==true))
            {
                devApp.VerifyREBy = User.Identity.Name;
                devApp.VerifyREDate = DateTime.Now;
                _context.Update(devApp);
                id = devApp.SchoolID;
                iid = devApp.IndicatorID;
            }
            var indi = _context.IncdicatorTracking.Where(a => a.SchoolID == id && a.IndicatorID == iid).FirstOrDefault();
            indi.ReVerified = true;
            indi.ReVerifiedBy = User.Identity.Name;
            indi.ReVerifiedDate = DateTime.Now;
            _context.Update(indi);
           await _context.SaveChangesAsync();
            return RedirectToAction("IndicatorList", new { id = id });
        }

        // GET: Verify/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Verify/Create
        public ActionResult Index()
        {
            return View();
        }

     
    }
}