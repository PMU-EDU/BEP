using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Areas.Procurement.Models;
using BES.Data;
using BES.Areas.Procurement.Models.ModelViews;
using BES.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace BES.Areas.Procurement.Controllers
{
    [Area("Procurement")]
    public class ActivitiesController : Controller
    {        
        private readonly ApplicationDbContext _context;

        public ActivitiesController(ApplicationDbContext context)
        {            
            _context = context;
        }

        // GET: Procurement/Activities
        public async Task<IActionResult> Index(short PPID)
        {            
            var applicationDbContext = _context.Activity.Include(p => p.Method).Include(p => p.PProcurementPlan).Where(a => a.ProcurementPlanID == PPID);            
            ViewBag.TotalGActivities = applicationDbContext.Count(a => a.ProcurementPlanID == PPID).ToString();
            ViewBag.TotalGActivitiesCompleted = applicationDbContext.Count(a => a.ProcurementPlanID == PPID && a.Status == 3).ToString();
            ViewBag.TotalGActivitiesProcess = applicationDbContext.Count(a => a.ProcurementPlanID == PPID && a.Status == 2).ToString();
            ViewBag.TotalGActivitiesRemain = applicationDbContext.Count(a => a.ProcurementPlanID == PPID && a.Status == 1).ToString();
            ViewBag.TotalGActivitiesCancelled = applicationDbContext.Count(a => a.ProcurementPlanID == PPID && a.Status == 4).ToString();
            ViewBag.TotalGAECost = applicationDbContext.Where(a => a.ProcurementPlanID == PPID).Sum(a => ((int?)a.EstimatedCost)).ToString();
            ViewBag.TotalGAECost = string.IsNullOrEmpty(ViewBag.TotalGAECost) ? "0" : ViewBag.TotalGAECost;
            ViewBag.TotalGAACost = _context.Lot.Include(s=>s.Activity).Where(a => a.Activity.ProcurementPlanID == PPID).Sum(a => ((int?)a.ActualCost ?? 0)).ToString();
            ViewBag.TotalGAACost = string.IsNullOrEmpty(ViewBag.TotalGAACost) ? "0" : ViewBag.TotalGAACost;
            //ViewBag.TotalGACommit = (ppactivities.Where(a => a.ProcurementPlanID == 1 && a.Status == 3).Sum(a => (int?)a.ActualCost) ?? 0).ToString();
            ViewBag.TotalGACommit = _context.VLotItemDetail.Include(a => a.Activity.PProcurementPlan).Where(a => a.Activity.ProcurementPlanID == PPID).Sum(a => a.ActualUnitRate * a.FQuantity).ToString();
            ViewBag.TotalGACommit = string.IsNullOrEmpty(ViewBag.TotalGACommit) ? "0" : ViewBag.TotalGACommit;
            ViewBag.PPName = _context.ProcurementPlan.Find(PPID).Name;
            ViewBag.PPID = PPID;
            //ZongSMS obj = new ZongSMS("","","","");
            //obj.SendSingleSMS("");

            var ActivitySequence = _context.Activity.Where(a => a.ProcurementPlanID == PPID).OrderBy(a=>a.StepReferenceNo).Select(a => a.StepReferenceNo).ToList();
            var ActivityOnStep = (from c in _context.ActivityDetail
                           group c by c.Activity.StepReferenceNo into g
                           select new
                           {
                               StepReferenceNo = g.Key,                               
                               SerailNo = g.Max(a => a.Step.SerailNo)
                           }).OrderBy(a=>a.StepReferenceNo).ToList();

            int counter = 0;
            List<short> finalList = new List<short>();
            foreach(var val in ActivitySequence)
            {
                if(ActivityOnStep.Count > counter && val == ActivityOnStep.ElementAt(counter).StepReferenceNo)
                {
                    finalList.Add(ActivityOnStep.ElementAt(counter).SerailNo ?? 0);
                    counter++;
                }
                else
                {
                    finalList.Add(0); 
                }               
            }
            ViewBag.ActivitySequence = ActivitySequence;
            ViewBag.finalList = finalList;
            return View(await applicationDbContext.ToListAsync());
        }
        public ActionResult Popup(int? id, int? pointout)
        {
            ViewBag.PointOut = pointout;
            if (id == null)
            {
                var ppactivities = _context.Activity.Include(p => p.Method).Include(p => p.PProcurementPlan).Where(a => a.ProcurementPlanID == 1);
                return PartialView(ppactivities.ToList());
            }
            else
            {
                var ppactivities = _context.Activity.Include(p => p.Method).Include(p => p.PProcurementPlan).Where(a => a.ProcurementPlanID == 1 && a.Status == id);
                return PartialView(ppactivities.ToList());
            }
        }
        // GET: Procurement/Activities/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPActivityDetailList = _context.ActivityDetail.Include(a=>a.Step).Include(a => a.Activity.Method).Where(a => a.ActivityID == id).ToList();
            var pPLotList = _context.Lot.Include(a => a.Contractor).Where(a => a.ActivityID == id).ToList();
            var pPLotItemList = _context.LotItem.Include(a => a.Lot.Contractor).Where(a => a.Lot.ActivityID == id).OrderBy(a => a.Lot.lotno);
            var query = (from e in _context.VLotItemDetail
                         group e by new { e.ActivityID, e.lotId } into eg
                         select new { eg.Key.ActivityID, eg.Key.lotId, PPAddendumTypeId = eg.Max(rl => rl.AddendumTypeId), ActualDate = eg.Max(rl => rl.ActualDate), LExpiryDate = eg.Max(rl => rl.LExpiryDate), AExpiryDate = eg.Max(rl => rl.AExpiryDate), IQuantity = eg.Sum(rl => rl.IQuantity), FQuantity = eg.Sum(rl => rl.FQuantity), EstimatedUnitRate = eg.Sum(rl => (rl.EstimatedUnitRate * rl.IQuantity)), ActualUnitRate = eg.Sum(rl => (rl.ActualUnitRate * rl.FQuantity)), Attachment = eg.Max(rl => rl.Attachment), ContractorID = eg.Max(rl => rl.ContractorID), CompanyName = eg.Max(rl => rl.CompanyName), ItemName = eg.Max(rl => rl.ItemName), Unit = eg.Max(rl => rl.Unit), LotItemId = eg.Max(rl => rl.LotItemId), lotno = eg.Max(rl => rl.lotno), Addandum = eg.Max(rl => rl.Addandum) }).Where(a => a.ActivityID == id).ToList();
            List<VLotItemDetail> pPItemDetailList = new List<VLotItemDetail>();
            for (int i = 0; i < query.ToList().Count; i++)
            {
                pPItemDetailList.Add(new VLotItemDetail
                {
                    ActivityID = query[i].ActivityID,
                    ActualUnitRate = query[i].ActualUnitRate,
                    Addandum = query[i].Addandum,
                    Attachment = query[i].Attachment,
                    CompanyName = query[i].CompanyName,
                    ContractorID = query[i].ContractorID,
                    AddendumTypeId = query[i].PPAddendumTypeId,
                    ActualDate = query[i].ActualDate,
                    LExpiryDate = query[i].LExpiryDate,
                    AExpiryDate = query[i].AExpiryDate,
                    EstimatedUnitRate = query[i].EstimatedUnitRate,
                    FQuantity = query[i].FQuantity,
                    ItemName = query[i].ItemName,
                    Unit = query[i].Unit,
                    lotId = query[i].lotId,
                    lotno = query[i].lotno,
                    LotItemId = query[i].LotItemId,
                    IQuantity = query[i].IQuantity
                });
            }
            var pPAddendumLotItemList = _context.Addendum.Include(a => a.Lot.Contractor).Where(a => a.Lot.ActivityID == id).OrderBy(a => a.Lot.lotno).ToList();
            if (pPActivityDetailList == null)
            {
                return NotFound();
            }
            var result = (from p in _context.Addendum.Include(a => a.Lot).Where(a => a.Lot.ActivityID == id)
                          select new { p.LotId, p.AddendumId } into x
                          group x by new { x.LotId } into g
                          select new
                          {
                              LotId = g.Key.LotId,
                              TotalAddendum = g.Count()
                          }).ToList();
            List<int> data = new List<int>();
            foreach (var obj in result)
            {
                data.Add(obj.LotId);
                data.Add(obj.TotalAddendum);

            }
            int[] arrStrings = data.ToArray();
            ViewData["Data"] = arrStrings;
            
            var activityObj = _context.Activity.Include(a=>a.Method).Where(a=>a.ActivityID == id);
            var tuple = new Tuple<List<ActivityDetail>, List<Lot>, IEnumerable<Addendum>, IEnumerable<VLotItemDetail>,List<Activity>>(pPActivityDetailList, pPLotList, pPAddendumLotItemList.ToList(), pPItemDetailList,activityObj.ToList());
            return View(tuple);
        }

        //[Authorize(Roles = "Procurement")]
        // GET: Procurement/Activities/Create
        public IActionResult Create(short id)
        {
            ViewBag.MethodID = new SelectList(_context.Method, "MethodID", "Name");
            ViewBag.ProjectNo = new SelectList(_context.Project.Where(a=>a.ProjectNo == 2), "ProjectNo", "ProjectName");
            ViewBag.ProcurementPlanID = new SelectList(_context.ProcurementPlan.Where(a => a.ProcurementPlanID == id), "ProcurementPlanID", "Name");
            ViewBag.ProcurementFor = new SelectList(new[]
                   {
                         new { Id = "1", Name = "Schools" },
                        new { Id = "2", Name = "Office" },
                    }, "Id", "Name");
            ViewBag.ReviewType = new SelectList(new[]
                   {
                         new { Id = "Prior Review", Name = "Prior Review" },
                        new { Id = "Post Review", Name = "Post Review" },
                    }, "Id", "Name");
            Activity pPActivity = new Activity();
            pPActivity.CreatedBy = User.Identity.Name;
            ViewBag.PPID = id;
            return View(pPActivity);
        }

        // POST: Procurement/Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(short id,[Bind("ActivityID,ProcurementPlanID,StepReferenceNo,Name,LotTotal,SchoolTotal,ProcurementFor,Description,EstimatedCost,ActualCost,MethodID,ReviewType,Status,IsCenceled,Remarks,CreatedDate,CreatedBy,UpdatedBy,UpdatedDate,ProjectNo")] Activity activity)
        {
            if (ModelState.IsValid)
            {               
                var val = _context.Activity.Count(a => a.StepReferenceNo == activity.StepReferenceNo && a.ProcurementPlanID == id);
                if (val == 0)
                {
                    activity.ActualCost = 0;
                    activity.CreatedDate = DateTime.Now;
                    activity.Status = 1;
                    activity.ProcurementPlanID = id;
                    if(id == 2)
                    {
                        activity.LotTotal = 1;                        
                    }
                    _context.Add(activity);
                    await _context.SaveChangesAsync();
                    if (activity.ProcurementPlanID == 2)// where 2 is Works
                    {
                        ActivityDetailWork Obj = new ActivityDetailWork();
                        Obj.ActivityID = _context.Activity.Max(a => a.ActivityID);
                        Obj.TotalSchool = activity.SchoolTotal;
                        _context.Add(Obj);
                        await _context.SaveChangesAsync();
                    }                    
                    return RedirectToAction(nameof(Index),new { PPID = id });                    
                }
                else
                {
                    ViewBag.Error = "Activity No." + activity.StepReferenceNo.ToString() + " already exist!";
                }
            }
            ViewBag.ReviewType = new SelectList(new[]
                 {
                         new { Id = "Prior Review", Name = "Prior Review" },
                        new { Id = "Post Review", Name = "Post Review" },
                    }, "Id", "Name");
            ViewBag.ProcurementFor = new SelectList(new[]
                   {
                         new { Id = "1", Name = "Schools" },
                        new { Id = "2", Name = "Office" },
                    }, "Id", "Name", activity.ProcurementFor);
            ViewBag.MethodID = new SelectList(_context.Method, "MethodID", "Name", activity.MethodID);
            ViewBag.ProjectNo = new SelectList(_context.Project, "ProjectNo", "ProjectName", 2);
            ViewBag.ProcurementPlanID = new SelectList(_context.ProcurementPlan.Where(a => a.ProcurementPlanID == id), "ProcurementPlanID", "Name", activity.ProcurementPlanID);
            return View(activity);
        }

        // GET: Procurement/Activities/Edit/5
        //[Authorize(Roles = "Procurement")]
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            ViewData["MethodID"] = new SelectList(_context.Method, "MethodID", "Name", activity.MethodID);
            ViewData["ProjectNo"] = new SelectList(_context.Project, "ProjectNo", "ProjectName", activity.ProjectNo);
            ViewData["ProcurementPlanID"] = new SelectList(_context.ProcurementPlan.Where(a=>a.ProcurementPlanID == activity.ProcurementPlanID), "ProcurementPlanID", "Name");
            ViewBag.ReviewType = new SelectList(new[]
                   {
                         new { Id = "Prior Review", Name = "Prior Review" },
                        new { Id = "Post Review", Name = "Post Review" },
                    }, "Id", "Name", activity.ReviewType);
            ViewBag.ProcurementFor = new SelectList(new[]
                   {
                         new { Id = "1", Name = "Schools" },
                        new { Id = "2", Name = "Office" },
                    }, "Id", "Name", activity.ProcurementFor);
            activity.UpdatedBy = User.Identity.Name;
            return View(activity);
        }

        // POST: Procurement/Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("ActivityID,ProcurementPlanID,ActivityNo,StepReferenceNo,Name,LotTotal,SchoolTotal,ProcurementFor,Description,EstimatedCost,ActualCost,MethodID,ReviewType,Status,IsCenceled,Remarks,CreatedBy,CreatedDate,UpdatedDate,ProjectNo")] Activity activity)
        {
            if (id != activity.ActivityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {                    
                    activity.UpdatedDate = DateTime.Now.Date;
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.ActivityID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit),new { id = activity.ActivityID});
            }
            ViewData["MethodID"] = new SelectList(_context.Method, "MethodID", "Name", activity.MethodID);
            ViewData["ProjectNo"] = new SelectList(_context.Project, "ProjectNo", "ProjectName", activity.ProjectNo);
            ViewData["ProcurementPlanID"] = new SelectList(_context.ProcurementPlan, "ProcurementPlanID", "Name", activity.ProcurementPlanID);
            ViewBag.ReviewType = new SelectList(new[]
                   {
                         new { Id = "Prior Review", Name = "Prior Review" },
                        new { Id = "Post Review", Name = "Post Review" },
                    }, "Id", "Name", activity.ReviewType);
            ViewBag.ProcurementFor = new SelectList(new[]
                   {
                         new { Id = "1", Name = "Schools" },
                        new { Id = "2", Name = "Office" },
                    }, "Id", "Name", activity.ProcurementFor);
            return View(activity);
        }

        // GET: Procurement/Activities/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity
                .Include(a => a.Method)
                .Include(a => a.PPProject)
                .Include(a => a.PProcurementPlan)
                .FirstOrDefaultAsync(m => m.ActivityID == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Procurement/Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var activity = await _context.Activity.FindAsync(id);
            _context.Activity.Remove(activity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(short id)
        {
            return _context.Activity.Any(e => e.ActivityID == id);
        }
    }
}
