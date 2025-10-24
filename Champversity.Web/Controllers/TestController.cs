using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Champversity.DataAccess;
using Champversity.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Champversity.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("Admin/[controller]")]
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUniversityService _universityService;

        public TestController(ApplicationDbContext dbContext, IUniversityService universityService)
        {
   _dbContext = dbContext;
    _universityService = universityService;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("TriggerMockResponse/{studentId}")]
  public async Task<IActionResult> TriggerMockUniversityResponse(int studentId)
        {
       try
       {
         var student = await _dbContext.Students.FindAsync(studentId);
     if (student == null)
       {
        return NotFound($"Student {studentId} not found");
          }

       // Create immediate mock response for testing
                var mockResponse = new UniversityResponse
      {
           StudentId = studentId,
 UniversityName = student.University,
         ResponseType = "InterviewSlots",
   ResponseData = await GenerateTestResponseData(student),
      ReceivedDate = DateTime.Now, // Immediate response for testing
 IsProcessed = false
           };

      _dbContext.UniversityResponses.Add(mockResponse);
   await _dbContext.SaveChangesAsync();

                // Process the response immediately
var processed = await _universityService.ProcessUniversityResponseAsync(mockResponse);

          if (processed)
      {
       TempData["SuccessMessage"] = $"Mock university response triggered for student {studentId}. Interview slots should now be available.";
      }
                else
        {
      TempData["ErrorMessage"] = "Failed to process mock university response.";
    }

                return RedirectToAction("Index", "Dashboard");
 }
            catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
       return RedirectToAction("Index", "Dashboard");
   }
      }

        [HttpPost]
        [Route("CreateSampleData/{studentId}")]
        public async Task<IActionResult> CreateSampleData(int studentId)
 {
            try
    {
    await CreateCompleteSampleDataAsync(studentId);
        TempData["SuccessMessage"] = $"Complete sample data created for student {studentId} and additional test students.";
    return RedirectToAction("Index", "Dashboard");
     }
            catch (Exception ex)
        {
     TempData["ErrorMessage"] = $"Error creating sample data: {ex.Message}";
   return RedirectToAction("Index", "Dashboard");
      }
        }

        private async Task CreateCompleteSampleDataAsync(int studentId)
   {
   // Create additional sample students for testing different scenarios
   var sampleStudents = new[]
            {
             new Student
                {
                    FirstName = "Alice",
         LastName = "Johnson",
       Email = "alice.johnson@example.com",
    PhoneNumber = "+1234567891",
 University = "Harvard University",
     UniversityCountry = "USA",
        UniversityCity = "Cambridge",
      IntendedProgramMajor = "Business Administration",
             ApplicationStatus = "Sent to University",
         IsProfileComplete = true,
         ApplicationSubmittedDate = DateTime.Now.AddDays(-2),
      DateOfBirth = new DateTime(1995, 5, 15),
          Gender = "Female",
            Nationality = "Canadian"
      },
    new Student
                {
        FirstName = "Bob",
              LastName = "Smith",
Email = "bob.smith@example.com",
        PhoneNumber = "+1234567892",
      University = "MIT",
                    UniversityCountry = "USA",
 UniversityCity = "Boston",
        IntendedProgramMajor = "Computer Science",
    ApplicationStatus = "Interview Slots Received",
         IsProfileComplete = true,
     ApplicationSubmittedDate = DateTime.Now.AddDays(-5),
     DateOfBirth = new DateTime(1994, 8, 22),
 Gender = "Male",
                    Nationality = "American"
     },
                new Student
        {
      FirstName = "Carol",
           LastName = "Davis",
             Email = "carol.davis@example.com",
                 PhoneNumber = "+1234567893",
      University = "Stanford University",
         UniversityCountry = "USA",
        UniversityCity = "Stanford",
             IntendedProgramMajor = "Engineering",
               ApplicationStatus = "Interview Completed",
   IsProfileComplete = true,
   ApplicationSubmittedDate = DateTime.Now.AddDays(-10),
       DateOfBirth = new DateTime(1996, 12, 3),
    Gender = "Female",
        Nationality = "British"
       }
            };

       foreach (var student in sampleStudents)
    {
  var existingStudent = await _dbContext.Students
        .FirstOrDefaultAsync(s => s.Email == student.Email);
  
         if (existingStudent == null)
      {
                    _dbContext.Students.Add(student);
         await _dbContext.SaveChangesAsync();
           
         // Create sample data based on status
   await CreateStatusSpecificDataAsync(student);
             }
            }

         // Create sample validation rules
            await CreateSampleValidationRulesAsync();

        // Create sample manual tasks
          await CreateSampleManualTasksAsync(studentId);

            await _dbContext.SaveChangesAsync();
        }

    private async Task CreateStatusSpecificDataAsync(Student student)
        {
       switch (student.ApplicationStatus)
{
case "Interview Slots Received":
// Create interview slots
      var slots = new[]
   {
      new InterviewSlot
       {
       StudentId = student.Id,
             SlotDateTime = DateTime.Now.AddDays(3).AddHours(10),
      IsSelected = false
     },
      new InterviewSlot
         {
   StudentId = student.Id,
      SlotDateTime = DateTime.Now.AddDays(4).AddHours(14),
         IsSelected = false
     },
           new InterviewSlot
         {
  StudentId = student.Id,
          SlotDateTime = DateTime.Now.AddDays(5).AddHours(16),
       IsSelected = true
     }
              };
        _dbContext.InterviewSlots.AddRange(slots);
 
         // Create manual task for slot confirmation
      var slotTask = new ManualTask
  {
               StudentId = student.Id,
 TaskType = "CallForSlotConfirmation",
    TaskDescription = $"Call {student.FirstName} {student.LastName} to confirm interview slot selection",
       AssignedTo = "Staff",
     Status = "Pending",
       CreatedDate = DateTime.Now,
      Priority = "High"
        };
          _dbContext.ManualTasks.Add(slotTask);
 break;

case "Interview Completed":
  // Create completed interview slots
                var completedSlot = new InterviewSlot
         {
     StudentId = student.Id,
        SlotDateTime = DateTime.Now.AddDays(-2),
   IsSelected = true
};
          _dbContext.InterviewSlots.Add(completedSlot);
         
  // Create completed manual task
         var completedTask = new ManualTask
    {
     StudentId = student.Id,
     TaskType = "InterviewFollowUp",
         TaskDescription = $"Follow up on interview results for {student.FirstName} {student.LastName}",
         AssignedTo = "Staff",
     Status = "Completed",
            CreatedDate = DateTime.Now.AddDays(-3),
       CompletedDate = DateTime.Now.AddDays(-1),
  Priority = "Medium"
           };
          _dbContext.ManualTasks.Add(completedTask);
       break;
      }
        }

        private async Task CreateSampleValidationRulesAsync()
        {
            var rules = new[]
   {
                new ValidationRule
           {
             FieldName = "FirstName",
     RuleType = "Required",
            RuleValue = "true",
     ErrorMessage = "First name is required",
             IsActive = true
         },
      new ValidationRule
         {
     FieldName = "Email",
       RuleType = "Email",
RuleValue = "true",
     ErrorMessage = "Please provide a valid email address",
                    IsActive = true
      },
           new ValidationRule
             {
 FieldName = "IELTSScore",
    RuleType = "Range",
        RuleValue = "0-9",
     ErrorMessage = "IELTS score must be between 0 and 9",
                 IsActive = true
           }
    };

    foreach (var rule in rules)
            {
            var existing = await _dbContext.ValidationRules
         .FirstOrDefaultAsync(r => r.FieldName == rule.FieldName && r.RuleType == rule.RuleType);
     
        if (existing == null)
          {
      _dbContext.ValidationRules.Add(rule);
         }
      }
        }

     private async Task CreateSampleManualTasksAsync(int studentId)
        {
            var tasks = new[]
       {
           new ManualTask
      {
         StudentId = studentId,
       TaskType = "DocumentVerification",
       TaskDescription = $"Verify uploaded documents for student ID {studentId}",
              AssignedTo = "Staff",
         Status = "Pending",
          CreatedDate = DateTime.Now,
       Priority = "Medium"
                },
new ManualTask
  {
    StudentId = studentId,
              TaskType = "PaymentConfirmation",
               TaskDescription = $"Confirm payment received for student ID {studentId}",
         AssignedTo = "Admin",
       Status = "InProgress",
   CreatedDate = DateTime.Now.AddHours(-2),
    Priority = "High"
      }
            };

            _dbContext.ManualTasks.AddRange(tasks);
        }

        private async Task<string> GenerateTestResponseData(Student student)
      {
      // Generate immediate test slots
  var baseDate = DateTime.Now.AddDays(7); // Next week
            var slots = new[]
       {
 baseDate.AddHours(10), // 10 AM
     baseDate.AddDays(1).AddHours(14), // 2 PM next day
           baseDate.AddDays(2).AddHours(16)  // 4 PM day after
    };

            var xmlResponse = $@"
<UniversityResponse>
    <StudentID>{student.Id}</StudentID>
    <UniversityName>{student.University}</UniversityName>
    <ResponseDate>{DateTime.Now:yyyy-MM-dd}</ResponseDate>
    <Status>Accepted</Status>
    <InterviewSlots>
        <Slot>
   <DateTime>{slots[0]:yyyy-MM-dd HH:mm}</DateTime>
            <Duration>30 minutes</Duration>
      <Mode>Online</Mode>
        </Slot>
        <Slot>
      <DateTime>{slots[1]:yyyy-MM-dd HH:mm}</DateTime>
   <Duration>30 minutes</Duration>
 <Mode>Online</Mode>
        </Slot>
        <Slot>
    <DateTime>{slots[2]:yyyy-MM-dd HH:mm}</DateTime>
     <Duration>30 minutes</Duration>
      <Mode>In-Person</Mode>
        </Slot>
    </InterviewSlots>
</UniversityResponse>";

            return xmlResponse;
        }
 }
}