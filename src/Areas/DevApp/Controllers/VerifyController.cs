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
    [Area("DevApp")]
    public class VerifyController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
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
        // GET: Verify
        public async Task<IActionResult> SchoolList()
        {
            //ViewBag.id = id;
            //int PId = id == 926982 ? 4 : 3;
            //ViewBag.SectionID = id;
            //if (id == 926982)
            //{
            //    ViewBag.Section = "Education ";
            //}
            //else if (id == 352769)
            //{
            //    ViewBag.Section = "Development ";
            //}
            //else
            //{
            //    return RedirectToAction("index", "BaselineGenerals");
            //}
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
                                          Proj_IncdicatorTracking.ReVerified == false && Proj_IncdicatorTracking.ReUpload == false && Indicators.IndicatorID >28
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