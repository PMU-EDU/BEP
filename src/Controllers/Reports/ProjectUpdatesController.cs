using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BES.Data;
using BES.Models.Data;
using BES.Models.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BES.Controllers.Reports
{
    public class ProcurementProgress
    {
        public short ProcurementPlanID { get; set; }
        [Key]
        public string StepReferenceNo { get; set; }
        public string Activity { get; set; }
        public int Initiated { get; set; }
        public int Completed { get; set; }

    }

    public class ProjectSummary
    {
        public List<IndicatorsSummary> indicatorsSummaries { get; set; }
        public List<ProcurementProgress> procurementProgresses { get; set; }
    }

    public class ProjectUpdatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly UserManager<ApplicationUser> _userManager;

        public ProjectUpdatesController( ApplicationDbContext context)
        {
           // _userManager = userManager;
            _context = context;
        }

        //public ApplicationDbContext Context => _context;

        public IActionResult Index(int id, bool verify ,int? RegionID, DateTime? StartDate, DateTime? EndDate)
        {
            ViewBag.SectionID = id;
            ViewBag.Verify = verify;

            if (id == 926982)
            {
               
                    ViewBag.Section = "Education Section";
            }
            
           // ViewBag.Section = "Education Section";
           ////End temp code 
            else if (id == 352769)
                ViewBag.Section = "Development Section";
            else if (id == 123987)
                ViewBag.Section = "Project all Sections";
            else
                return RedirectToAction("index", "BaselineGenerals");

            if (verify == true)
                ViewBag.SummaryType = " Progress so far ";
            else
                ViewBag.SummaryType = " Uploaded Evidences so far";
            ViewBag.toDate = DateTime.Now.Date;
            //ViewBag.fromDate = ViewBag.toDate.AddDays(-30);
            ViewBag.fromDate = new DateTime(2018, 1, 1);

            List<Region> RegionList = new List<Region>();
            RegionList = _context.Regions.ToList();
            RegionList.Insert(0, new Region { RegionID = 0, RegionName = "All" });
            RegionList.RemoveAt(RegionList.Count - 1);
            ViewData["RegionID"] = new SelectList(RegionList, "RegionID", "RegionName");
            //var qry = _context.IncdicatorTracking.Include(a=>a.Indicator).Include(a=>a.School).i
            return View();
        }
        public JsonResult GetDistrict(int RegionID)
        {
            List<District> districtList = new List<District>();
            districtList = _context.Districts.Where(a => a.RegionID == RegionID).ToList();
            districtList.Insert(0, new District { DistrictID = 0, DistrictName = "All" });
            return Json(new SelectList(districtList, "DistrictID", "DistrictName"));
        }

        public async Task<IActionResult> FilterView(int? id, bool verify, short? RID, short? DID, DateTime? StartDate, DateTime? EndDate)
        {
            if (RID == 0) { RID = null; }
            ViewBag.Verify = verify;
            ViewBag.RID = RID;
            ViewBag.DID = DID;
            var indicatorsSummaries = _context.IndicatorsSummaries.FromSql("exec IndicatorSummarySP @RID, @DID, @verify", new SqlParameter("@RID", RID == null ? (object)DBNull.Value : RID), new SqlParameter("@DID", DID == null ? (object)DBNull.Value : DID), new SqlParameter("@verify", verify)); //.ToList<IndicatorsSummary>();
            var indicatorTotalTarget = _context.indicatorsTotalTargets.FromSql("exec IndicatorsTotalTargetSP @RID, @DID", new SqlParameter("@RID", RID == null ? (object)DBNull.Value : RID), new SqlParameter("@DID", DID == null ? (object)DBNull.Value : DID)); //.ToList<IndicatorsTotalTarget>(); ;
                                                                                                                                                                                                                                                                   // var indicatorTotalTarget = _context.indicatorsTotalTargets;
            var indictorAll = _context.Indicator.Where(a => a.IndicatorID <= 40);
            var query = from target in indicatorTotalTarget
                            //from summary in indicatorsSummaries
                            //from Indicator in indictorAll
                        from Indicator in indictorAll
                        join summary in indicatorsSummaries on Indicator.IndicatorID equals summary.IndicatorID into summary_join
                        from summary in summary_join.DefaultIfEmpty()

                        select new IndicatorsSummary
                        {
                            IndicatorID = Indicator.IndicatorID,
                            IndicatorName = Indicator.IndicatorName,
                            PartnerID = Indicator.PartnerID,
                            PotentailAchieve = summary.PotentailAchieve == null ? 0 : summary.PotentailAchieve,
                            FeederAchieve = summary.FeederAchieve == null ? 0 : summary.FeederAchieve,
                            NLAchieve = summary.NLAchieve == null ? 0 : summary.NLAchieve,
                            //TotalAchieve = summary.PotentailAchieve+summary.FeederAchieve+summary.NLAchieve,
                            TotalAchieve = summary.TotalAchieve == null ? 0 : summary.TotalAchieve,
                            //PotentailTarget = target.Potential,
                            PotentailTarget = Indicator.IndicatorID > 20 ? (Indicator.IndicatorID >25  ? (Indicator.IndicatorID > 34 ? target.PotentialRepair : target.PotentialNew):target.PotentialDev) : target.Potential,
                            FeederTarget = Indicator.IndicatorID > 20    ? (Indicator.IndicatorID > 25 ? (Indicator.IndicatorID > 34 ? target.FeederRepair    : target.FeederNew) : target.FeederDev) : (Indicator.IsFeeder ? target.Feeder : 0),
                            NLTarget = Indicator.IndicatorID > 20        ? (Indicator.IndicatorID > 25 ? (Indicator.IndicatorID > 34 ? target.NextLevelRepair : target.NextLevelNew) : target.NextLevelDev) : (Indicator.IsNextLevel ? target.NextLevel : 0),
                            TotalTarget = Indicator.IndicatorID > 20 ? (Indicator.IndicatorID > 35 ? target.TotalRepair : target.TotalNew) : target.TotalTarget
                            // TotalTarget = Indicator.IndicatorID > 20 ? (Indicator.IndicatorID > 35 ? target.PotentialRepair+ target.FeederRepair +target.NextLevelRepair : target.PotentialNew+ target.PotentialRepair+ target.NextLevelNew) : target.Potential+ target.Feeder+ target.NextLevel
                        };

            if (id == 926982)
            {
                ViewBag.Section = "Education Section";
                query = query.Where(a => a.IndicatorID < 20);
            }
            else if (id == 352769)
            {
                ViewBag.Section = "Development Section";
                query = query.Where(a => a.IndicatorID > 20);
            }

            // Todo: need optimzation below code taking 3 secs more. 
            try
            {
                var eduQuery = query.Where(a => a.PartnerID == 4);
                // var eduTotal = eduQuery.Sum(a => a.TotalTarget);
                // if (eduTotal > 0)
                ViewBag.EduPercent = eduQuery.Sum(a => a.TotalAchieve) * 100 / eduQuery.Sum(a => a.TotalTarget);
                //else
                //  ViewBag.EduPercent = 0;
            }
            catch
            {
                ViewBag.EduPercent = 0;
            }
            //try { ViewBag.DevPercent = query.Where(a => a.PartnerID == 3).Sum(a => a.TotalAchieve) * 100 / query.Where(a => a.PartnerID == 3).Sum(a => a.TotalTarget); }
            //catch { ViewBag.DevPercent = 0; }
            ViewBag.DevPercent = 0;

            ProjectSummary projectSummary = new ProjectSummary
            {
                indicatorsSummaries = await query.ToListAsync(),
                procurementProgresses = await (from Activity in _context.Activity
                                                   //  where Activity.ProcurementPlanID==1
                                               select new ProcurementProgress
                                               {
                                                   ProcurementPlanID = Activity.ProcurementPlanID,
                                                   StepReferenceNo = Activity.StepReferenceNo,
                                                   Activity = Activity.Name,
                                                   Initiated = Activity.Status >= 1 ? 1 : 0,
                                                   Completed = Activity.Status == 4 ? 1 : 0
                                                   
                                               }).ToListAsync()
            };

            ViewBag.ProcPercent = projectSummary.procurementProgresses.Sum(a => a.Initiated) * 25 / projectSummary.procurementProgresses.Count()
                                + projectSummary.procurementProgresses.Sum(a => a.Completed) * 75 / projectSummary.procurementProgresses.Count();

            return PartialView(projectSummary);
        }

        public async Task<IActionResult> SchoolList (int id,int? RID, int? DID, bool verify,int? Type, string IndicatorName)
        {
            ViewBag.IndicatorName = IndicatorName;
            var query = _context.schIndicatorStatuses.FromSql("exec IndicatorSchoolWiseStatus @RID, @DID, @IID, @Type, @verify", 
                                                                                new SqlParameter("@RID", RID == null ? (object)DBNull.Value : RID), 
                                                                                new SqlParameter("@DID", DID == null ? (object)DBNull.Value : DID),
                                                                                new SqlParameter("@IID", id),
                                                                                new SqlParameter("@verify", verify),
                                                                                 new SqlParameter("@Type", Type == null ? (object)DBNull.Value : Type)
                                                                                                     ); //.ToList<IndicatorsSummary>();

            //var query = from Ucs in _context.UCs
            //            join Schools in _context.Schools on new { UCID = Ucs.UCID } equals new { UCID = Schools.UCID }
            //            join Tehsils in _context.Tehsils
            //                  on new { Ucs.TehsilID, Column1 = Ucs.TehsilID, Column2 = Ucs.TehsilID }
            //              equals new { Tehsils.TehsilID, Column1 = Tehsils.TehsilID, Column2 = Tehsils.TehsilID }
            //            join Districts in _context.Districts
            //                  on new { Tehsils.DistrictID, Column1 = Tehsils.DistrictID, Column2 = Tehsils.DistrictID }
            //              equals new { Districts.DistrictID, Column1 = Districts.DistrictID, Column2 = Districts.DistrictID }
            //            join IncdicatorTracking in _context.IncdicatorTracking on Schools.SchoolID equals IncdicatorTracking.SchoolID into IncdicatorTracking_join
            //            from IncdicatorTracking in IncdicatorTracking_join.DefaultIfEmpty()
            //            group new { Districts, Schools, Ucs, IncdicatorTracking } by new
            //            {
            //                Districts.RegionID,
            //                Districts.DistrictName,
            //                Schools.SName,
            //                Schools.BEMIS,
            //                Schools.ClusterBEMIS,
            //                Schools.SLevel,
            //                Schools.type,
            //                Schools.SchoolID
            //            } into g
            //            orderby
            //              g.Key.RegionID,
            //              g.Key.DistrictName,
            //              g.Key.ClusterBEMIS
            //            select new School
            //            {
            //                RegName= g.Key.RegionID.ToString(),
            //                DisName=  g.Key.DistrictName,
            //                SchoolID = g.Key.SchoolID,
            //               SName= g.Key.SName,
            //               BEMIS= g.Key.BEMIS,
            //               ClusterBEMIS= g.Key.ClusterBEMIS,
            //              SLevel=  g.Key.SLevel,
            //                type = g.Key.type,
            //                Abandon = g.Max(p => (p.IncdicatorTracking.IndicatorID == 13 &&
            //                                           p.IncdicatorTracking.IsUpload==true ? true : false)),

            //            };
            return PartialView(query);
           
        }
    }
}