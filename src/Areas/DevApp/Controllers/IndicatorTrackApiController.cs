using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BES.Areas.DevApp.Models;
using BES.Data;
using BES.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;

namespace BES.Areas.DevApp.Controllers
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
        public async Task<ActionResult<schoolIndicatorPictureRepository>> Get()
        //public async Task<ActionResult<pictureRepoitoryModel>> Get()
        {
            List<repositoryDetailList> repositoryDetailList = new List<repositoryDetailList>();
            repositoryDetailList image1 = new repositoryDetailList
            {
                current_date =  DateTime.Now.ToString(),
                picture_latitude = 11.11,
                picture_logitude = 22.22,
                picture_comment = "Picure Comments",
                picture_data =  new sbyte[] { -1, 2, 3, 4, 5, 6, 7 },
                
            };

            repositoryDetailList.Add(image1);
            repositoryDetailList.Add(image1);
           


            pictureRepoitoryModel pictureRepoitory = new pictureRepoitoryModel
            {
                school_id = 1,
                indicatorID = 29,
                current_date = DateTime.Now.ToString(),
                pr_id=1,
                picture_count = 5,
               //  repositoryDetailList= repositoryDetailList

            };
            List<pictureRepoitoryModel> IndicatorPictureRepository = new List<pictureRepoitoryModel>();
            IndicatorPictureRepository.Add(pictureRepoitory);
            //schoolIndicatorPictureRepository.Add(pictureRepoitory);

            List<PictureRepository> schoolIndicatorPictureRepository = new List<PictureRepository>();
            PictureRepository SchoolIndicatorRepository = new PictureRepository
            {
                pictureRepoitoryModel = pictureRepoitory,
                repositoryDetailList = repositoryDetailList
            };
            schoolIndicatorPictureRepository.Add(SchoolIndicatorRepository);
            schoolIndicatorPictureRepository schoolIndicatorPictureRepositoryList = new schoolIndicatorPictureRepository()
            {
                schoolIndicatorPictureRepositoryList=schoolIndicatorPictureRepository
            };
            
            return schoolIndicatorPictureRepositoryList;
        }

        // POST: api/IndicatorTrackApi
        [HttpPost]
        public async Task<ActionResult<pictureRepoitoryModel>> Post(schoolIndicatorPictureRepository schoolIndicatorPictureRepositoryList)
        {
            bool status = true;
            string message = "Success";

           // List<IndicatorDevApp> indicatorDevAppList = new List<IndicatorDevApp>();
            foreach (var RepoList in schoolIndicatorPictureRepositoryList.schoolIndicatorPictureRepositoryList)
            {
                var repo = RepoList.pictureRepoitoryModel;

                //Check if record already exist 
                if (IndicatorTrackingExists(repo.school_id, repo.indicatorID))
                {
                    status = false;
                    message = "Record Already Exist of School ID: " + repo.school_id + "and Indicator ID:" + repo.indicatorID;
                    //return Conflict();
                    return Ok(new { status, message });
                }

                IndicatorTracking indicatorTracking = new IndicatorTracking
                {
                    IndicatorID = repo.indicatorID,
                    SchoolID = repo.school_id,
                    DateOfUpload = Convert.ToDateTime( repo.current_date),
                    IsUpload = true,
                    ReUpload = false,
                    Verified = false,
                    CreateDate = DateTime.Now,
                    TotalFilesUploaded = repo.picture_count,
                };
                _context.Add(indicatorTracking);
                var school = _context.Schools.Find(repo.school_id);
                if (school.NewConstruction && repo.indicatorID<35)
                { school.CurrentStage += 1; }
                if(school.RepairRennovation && repo.indicatorID>35)
                {
                    school.RepairRennovationStatus += 1;
                }
                _context.Update(school);

                short i = 1;
                //
                string District = _context.Schools.Include(a => a.UC.Tehsil.District)
                                 .Where(a => a.SchoolID == repo.school_id)
                               .Select(a => a.UC.Tehsil.District.DistrictName).FirstOrDefault();
                //string 
                var rootPath = Path.Combine(
                               Directory.GetCurrentDirectory(), "wwwroot\\Documents\\DevelopmentApp\\");

                //string sPath = Path.Combine(rootPath + District + "/" + iID + "/", sID.ToString());
                string sPath = Path.Combine(rootPath + District + "/" + repo.indicatorID + "/",repo.school_id.ToString());
                if (!System.IO.Directory.Exists(sPath) & RepoList.repositoryDetailList.Any() )
                {
                    System.IO.Directory.CreateDirectory(sPath);
                }
                List<IndicatorDevApp> IndicatorDevAppList = new List<IndicatorDevApp>();
                foreach (var repoDetail in RepoList.repositoryDetailList)
                {
                    //byte[] byteImage = new byte[repoDetail.picture_data.Length];
                    //Buffer.BlockCopy(repoDetail.picture_data, 0, byteImage, 0, repoDetail.picture_data.Length);
                    string FullPathWithFileName = Path.Combine(sPath,repo.school_id+"-"+ i + ".jpg");
                    IndicatorDevApp indicatorDevApp = new IndicatorDevApp
                    {

                        ImageID = i++,
                        SchoolID = repo.school_id,
                        IndicatorID = repo.indicatorID,
                        ImagePath = FullPathWithFileName,
                        //ImageByte = repoDetail.picture_path,
                        //ImagePath= repoDetail.picture_path,
                        Longitude = repoDetail.picture_logitude,
                        Latitude = repoDetail.picture_latitude,
                        DateTime = Convert.ToDateTime( repoDetail.current_date),
                        SyncDate = DateTime.Now,
                        Remarks = repoDetail.picture_comment,

                    };

                    _context.Add(indicatorDevApp);
                   // await _context.SaveChangesAsync();
                    

                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(repoDetail.picture_path)))
                    {
                        using (Bitmap bm2 = new Bitmap(ms))
                        {
                            //bm2.SetPropertyItem() 
                            Geotag(bm2,repoDetail.picture_latitude,repoDetail.picture_logitude,Convert.ToDateTime( repoDetail.current_date))
                                .Save(FullPathWithFileName);
                        }
                    }
                }
            try
            {
                 await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IndicatorTrackingExists(repo.school_id, repo.indicatorID))
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
 }
            
            return Ok(new { status, message });
            //  return CreatedAtAction("GetIndicatorTracking", new { id = indicatorTracking.SchoolID }, indicatorTracking);
        }

        static Image Geotag(Image original, double lat, double lng, DateTime dateTime)
        {
            // These constants come from the CIPA DC-008 standard for EXIF 2.3
            const short ExifTypeByte = 1;
            const short ExifTypeAscii = 2;
            const short ExifTypeRational = 5;

            const int ExifTagGPSVersionID = 0x0000;
            const int ExifTagGPSLatitudeRef = 0x0001;
            const int ExifTagGPSLatitude = 0x0002;
            const int ExifTagGPSLongitudeRef = 0x0003;
            const int ExifTagGPSLongitude = 0x0004;
            const int ExifTagGPSDateStamp = 0x001d;

            char latHemisphere = 'N';
            if (lat < 0)
            {
                latHemisphere = 'S';
                lat = -lat;
            }
            char lngHemisphere = 'E';
            if (lng < 0)
            {
                lngHemisphere = 'W';
                lng = -lng;
            }

            MemoryStream ms = new MemoryStream();
            original.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);

            Image img = Image.FromStream(ms);
            AddProperty(img, ExifTagGPSVersionID, ExifTypeByte, new byte[] { 2, 3, 0, 0 });
            AddProperty(img, ExifTagGPSLatitudeRef, ExifTypeAscii, new byte[] { (byte)latHemisphere, 0 });
            AddProperty(img, ExifTagGPSLatitude, ExifTypeRational, ConvertToRationalTriplet(lat));
            AddProperty(img, ExifTagGPSLongitudeRef, ExifTypeAscii, new byte[] { (byte)lngHemisphere, 0 });
            AddProperty(img, ExifTagGPSLongitude, ExifTypeRational, ConvertToRationalTriplet(lng));
            AddProperty(img, ExifTagGPSDateStamp, ExifTypeAscii, BitConverter.GetBytes(dateTime.Ticks));
            return img;
        }

        static byte[] ConvertToRationalTriplet(double value)
        {
            int degrees = (int)Math.Floor(value);
            value = (value - degrees) * 60;
            int minutes = (int)Math.Floor(value);
            value = (value - minutes) * 60 * 100;
            int seconds = (int)Math.Round(value);
            byte[] bytes = new byte[3 * 2 * 4]; // Degrees, minutes, and seconds, each with a numerator and a denominator, each composed of 4 bytes
            int i = 0;
            Array.Copy(BitConverter.GetBytes(degrees), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(minutes), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(seconds), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(100), 0, bytes, i, 4);
            return bytes;
        }

        static void AddProperty(Image img, int id, short type, byte[] value)
        {
            PropertyItem pi = img.PropertyItems[0];
            pi.Id = id;
            pi.Type = type;
            pi.Len = value.Length;
            pi.Value = value;
            img.SetPropertyItem(pi);
        }

        public static Bitmap ByteArrayToImage(byte[] source)
        {
            using (var ms = new MemoryStream(source))
            {
                return new Bitmap(ms);
            }
        }
        public string SbyteToString (sbyte[] arr)
        {
            string buffer = "[";
            foreach(var a in arr)
            {
                buffer += a.ToString() + ",";
            }
            buffer += "]";
            return buffer;
        }

    
        private bool IndicatorTrackingExists(int? SchoolID, int? IndicatorID)
        {
            return _context.IncdicatorTracking.Any(e => e.SchoolID == SchoolID && e.IndicatorID == IndicatorID);
        }


    }
}
