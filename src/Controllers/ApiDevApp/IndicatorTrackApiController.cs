using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BES.Models.ApiDevApp;
using BES.Models.Data;
using BES.Data;
using Microsoft.EntityFrameworkCore;
using BES.Areas.DevApp.Models;

namespace BES.Controllers.ApiDevApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndicatorTrackApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IndicatorTrackApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/IndicatorTrackApi
        [HttpGet]
        public async Task<ActionResult<pictureRepoitoryModel>> Get()
        {
            List<repositoryDetailList> repositoryDetailList = new List<repositoryDetailList>();
            repositoryDetailList image1 = new repositoryDetailList
            {
                // = 1,
                //IndicatorID = 29,
                //ImageID = 1,
                //DateTime = DateTime.Now,
                //ImagePath = "byte Array 1",
                //Latitude = 12.12,
                //Longitude = 24.24,
                //Remarks = "Remarks image 1",
                //SyncDate = DateTime.Now,

            };
            IndicatorDevApp image2 = new IndicatorDevApp
            {
                SchoolID = 1,
                IndicatorID = 29,
                ImageID = 2,
                DateTime = DateTime.Now,
                ImagePath = "byte Array 2",
                Latitude = 12.12,
                Longitude = 24.24,
                Remarks = "Remarks image 2",
                SyncDate = DateTime.Now,

            };
           // indicatorTrackDevApps.Add(image1);
           // indicatorTrackDevApps.Add(image2);
           
           
            pictureRepoitoryModel indicatorTrackDevApp = new pictureRepoitoryModel
            {
                school_id = 1,
                indicatorID = 29,
                current_date = DateTime.Now,
               
                picture_count=5,
               // repositoryDetailList= indicatorTrackDevApps

            };
            return indicatorTrackDevApp;
        }

        // GET: api/IndicatorTrackApi/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/IndicatorTrackApi
        [HttpPost]
        public async Task<ActionResult<pictureRepoitoryModel>> Post(pictureRepoitoryModel indicatorTrackDevApp)
        {
            bool status = true;
            string message = "Success";

            IndicatorTracking indicatorTracking = new IndicatorTracking
            {
                SchoolID = indicatorTrackDevApp.school_id,
                IndicatorID = indicatorTrackDevApp.indicatorID,
                IsUpload = true,
                TotalFilesUploaded = indicatorTrackDevApp.picture_count,
                DateOfUpload = DateTime.Now


            };
            _context.IncdicatorTracking.Add(indicatorTracking);
            // Saving Image code go here
            //foreach (var i in indicatorTrackDevApp.IndicatorDevApps)
            //{

            //}


            try
            {
               // await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IndicatorTrackingExists(indicatorTracking.SchoolID, indicatorTracking.IndicatorID))
                {
                              status = false;
                    message = "Record Already Exist against this school and Indicator";
                    //return Conflict();
                }
                else
                {
                    throw;
                }
                
               
                
            }

            return Ok(new { status, message });
            //  return CreatedAtAction("GetIndicatorTracking", new { id = indicatorTracking.SchoolID }, indicatorTracking);
        }


        private bool IndicatorTrackingExists(int? SchoolID, int? IndicatorID)
        {
            return _context.IncdicatorTracking.Any(e => e.SchoolID == SchoolID && e.IndicatorID==IndicatorID);
        }
    }
}
