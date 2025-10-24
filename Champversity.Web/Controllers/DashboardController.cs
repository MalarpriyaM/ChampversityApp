using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Champversity.DataAccess;
using Champversity.Web.Models;

namespace Champversity.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("Admin/[controller]")]
    public class DashboardController : Controller
 {
     private readonly ApplicationDbContext _dbContext;
    private readonly IManualTaskService _manualTaskService;

  public DashboardController(ApplicationDbContext dbContext, IManualTaskService manualTaskService)
   {
    _dbContext = dbContext;
    _manualTaskService = manualTaskService;
        }

 [Route("")]
 [Route("Index")]
    public async Task<IActionResult> Index()
  {
    var viewModel = new DashboardViewModel();

 // Get statistics
      viewModel.TotalApplications = await _dbContext.Students.CountAsync();
  viewModel.PendingTasks = await _dbContext.ManualTasks
     .CountAsync(t => t.Status == "Pending" || t.Status == "InProgress");
   viewModel.InterviewSlotsAvailable = await _dbContext.InterviewSlots
 .CountAsync(s => !s.IsSelected);
viewModel.UniversityResponsesPending = await _dbContext.UniversityResponses
.CountAsync(r => !r.IsProcessed);

   // Get recent applications
    var recentApplications = await _dbContext.Students
     .OrderByDescending(s => s.ApplicationSubmittedDate)
 .Take(10)
          .Select(s => new RecentApplicationViewModel
 {
  Id = s.Id,
       StudentName = $"{s.FirstName} {s.LastName}",
  University = s.University,
       Status = s.ApplicationStatus,
      SubmittedDate = s.ApplicationSubmittedDate
        })
      .ToListAsync();

 viewModel.RecentApplications = recentApplications;

            // Get pending tasks
     var pendingTasks = await _dbContext.ManualTasks
      .Where(t => t.Status == "Pending" || t.Status == "InProgress")
 .Include(t => _dbContext.Students.Where(s => s.Id == t.StudentId).FirstOrDefault())
            .OrderByDescending(t => t.Priority == "High" ? 3 : t.Priority == "Medium" ? 2 : 1)
        .ThenBy(t => t.CreatedDate)
       .Take(10)
   .ToListAsync();

            viewModel.PendingTasks_List = pendingTasks.Select(t =>
            {
   var student = _dbContext.Students.Find(t.StudentId);
        return new PendingTaskViewModel
 {
    Id = t.Id,
    TaskType = t.TaskType,
 Description = t.TaskDescription,
         Priority = t.Priority,
   CreatedDate = t.CreatedDate,
      StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown"
          };
     }).ToList();

      return View(viewModel);
  }
 }
}