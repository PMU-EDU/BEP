using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Areas.Procurement.Models;
using BES.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using BES.Models.Data;

namespace BES.Areas.Procurement.Controllers
{
    [Area("Procurement")]
    public class ActivityDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivityDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Procurement/ActivityDetails
        public async Task<IActionResult> Index(short? id)
        {
            ViewBag.ActivityID = id;
            var activityDetail = _context.ActivityDetail.Include(p => p.Activity).Include(p => p.Step).Where(p => p.ActivityID == id);
            int totalSteps = _context.Step.Where(p => p.ProcurementPlanID == _context.Activity.Where(a=>a.ActivityID == id).Select(a=>a.ProcurementPlanID).FirstOrDefault()).Count();
            if (totalSteps > 0 && activityDetail.Count() == totalSteps)
            {
                ViewBag.IsReachedLast = true;
            }
            else { ViewBag.IsReachedLast = false; }
            return PartialView(await activityDetail.ToListAsync());
        }

        // GET: Procurement/ActivityDetails/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityDetail = await _context.ActivityDetail
                .Include(a => a.Activity)
                .Include(a => a.Step)
                .FirstOrDefaultAsync(m => m.ActivityID == id);
            if (activityDetail == null)
            {
                return NotFound();
            }

            return View(activityDetail);
        }

        public short nextStepID(short stepNo, short PPID)
        {
            var CurStep = _context.Step.Where(a => a.SerailNo == stepNo && a.ProcurementPlanID == PPID).FirstOrDefault();
            short nextStepID = _context.Step.Where(a => a.ProcurementPlanID == CurStep.ProcurementPlanID && a.SerailNo == (CurStep.SerailNo + 1)).Select(a => a.StepID).FirstOrDefault();
            return (nextStepID);
        }
        //[Authorize(Roles = "Procurement")]
        // GET: Procurement/ActivityDetails/Create
        public IActionResult Create(short id)
        {
            ActivityDetail activityDetail = new ActivityDetail();
            activityDetail.ActivityID = id;
            short stepNo = (short)_context.ActivityDetail.Count(a => a.ActivityID == id);
            Activity activity = _context.Activity.Find(id);            
            if (stepNo == 0)
            {
                if (activity.ReviewType == "Post Review")
                {
                    stepNo++;
                }
                //PProcurementPlan plan = db.PProcurementPlans.Find(activity.ProcurementPlanID);
                Step step = _context.Step.Where(a => a.ProcurementPlanID == activity.ProcurementPlanID).FirstOrDefault();
                activityDetail.StepID = step.StepID;
            }
            else
            {
                activityDetail.StepID = nextStepID(stepNo, activity.ProcurementPlanID);
                if (activity.ReviewType == "Post Review" && stepNo == 6)
                {
                    stepNo++;
                }
            }

            activityDetail.Activity = _context.Activity.Find(id);
            activityDetail.Step = _context.Step.Find(activityDetail.StepID);
            ViewBag.PPName = _context.ProcurementPlan.Find(activityDetail.Step.ProcurementPlanID).Name;
            activityDetail.PlannedDate = DateTime.Now;
            activityDetail.CreatedDate = DateTime.Now;
            activityDetail.CreatedBy = User.Identity.Name;
            ViewBag.Status = "0";
            if ((stepNo+1) == 9)
            {
                var val = _context.Lot.Where(a => a.ActivityID == activity.ActivityID && a.IsMatched == false).Count();
                ViewBag.Status = (val > 0 ? "0" : "1");
            }
            return View(activityDetail);
        }

        // POST: Procurement/ActivityDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string PPName, [Bind("StepID,ActivityID,NotApplicable,PlannedDate,ActualDate,Attachment,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] ActivityDetail activityDetail, IFormFile Attachment)
        {
            if (ModelState.IsValid)
            {
                if (activityDetail.NotApplicable == true)
                {
                    activityDetail.Attachment = null;
                    activityDetail.ActualDate = null;
                    activityDetail.PlannedDate = null;
                }
                else
                {
                    if (Attachment != null)
                    {
                        var rootPath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Procurement\\");
                        string fileName = Path.GetFileName(Attachment.FileName);
                        fileName = fileName.Replace("&", "n");
                        string AName = _context.Activity.Find(activityDetail.ActivityID).Name;
                        AName = AName.Replace("&", "n");
                        //var PPName = _context.ProcurementPlan.Find(activityDetail.Step.ProcurementPlanID).Name;
                        activityDetail.Attachment = Path.Combine("/Documents/Procurement/", PPName + "/" + "//" + AName + "//" + activityDetail.StepID + "//" + fileName);//Server Path                
                                                                                                                                                          //_context.ActivityDetail.Add(activityDetail);
                        string sPath = Path.Combine(rootPath + PPName + "/" + AName + "/", activityDetail.StepID.ToString());
                        if (!System.IO.Directory.Exists(sPath))
                        {
                            System.IO.Directory.CreateDirectory(sPath);
                        }
                        string FullPathWithFileName = Path.Combine(sPath, fileName);
                        using (var stream = new FileStream(FullPathWithFileName, FileMode.Create))
                        {
                            await Attachment.CopyToAsync(stream);
                        }
                    }
                }
                activityDetail.CreatedDate = DateTime.Now.Date;
                //Activity activityObj = _context.Activity.Find(activityDetail.ActivityID);                
                //_context.Update(activityObj);
                _context.Add(activityDetail);
                await _context.SaveChangesAsync();
                if (activityDetail.StepID == 1)
                {
                    Activity Obj = _context.Activity.Find(activityDetail.ActivityID);
                    Obj.Status = 2;
                    _context.Update(Obj);

                }                
                return RedirectToAction(nameof(Edit),"Activities",new { id = activityDetail.ActivityID});
            }           
            return View(activityDetail);
        }
        
        //[Authorize(Roles = "Procurement")]
        // GET: Procurement/ActivityDetails/Edit/5
        public async Task<IActionResult> Edit(short ActivityID, short StepID)
        {
            if (ActivityID == 0 || StepID == 0)
            {
                return NotFound();
            }

            var activityDetail = await _context.ActivityDetail.Where(a=>a.ActivityID== ActivityID&& a.StepID==StepID).FirstOrDefaultAsync();
            activityDetail.Activity = _context.Activity.Find(ActivityID);
            activityDetail.Step = _context.Step.Find(StepID);
            activityDetail.UpdatedBy = User.Identity.Name;
            ViewBag.PPName = _context.ProcurementPlan.Find(activityDetail.Step.ProcurementPlanID).Name;
            if (activityDetail == null)
            {
                return NotFound();
            }           
            ViewData["ContractorID"] = new SelectList(_context.Contractor.Where(a => a.ContractorTypeID == 1).ToList(), "ContractorID", "CompanyName");
            ViewBag.CName = _context.ActivityDetailWork.Include(a=>a.Contractor).Where(a => a.ActivityID == ActivityID).Select(a => a.Contractor.CompanyName).FirstOrDefault();
            ViewBag.CEDate = _context.ActivityDetailWork.Where(a => a.ActivityID == ActivityID).Select(a => a.ExpiryDate.ToString() ?? "").FirstOrDefault();
            return View(activityDetail);
        }

        // POST: Procurement/ActivityDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string PPName, int ActualCost, [Bind("StepID,ActivityID,NotApplicable,PlannedDate,ActualDate,Attachment,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] ActivityDetail activityDetail, IFormFile Attachment, DateTime ExpiryDate, short CID)
        {           

            if (ModelState.IsValid)
            {
                try
                {
                    if (activityDetail.NotApplicable == true)
                    {
                        activityDetail.Attachment = null;
                        activityDetail.ActualDate = null;
                        activityDetail.PlannedDate = null;
                    }
                    else
                    {
                        if (Attachment != null)
                            {
                            var rootPath = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Procurement\\");
                            string fileName = Path.GetFileName(Attachment.FileName);
                            fileName = fileName.Replace("&", "n");
                            string AName = _context.Activity.Find(activityDetail.ActivityID).Name;
                            AName = AName.Replace("&", "n");
                            //var PPName = _context.ProcurementPlan.Find(activityDetail.Step.ProcurementPlanID).Name;                                    
                            activityDetail.Attachment = Path.Combine("/Documents/Procurement/", PPName + "/" + "//" + AName + "//" + activityDetail.StepID + "//" + fileName);//Server Path                
                            //_context.ActivityDetail.Add(activityDetail);
                            string sPath = Path.Combine(rootPath + PPName + "/" + AName + "/", activityDetail.StepID.ToString());
                            if (!System.IO.Directory.Exists(sPath))
                            {
                                System.IO.Directory.CreateDirectory(sPath);
                            }
                            string FullPathWithFileName = Path.Combine(sPath, fileName);
                            using (var stream = new FileStream(FullPathWithFileName, FileMode.Create))
                            {
                                await Attachment.CopyToAsync(stream);
                            }
                        }
                    }
                    Activity activityObj = _context.Activity.Find(activityDetail.ActivityID);
                    activityObj.ActualCost = ActualCost;                   
                    //_context.Update(activityObj);
                    _context.Update(activityDetail);
                    await _context.SaveChangesAsync();
                    if (activityDetail.StepID == 21)
                    {
                        ActivityDetailWork Obj = _context.ActivityDetailWork.Where(a => a.ActivityID == activityDetail.ActivityID).FirstOrDefault();
                        Obj.ContractorID = CID;
                        Obj.ExpiryDate = ExpiryDate;                        
                        _context.Update(Obj);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Edit), new { activityDetail.ActivityID, activityDetail.StepID });
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityDetailExists(activityDetail.ActivityID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if(activityDetail.StepID == 8)
                {
                    return RedirectToAction(nameof(Edit), new { activityDetail.ActivityID, activityDetail.StepID });
                }
                else
                {
                    return RedirectToAction(nameof(Edit), "Activities", new { id = activityDetail.ActivityID });
                }                
            }
            ViewData["ActivityID"] = new SelectList(_context.Activity, "ActivityID", "Description", activityDetail.ActivityID);
            ViewData["StepID"] = new SelectList(_context.Step, "StepID", "StepID", activityDetail.StepID);
            return View(activityDetail);
        }

        // GET: Procurement/ActivityDetails/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityDetail = await _context.ActivityDetail
                .Include(a => a.Activity)
                .Include(a => a.Step)
                .FirstOrDefaultAsync(m => m.ActivityID == id);
            if (activityDetail == null)
            {
                return NotFound();
            }

            return View(activityDetail);
        }

        // POST: Procurement/ActivityDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var activityDetail = await _context.ActivityDetail.FindAsync(id);
            _context.ActivityDetail.Remove(activityDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityDetailExists(short id)
        {
            return _context.ActivityDetail.Any(e => e.ActivityID == id);
        }
    }
}
