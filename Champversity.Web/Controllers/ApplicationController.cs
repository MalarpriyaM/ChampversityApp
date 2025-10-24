using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Champversity.Web.Models;
using Champversity.DataAccess;
using Champversity.DataAccess.Models;

namespace Champversity.Web.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly ExcelService _excelService;
        private readonly ApplicationDbContext _dbContext;

 public ApplicationController(
    ILogger<ApplicationController> logger,
      ExcelService excelService,
       ApplicationDbContext dbContext)
        {
     _logger = logger;
  _excelService = excelService;
     _dbContext = dbContext;
  }

  public IActionResult Index()
      {
  return View();
        }

  [HttpGet]
  public IActionResult DownloadTemplate()
    {
            try
            {
 byte[] fileBytes = _excelService.GenerateExcelTemplate();
    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ChampversityApplication.xlsx");
      }
   catch (Exception ex)
      {
      _logger.LogError(ex, "Error generating Excel template");
 return RedirectToAction("Error", "Home");
     }
 }

 [HttpGet]
     public IActionResult UploadApplication()
      {
  return View();
  }
        
  [HttpPost]
      public async Task<IActionResult> UploadApplication(IFormFile excelFile)
        {
     if (excelFile == null || excelFile.Length == 0)
    {
  ModelState.AddModelError("", "Please select a file to upload");
        return View();
    }
  
    try
  {
 // Process the uploaded Excel file
      var student = await _excelService.ProcessUploadedExcel(excelFile);
 
     // Save student to database
     _dbContext.Students.Add(student);
      await _dbContext.SaveChangesAsync();
      
      // Return success view with application reference number
    return View("UploadSuccess", student.Id);
        }
     catch (Exception ex)
   {
    _logger.LogError(ex, "Error processing uploaded file");
    ModelState.AddModelError("", $"Error processing file: {ex.Message}");
        return View();
    }
  }
  
        [HttpGet]
     public async Task<IActionResult> CheckStatus(int? id)
 {
  if (!id.HasValue)
        {
   return View();
      }
   
 var student = await _dbContext.Students
    .Include(s => s.InterviewSlots)
        .FirstOrDefaultAsync(s => s.Id == id.Value);
      
   if (student == null)
 {
    ModelState.AddModelError("", "Application not found");
        return View();
      }
 
 return View("ApplicationStatus", student);
  }

[HttpPost]
        public async Task<IActionResult> SelectInterviewSlot(int studentId, int slotId)
    {
       try
     {
   // Find the student
  var student = await _dbContext.Students
     .Include(s => s.InterviewSlots)
   .FirstOrDefaultAsync(s => s.Id == studentId);
   
   if (student == null)
 {
    TempData["ErrorMessage"] = "Student not found.";
       return RedirectToAction("CheckStatus", new { id = studentId });
      }

     // Find the selected slot
      var selectedSlot = student.InterviewSlots.FirstOrDefault(s => s.Id == slotId);
       if (selectedSlot == null)
  {
  TempData["ErrorMessage"] = "Interview slot not found.";
  return RedirectToAction("CheckStatus", new { id = studentId });
            }

    // Unmark all other slots for this student
    foreach (var slot in student.InterviewSlots)
     {
    slot.IsSelected = false;
      }
   
// Mark the selected slot
      selectedSlot.IsSelected = true;
   student.SelectedSlot = selectedSlot;
student.ApplicationStatus = "Interview Slot Selected - Pending Confirmation";

    // Create a manual task for staff to confirm
   var confirmationTask = new ManualTask
      {
     StudentId = studentId,
        TaskType = "ConfirmInterviewSlot",
     TaskDescription = $"Confirm interview slot selection for {student.FirstName} {student.LastName} on {selectedSlot.SlotDateTime:MMMM dd, yyyy HH:mm}",
    Status = "Pending",
 Priority = "High",
    CreatedDate = DateTime.Now,
            AssignedTo = "Staff"
  };

     _dbContext.ManualTasks.Add(confirmationTask);
        await _dbContext.SaveChangesAsync();

  TempData["SuccessMessage"] = "Interview slot selected successfully. Our staff will contact you for confirmation.";
}
      catch (Exception ex)
        {
       _logger.LogError(ex, $"Error selecting interview slot for student {studentId}");
  TempData["ErrorMessage"] = "Error selecting interview slot. Please try again.";
        }

  return RedirectToAction("CheckStatus", new { id = studentId });
        }
    }
}