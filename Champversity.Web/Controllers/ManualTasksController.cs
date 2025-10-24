using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Champversity.DataAccess;
using Champversity.DataAccess.Models;

namespace Champversity.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("Admin/[controller]")]
    public class ManualTasksController : Controller
    {
        private readonly IManualTaskService _manualTaskService;
        private readonly ApplicationDbContext _dbContext;

        public ManualTasksController(IManualTaskService manualTaskService, ApplicationDbContext dbContext)
        {
            _manualTaskService = manualTaskService;
     _dbContext = dbContext;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
      var tasks = await _manualTaskService.GetPendingTasksAsync();
        var tasksWithStudents = new List<object>();

      foreach (var task in tasks)
     {
  var student = await _dbContext.Students.FindAsync(task.StudentId);
    tasksWithStudents.Add(new
     {
      Task = task,
        Student = student
        });
        }

        ViewBag.TasksWithStudents = tasksWithStudents;
     return View(tasks);
    }

   [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
  var task = await _manualTaskService.GetTaskByIdAsync(id);
  if (task == null)
        {
 return NotFound();
        }

       var student = await _dbContext.Students.FindAsync(task.StudentId);
   ViewBag.Student = student;
   return View(task);
    }

        [Route("UpdateStatus/{id}")]
    [HttpPost]
   public async Task<IActionResult> UpdateStatus(int id, string status, string notes)
        {
            var success = await _manualTaskService.UpdateTaskStatusAsync(id, status, notes);
  if (success)
        {
 TempData["SuccessMessage"] = "Task status updated successfully.";
  }
            else
  {
         TempData["ErrorMessage"] = "Failed to update task status.";
        }
        return RedirectToAction("Details", new { id });
   }

[Route("Assign/{id}")]
        [HttpPost]
public async Task<IActionResult> Assign(int id, string assignee)
      {
     var success = await _manualTaskService.AssignTaskAsync(id, assignee);
         if (success)
  {
       TempData["SuccessMessage"] = "Task assigned successfully.";
       }
    else
        {
    TempData["ErrorMessage"] = "Failed to assign task.";
    }
            return RedirectToAction("Details", new { id });
        }

        [Route("Complete/{id}")]
        [HttpPost]
        public async Task<IActionResult> Complete(int id, string completionNotes)
      {
     var success = await _manualTaskService.CompleteTaskAsync(id, completionNotes);
   if (success)
        {
   TempData["SuccessMessage"] = "Task completed successfully.";
        }
        else
 {
    TempData["ErrorMessage"] = "Failed to complete task.";
        }
        return RedirectToAction("Index");
     }

        [Route("MyTasks")]
 public async Task<IActionResult> MyTasks()
    {
      var currentUser = User.Identity.Name;
       var tasks = await _manualTaskService.GetTasksByAssigneeAsync(currentUser);
            var tasksWithStudents = new List<object>();

    foreach (var task in tasks)
        {
       var student = await _dbContext.Students.FindAsync(task.StudentId);
     tasksWithStudents.Add(new
            {
    Task = task,
         Student = student
        });
  }

       ViewBag.TasksWithStudents = tasksWithStudents;
       return View(tasks);
        }
    }
}