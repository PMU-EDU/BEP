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

namespace BES.Areas.Procurement.Controllers
{
    [Area("Procurement")]
    public class AddendaController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AddendaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Procurement/Addenda
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Addendum.Include(a => a.AddendumType).Include(a => a.Lot);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Procurement/Addenda
        public async Task<IActionResult> PartialIndex(int id, int AID)
        {
            var applicationDbContext = _context.Addendum.Include(a => a.AddendumType).Include(a => a.Lot).Where(a=>a.LotId == id);
            //ViewBag.Expiry = _context.ActivityDetail.Include(a => a.Step).Where(a => a.ActivityID == AID && a.Step.SerailNo == 9).Select(a => a.ActualDate).FirstOrDefault().ToString();
            //ViewBag.AID = AID;
            return PartialView(await applicationDbContext.ToListAsync());
        }
        // GET: Procurement/Addenda/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendum = await _context.Addendum
                .Include(a => a.AddendumType)
                .Include(a => a.Lot)
                .FirstOrDefaultAsync(m => m.AddendumId == id);
            if (addendum == null)
            {
                return NotFound();
            }

            return View(addendum);
        }

        [Authorize(Roles = "Procurement")]
        // GET: Procurement/Addenda/Create
        public IActionResult Create(int id, short AID)
        {
            ViewBag.ActivityId = AID; //db.PPLots.Where(a => a.lotId == id).Select(a => a.ActivityID).FirstOrDefault();
            var result = _context.Lot
               .Where(a => a.lotId == id)
                  .Select(x => new
                  {
                      x.lotId,
                      lotDescription = "Lot No - " + x.lotno.ToString()
                  }).OrderBy(a => a.lotId);
            ViewData["LotId"] = new SelectList(result, "lotId", "lotDescription");
            ViewData["AddendumTypeId"] = new SelectList(_context.AddendumType, "AddendumTypeId", "Name");
            ViewBag.LotNo = result.Select(a => a.lotDescription).FirstOrDefault();                        
            return View();
        }

        // POST: Procurement/Addenda/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, short AID, [Bind("AddendumId,LotId,AddendumTypeId,Attachment,Remarks,ExpiryDate")] Addendum addendum, IFormFile Attachment)
        {
            if (ModelState.IsValid)
            {
                if (Attachment != null)
                {
                    var rootPath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Procurement\\");
                    string fileName = Path.GetFileName(Attachment.FileName);
                    fileName = fileName.Replace("&", "n");
                    string AName = _context.Lot.Include(a => a.Activity).Where(a => a.lotId == id).Select(a => a.Activity.Name).FirstOrDefault().ToString();
                    AName = AName.Replace("&", "n");
                    var PPName = _context.ProcurementPlan.Find(_context.Activity.Find(AID).ProcurementPlanID).Name;
                    int NextID = (_context.Addendum.Max(a => (int?)a.AddendumId) ?? 1) + 1;
                    addendum.Attachment = Path.Combine("/Documents/Procurement/", PPName + "/" + "//" + AName + "//Addendum//" + NextID.ToString() + "//" + fileName);//Server Path                
                    string sPath = Path.Combine(rootPath + PPName + "/" + AName + "//Addendum//" + NextID.ToString());
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
                if (addendum.AddendumTypeId == 1)
                {
                    addendum.ExpiryDate = _context.ActivityDetail.Include(a => a.Step).Where(a => a.ActivityID == AID && a.Step.SerailNo == 9).Select(a => a.ActualDate).FirstOrDefault();
                }
                _context.Add(addendum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create), new { id, AID });
            }
            ViewData["AddendumTypeId"] = new SelectList(_context.Set<AddendumType>(), "AddendumTypeId", "AddendumTypeId", addendum.AddendumTypeId);
            ViewData["LotId"] = new SelectList(_context.Lot, "lotId", "lotId", addendum.LotId);
            return View(addendum);
        }

        [Authorize(Roles = "Procurement")]
        // GET: Procurement/Addenda/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendum = await _context.Addendum.FindAsync(id);
            if (addendum == null)
            {
                return NotFound();
            }
            ViewData["AddendumTypeId"] = new SelectList(_context.Set<AddendumType>(), "AddendumTypeId", "AddendumTypeId", addendum.AddendumTypeId);
            ViewData["LotId"] = new SelectList(_context.Lot, "lotId", "lotId", addendum.LotId);
            return View(addendum);
        }

        // POST: Procurement/Addenda/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AddendumId,LotId,AddendumTypeId,Attachment,Remarks,ExpiryDate")] Addendum addendum)
        {
            if (id != addendum.AddendumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addendum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddendumExists(addendum.AddendumId))
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
            ViewData["AddendumTypeId"] = new SelectList(_context.Set<AddendumType>(), "AddendumTypeId", "AddendumTypeId", addendum.AddendumTypeId);
            ViewData["LotId"] = new SelectList(_context.Lot, "lotId", "lotId", addendum.LotId);
            return View(addendum);
        }

        // GET: Procurement/Addenda/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendum = await _context.Addendum
                .Include(a => a.AddendumType)
                .Include(a => a.Lot)
                .FirstOrDefaultAsync(m => m.AddendumId == id);
            if (addendum == null)
            {
                return NotFound();
            }

            return View(addendum);
        }

        // POST: Procurement/Addenda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var addendum = await _context.Addendum.FindAsync(id);
            _context.Addendum.Remove(addendum);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddendumExists(int id)
        {
            return _context.Addendum.Any(e => e.AddendumId == id);
        }
    }
}
