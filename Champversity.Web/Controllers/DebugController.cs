using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Champversity.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Champversity.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("Admin/[controller]")]
    public class DebugController : Controller
    {
  private readonly ApplicationDbContext _dbContext;

        public DebugController(ApplicationDbContext dbContext)
     {
            _dbContext = dbContext;
        }

   [Route("StudentStatus/{studentId}")]
        public async Task<IActionResult> StudentStatus(int studentId)
        {
            var student = await _dbContext.Students
        .Include(s => s.InterviewSlots)
       .Include(s => s.Certifications)
    .Include(s => s.Achievements)
        .Include(s => s.Volunteers)
          .FirstOrDefaultAsync(s => s.Id == studentId);

  if (student == null)
   {
           return NotFound($"Student {studentId} not found");
    }

         var universityResponses = await _dbContext.UniversityResponses
                .Where(r => r.StudentId == studentId)
        .ToListAsync();

            var manualTasks = await _dbContext.ManualTasks
        .Where(t => t.StudentId == studentId)
         .ToListAsync();

    ViewBag.UniversityResponses = universityResponses;
      ViewBag.ManualTasks = manualTasks;

     return View(student);
   }

        [Route("SystemOverview")]
      public async Task<IActionResult> SystemOverview()
        {
var data = new
{
      Students = await _dbContext.Students.ToListAsync(),
       UniversityResponses = await _dbContext.UniversityResponses.ToListAsync(),
     InterviewSlots = await _dbContext.InterviewSlots.ToListAsync(),
                ManualTasks = await _dbContext.ManualTasks.ToListAsync()
            };

          return View(data);
        }

        [Route("CheckSlots")]
    public async Task<IActionResult> CheckSlots()
        {
            var allSlots = await _dbContext.InterviewSlots
      .Include(s => s.StudentId)
            .ToListAsync();

   var slotsWithStudents = new List<object>();
            foreach (var slot in allSlots)
      {
         var student = await _dbContext.Students.FindAsync(slot.StudentId);
  slotsWithStudents.Add(new
   {
 Slot = slot,
        Student = student
                });
            }

          ViewBag.SlotsWithStudents = slotsWithStudents;
   return View(allSlots);
        }

        [Route("FixSlots/{studentId}")]
      [HttpPost]
        public async Task<IActionResult> FixSlots(int studentId)
        {
            var student = await _dbContext.Students.FindAsync(studentId);
  if (student == null)
            {
        return NotFound($"Student {studentId} not found");
      }

            // Create interview slots manually for testing
      var existingSlots = await _dbContext.InterviewSlots
        .Where(s => s.StudentId == studentId)
           .ToListAsync();

     if (!existingSlots.Any())
   {
                var baseDate = DateTime.Now.AddDays(7);
  var newSlots = new[]
       {
        new Champversity.DataAccess.Models.InterviewSlot
       {
            StudentId = studentId,
             SlotDateTime = baseDate.AddHours(10), // 10 AM next week
   IsSelected = false
  },
      new Champversity.DataAccess.Models.InterviewSlot
         {
           StudentId = studentId,
   SlotDateTime = baseDate.AddDays(1).AddHours(14), // 2 PM next day
               IsSelected = false
   },
  new Champversity.DataAccess.Models.InterviewSlot
        {
           StudentId = studentId,
     SlotDateTime = baseDate.AddDays(2).AddHours(16), // 4 PM day after
   IsSelected = false
  }
        };

       _dbContext.InterviewSlots.AddRange(newSlots);
     await _dbContext.SaveChangesAsync();

// Update student status
           student.ApplicationStatus = "Interview Slots Received";
        await _dbContext.SaveChangesAsync();

      TempData["SuccessMessage"] = $"Created {newSlots.Length} interview slots for student {studentId}";
   }
            else
         {
    TempData["InfoMessage"] = $"Student {studentId} already has {existingSlots.Count} interview slots";
       }

 return RedirectToAction("StudentStatus", new { studentId });
 }
    }
}