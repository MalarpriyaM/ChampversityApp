using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Champversity.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Champversity.Web.Controllers
{
    public class ReportData
    {
        public int TotalApplications { get; set; }
     public int ThisMonthApplications { get; set; }
        public List<StatusStat> StatusStats { get; set; } = new();
        public List<UniversityStat> UniversityStats { get; set; } = new();
        public List<TaskStat> TaskStats { get; set; } = new();
        public List<RecentActivityItem> RecentActivity { get; set; } = new();
    }

    public class StatusStat
    {
      public string Status { get; set; } = string.Empty;
 public int Count { get; set; }
    }

    public class UniversityStat
    {
        public string University { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TaskStat
    {
        public string Status { get; set; } = string.Empty;
  public int Count { get; set; }
    }

    public class RecentActivityItem
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
      public string University { get; set; } = string.Empty;
    public string ApplicationStatus { get; set; } = string.Empty;
     public DateTime ApplicationSubmittedDate { get; set; }
    }

    [Authorize(Roles = "Admin,Staff")]
    [Route("Admin/[controller]")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ReportsController(ApplicationDbContext dbContext)
        {
         _dbContext = dbContext;
        }

      [Route("")]
   [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var reportData = await GenerateReportDataAsync();
        return View(reportData);
        }

[Route("Applications")]
        public async Task<IActionResult> ApplicationsReport()
        {
       var applications = await _dbContext.Students
          .OrderByDescending(s => s.ApplicationSubmittedDate)
          .ToListAsync();

     return View(applications);
        }

[Route("Universities")]
        public async Task<IActionResult> UniversitiesReport()
        {
       var universityStats = await _dbContext.Students
     .Where(s => !string.IsNullOrEmpty(s.University))
        .GroupBy(s => s.University)
   .Select(g => new
     {
     University = g.Key,
      TotalApplications = g.Count(),
         Submitted = g.Count(s => s.ApplicationStatus == "Submitted"),
       SentToUniversity = g.Count(s => s.ApplicationStatus == "Sent to University"),
             InterviewSlots = g.Count(s => s.ApplicationStatus == "Interview Slots Received"),
          Completed = g.Count(s => s.ApplicationStatus == "Interview Completed")
    })
                .OrderByDescending(u => u.TotalApplications)
        .ToListAsync();

            return View(universityStats);
        }

 [Route("Performance")]
      public async Task<IActionResult> PerformanceReport()
        {
            var performanceData = await GeneratePerformanceDataAsync();
    return View(performanceData);
        }

        [Route("Tasks")]
        public async Task<IActionResult> TasksReport()
        {
   var taskStats = await _dbContext.ManualTasks
     .GroupBy(t => t.Status)
    .Select(g => new
          {
      Status = g.Key,
           Count = g.Count(),
              Tasks = g.OrderByDescending(t => t.CreatedDate).ToList()
                })
                .ToListAsync();

 return View(taskStats);
   }

 private async Task<ReportData> GenerateReportDataAsync()
        {
      var totalApplications = await _dbContext.Students.CountAsync();
            var thisMonth = DateTime.Now.Month;
            var thisYear = DateTime.Now.Year;
         var thisMonthApplications = await _dbContext.Students
         .CountAsync(s => s.ApplicationSubmittedDate.Month == thisMonth && s.ApplicationSubmittedDate.Year == thisYear);

            var statusStats = await _dbContext.Students
        .GroupBy(s => s.ApplicationStatus)
   .Select(g => new StatusStat { Status = g.Key, Count = g.Count() })
     .ToListAsync();

            var universityStats = await _dbContext.Students
  .Where(s => !string.IsNullOrEmpty(s.University))
        .GroupBy(s => s.University)
             .Select(g => new UniversityStat { University = g.Key, Count = g.Count() })
 .OrderByDescending(u => u.Count)
                .Take(10)
                .ToListAsync();

            var taskStats = await _dbContext.ManualTasks
          .GroupBy(t => t.Status)
     .Select(g => new TaskStat { Status = g.Key, Count = g.Count() })
                .ToListAsync();

 var recentActivity = await _dbContext.Students
      .OrderByDescending(s => s.ApplicationSubmittedDate)
         .Take(10)
                .Select(s => new RecentActivityItem
 {
         Id = s.Id,
           StudentName = s.FirstName + " " + s.LastName,
 University = s.University,
  ApplicationStatus = s.ApplicationStatus,
         ApplicationSubmittedDate = s.ApplicationSubmittedDate
       })
   .ToListAsync();

 return new ReportData
      {
        TotalApplications = totalApplications,
      ThisMonthApplications = thisMonthApplications,
                StatusStats = statusStats,
    UniversityStats = universityStats,
              TaskStats = taskStats,
     RecentActivity = recentActivity
    };
        }

        private async Task<object> GeneratePerformanceDataAsync()
     {
         var last30Days = DateTime.Now.AddDays(-30);

      var dailyStats = await _dbContext.Students
           .Where(s => s.ApplicationSubmittedDate >= last30Days)
.GroupBy(s => s.ApplicationSubmittedDate.Date)
                .Select(g => new
     {
            Date = g.Key,
         Applications = g.Count()
            })
      .OrderBy(d => d.Date)
      .ToListAsync();

            var processingTimes = await _dbContext.Students
          .Where(s => s.ApplicationStatus == "Interview Slots Received")
              .Select(s => new
   {
          s.Id,
            StudentName = s.FirstName + " " + s.LastName,
       SubmittedDate = s.ApplicationSubmittedDate,
           s.ApplicationStatus
          })
       .ToListAsync();

    var universityResponseTimes = await _dbContext.UniversityResponses
      .Where(r => r.IsProcessed)
    .Select(r => new
           {
   r.StudentId,
           r.UniversityName,
         r.ReceivedDate,
     ResponseTime = "Mock - 1-3 days" // In real scenario, calculate from submission to response
           })
       .ToListAsync();

            return new
            {
         DailyStats = dailyStats,
ProcessingTimes = processingTimes,
         UniversityResponseTimes = universityResponseTimes
   };
        }
    }
}