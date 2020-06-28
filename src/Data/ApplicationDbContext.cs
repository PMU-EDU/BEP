using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BES.Areas.Procurement.Models;
using BES.Models.Data;
using BES.Areas.LMS.Models;
using BES.Models.Reports;

namespace BES.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet< BES.Models.Data.School> Schools {get;set;}
        public DbSet< BES.Models.Data.UC> UCs {get;set;}
        public DbSet< BES.Models.Data.Tehsil> Tehsils {get;set;}
        public DbSet< BES.Models.Data.District> Districts {get;set;}
        public DbSet< BES.Models.Data.Region> Regions {get;set; }
        public DbSet<BES.Models.Data.SchoolClass> SchoolClasses { get; set; }

        public DbSet<BES.Models.Data.BaselineGeneral> BaselineGenerals { get; set; }
        public DbSet<BES.Models.Data.BLEnrollment> BLEnrollments { get; set; }
        public DbSet<BES.Models.Data.BLFacilitiesInfo> BLFacilitiesInfoes { get; set; }
        public DbSet<BES.Models.Data.BLFeederDetail> BLFeederDetails { get; set; }
        public DbSet<BES.Models.Data.BLFeederSchool> BLFeederSchools { get; set; }
        public DbSet<BES.Models.Data.BLLandAvailable> BLLandAvailables { get; set; }
        public DbSet<BES.Models.Data.BLPTSMCInfo> BLPTSMCInfoes { get; set; }
        public DbSet<BES.Models.Data.BLTeacherDetail> BLTeacherDetails { get; set; }
        public DbSet<BES.Models.Data.BLTeacherPost> BLTeacherPosts { get; set; }
        public DbSet<BES.Models.Data.BLTeacherPresent> BLTeacherPresents { get; set; }
        public DbSet<BES.Models.Data.BLTeacherSection> BLTeacherSections { get; set; }

        public DbSet<BES.Models.Data.ESSChecklist> ESSChecklist { get; set; }

        public DbSet<BES.Models.Reports.BLEUDetailView> BLEUDetailViews { get; set; }
        public DbSet<BES.Models.Reports.IndicatorsSummary> IndicatorsSummaries { get; set; }
        public DbSet<BES.Models.Reports.IndicatorsTotalTarget> indicatorsTotalTargets { get; set; }
        public DbSet<BES.Models.Data.SchIndicatorStatus> schIndicatorStatuses { get; set; }
        public DbSet<BES.Models.Data.Contractor> Contractor { get; set; }
        public DbSet<BES.Models.Data.Partner> Partners { get; set; }
        public DbSet<BES.Models.Data.Section> Section { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<BES.Areas.Procurement.Models.ActivityDetail>().HasKey(table => new {
                table.ActivityID,
                table.StepID
            });

            builder.Entity<BES.Models.Data.IndicatorTracking>().HasKey(table => new {
                table.SchoolID,
                table.IndicatorID
            });
            builder.Entity<BES.Models.Data.IndicatorDevApp>().HasKey(table => new {
                table.ImageID,
                table.SchoolID,
                table.IndicatorID
            });

            builder.Entity<BES.Models.Data.Tool5>().HasKey(table => new{
                table.SchoolID,
                table.Quarter
            });

            builder.Entity<BES.Models.Data.Tool5Detail>().HasKey(table => new {
                table.SchoolID,
                table.Quarter,
                table.ClassID
            });

        }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.Unit> Unit { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.Project> Project { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.Method> Method { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.ProcurementPlan> ProcurementPlan { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.Step> Step { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.Activity> Activity { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.ActivityDetail> ActivityDetail { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.Lot> Lot { get; set; }
        public DbSet<BES.Areas.Procurement.Models.ActivityDetailWork> ActivityDetailWork { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.LotItem> LotItem { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.LotItemImage> LotItemImage { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.Location> Location { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.Addendum> Addendum { get; set; }
        public DbSet<BES.Areas.Procurement.Models.AddendumType> AddendumType { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.AddendumDetail> AddendumDetail { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.SCManagement> SCManagement { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }
        //public DbSet<BES.Models.Data.>  { get; set; }


        public DbSet<BES.Areas.Procurement.Models.SCManagementItem> SCManagementItem { get; set; }
        public DbSet<BES.Areas.Procurement.Models.ModelViews.VLotItemDetail> VLotItemDetail { get; set; }
        public DbSet<BES.Models.Data.Indicator> Indicator { get; set; }
        public DbSet<BES.Models.Data.IndicatorTracking> IncdicatorTracking { get; set; }
        public DbSet<BES.Models.Data.IndicatorDevApp> IndicatorDevApp { get; set; }

        public DbSet<BES.Models.Data.TeacherProfile> TeacherProfile { get; set; }
        public DbSet<BES.Areas.Procurement.Models.WorkSchool> WorkSchool { get; set; }
        public DbSet<BES.Areas.Procurement.Models.AddendumWorks> AddendumWorks { get; set; }
        public DbSet<BES.Models.Data.Contact> Contact { get; set; }
        public DbSet<BES.Areas.Procurement.Models.AddendumWorkDetail> AddendumWorkDetail { get; set; }
        public DbSet<BES.Areas.LMS.Models.Employee> Employee { get; set; }
        public DbSet<BES.Areas.LMS.Models.EmpLeaveSummary> EmpLeaveSummary { get; set; }
        public DbSet<BES.Areas.LMS.Models.View_Models.LeaveSummaries> LeaveSummaries { get; set; }
        public DbSet<BES.Areas.LMS.Models.LeaveType> LeaveType { get; set; }
        public DbSet<BES.Areas.LMS.Models.LeaveRequest> LeaveRequest { get; set; }
        public DbSet<BES.Models.Data.TeacherPost> TeacherPost { get; set; }
        public DbSet<BES.Models.ApiDevApp.ApiUser> ApiUser { get; set; }
        public DbSet<BES.Models.Data.Tool5> Tool5 { get; set; }
        public DbSet<BES.Models.Data.Tool5Detail> Tool5Detail { get; set; }
        public DbSet<BES.Models.Reports.CumulativeRF> CumulativeRF { get; set; }
    }
}
