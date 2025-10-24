using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Champversity.DataAccess;
using Champversity.DataAccess.Models;

namespace Champversity.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("Admin/[controller]")]
  public class InterviewSlotsController : Controller
  {
     private readonly ApplicationDbContext _dbContext;

  public InterviewSlotsController(ApplicationDbContext dbContext)
        {
   _dbContext = dbContext;
        }

     [Route("")]
        [Route("Index")]
  public async Task<IActionResult> Index()
        {
            var slotsWithDetails = await _dbContext.InterviewSlots
      .Select(slot => new
             {
          Slot = slot,
    Student = _dbContext.Students.FirstOrDefault(s => s.Id == slot.StudentId)
     })
      .ToListAsync();

    ViewBag.SlotsWithDetails = slotsWithDetails;
            return View();
   }

        [Route("Pending")]
 public async Task<IActionResult> Pending()
        {
   var pendingSlots = await _dbContext.InterviewSlots
      .Where(slot => !slot.IsSelected)
         .Select(slot => new
      {
             Slot = slot,
        Student = _dbContext.Students.FirstOrDefault(s => s.Id == slot.StudentId)
  })
       .ToListAsync();

  ViewBag.PendingSlots = pendingSlots;
            return View();
        }

        [Route("Student/{studentId}")]
     public async Task<IActionResult> StudentSlots(int studentId)
        {
       var student = await _dbContext.Students.FindAsync(studentId);
            if (student == null)
       {
          return NotFound();
      }

            var slots = await _dbContext.InterviewSlots
     .Where(slot => slot.StudentId == studentId)
  .ToListAsync();

       ViewBag.Student = student;
            return View(slots);
        }

[Route("ConfirmSlot/{slotId}")]
        [HttpPost]
    public async Task<IActionResult> ConfirmSlot(int slotId, int studentId)
        {
       try
 {
            // Mark the selected slot
         var selectedSlot = await _dbContext.InterviewSlots.FindAsync(slotId);
         if (selectedSlot == null)
 {
       TempData["ErrorMessage"] = "Slot not found.";
        return RedirectToAction("StudentSlots", new { studentId });
      }

      // Unmark other slots for this student
    var otherSlots = await _dbContext.InterviewSlots
        .Where(s => s.StudentId == studentId && s.Id != slotId)
                 .ToListAsync();

           foreach (var slot in otherSlots)
         {
     slot.IsSelected = false;
   }

  selectedSlot.IsSelected = true;

    // Update student status
       var student = await _dbContext.Students.FindAsync(studentId);
              if (student != null)
        {
  student.ApplicationStatus = "Interview Confirmed";
   }

          await _dbContext.SaveChangesAsync();

        // Create a task to notify the student
  var notificationTask = new ManualTask
           {
     StudentId = studentId,
          TaskType = "NotifySlotConfirmation",
           TaskDescription = $"Notify student about confirmed interview slot on {selectedSlot.SlotDateTime:MMMM dd, yyyy HH:mm}",
        Status = "Pending",
            Priority = "High",
  CreatedDate = DateTime.Now,
       AssignedTo = "Staff"
    };

     _dbContext.ManualTasks.Add(notificationTask);
        await _dbContext.SaveChangesAsync();

     TempData["SuccessMessage"] = "Interview slot confirmed successfully.";
  }
   catch (Exception ex)
     {
         TempData["ErrorMessage"] = "Error confirming slot: " + ex.Message;
}

          return RedirectToAction("StudentSlots", new { studentId });
      }

        [Route("RequestAlternative/{studentId}")]
        [HttpPost]
 public async Task<IActionResult> RequestAlternative(int studentId, string reason)
  {
    try
 {
         // Create task to request alternative slots
                var task = new ManualTask
       {
     StudentId = studentId,
          TaskType = "RequestAlternativeSlots",
              TaskDescription = $"Contact university to request alternative interview slots. Reason: {reason}",
  Status = "Pending",
 Priority = "Medium",
              CreatedDate = DateTime.Now,
       AssignedTo = "Staff"
                };

    _dbContext.ManualTasks.Add(task);

      // Update student status
                var student = await _dbContext.Students.FindAsync(studentId);
      if (student != null)
                {
 student.ApplicationStatus = "Alternative Slots Requested";
                }

      await _dbContext.SaveChangesAsync();

  TempData["SuccessMessage"] = "Alternative slots request created successfully.";
            }
            catch (Exception ex)
 {
         TempData["ErrorMessage"] = "Error creating request: " + ex.Message;
         }

       return RedirectToAction("StudentSlots", new { studentId });
        }
    }
}