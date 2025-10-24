using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Champversity.DataAccess;
using Champversity.DataAccess.Models;

namespace Champversity.Web.Controllers
{
    public class InterviewSlotViewModel
    {
        public InterviewSlot Slot { get; set; } = new();
        public Student? Student { get; set; }
    }

    public class InterviewSlotsIndexViewModel
    {
public List<InterviewSlotViewModel> SlotsWithDetails { get; set; } = new();
    }

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
            // Get all slots and students
     var allSlots = await _dbContext.InterviewSlots
        .OrderBy(s => s.SlotDateTime)
     .ToListAsync();
            
         var allStudents = await _dbContext.Students.ToListAsync();
            
// Create lookup dictionary for faster student lookup
            var studentLookup = allStudents.ToDictionary(s => s.Id);
            
  // Create strongly typed view models
      var slotsWithDetails = allSlots.Select(slot => new InterviewSlotViewModel
         {
          Slot = slot,
  Student = studentLookup.TryGetValue(slot.StudentId, out var student) ? student : null
         }).ToList();

         // Create the main view model
  var viewModel = new InterviewSlotsIndexViewModel
       {
     SlotsWithDetails = slotsWithDetails
   };

    return View(viewModel);
        }

        [Route("Pending")]
     public async Task<IActionResult> Pending()
        {
            // Get pending slots
            var pendingSlots = await _dbContext.InterviewSlots
    .Where(slot => !slot.IsSelected)
      .OrderBy(slot => slot.SlotDateTime)
            .ToListAsync();
            
var allStudents = await _dbContext.Students.ToListAsync();
            var studentLookup = allStudents.ToDictionary(s => s.Id);
          
      var pendingSlotsWithDetails = pendingSlots.Select(slot => new
     {
       Slot = slot,
     Student = studentLookup.TryGetValue(slot.StudentId, out var student) ? student : null
            }).ToList();

   ViewBag.PendingSlots = pendingSlotsWithDetails;
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
    .OrderBy(slot => slot.SlotDateTime)
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