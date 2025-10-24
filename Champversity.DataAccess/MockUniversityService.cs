using System.Xml.Linq;
using Champversity.DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace Champversity.DataAccess
{
    public interface IUniversityService
    {
 Task<bool> SendApplicationToUniversityAsync(Student student, string xmlFilePath);
     Task<List<UniversityResponse>> CheckForResponsesAsync();
   Task<bool> ProcessUniversityResponseAsync(UniversityResponse response);
 Task<List<InterviewSlot>> GenerateMockInterviewSlotsAsync(int studentId, string universityName);
    }

    public class MockUniversityService : IUniversityService
    {
        private readonly ApplicationDbContext _dbContext;
   private readonly ILogger<MockUniversityService> _logger;
     private readonly Random _random = new Random();

  public MockUniversityService(ApplicationDbContext dbContext, ILogger<MockUniversityService> logger)
        {
     _dbContext = dbContext;
 _logger = logger;
     }

        public async Task<bool> SendApplicationToUniversityAsync(Student student, string xmlFilePath)
    {
   try
      {
     _logger.LogInformation($"Sending application for student {student.Id} to {student.University}");
        
    // Simulate network delay
        await Task.Delay(1000);
        
             // Mock success rate (90% success)
         var isSuccess = _random.NextDouble() > 0.1;
      
      if (isSuccess)
   {
        _logger.LogInformation($"Successfully sent application for student {student.Id} to {student.University}");
         
        // Schedule a mock response (simulate university processing time - 1-3 days)
   var responseDate = DateTime.Now.AddDays(_random.Next(1, 4));
 
         var mockResponse = new UniversityResponse
      {
       StudentId = student.Id,
   UniversityName = student.University,
       ResponseType = "InterviewSlots",
     ResponseData = await GenerateMockResponseDataAsync(student),
       ReceivedDate = responseDate,
        IsProcessed = false
  };

  _dbContext.UniversityResponses.Add(mockResponse);
         await _dbContext.SaveChangesAsync();
   }
      else
    {
   _logger.LogWarning($"Failed to send application for student {student.Id} to {student.University}");
            }

 return isSuccess;
        }
 catch (Exception ex)
   {
             _logger.LogError(ex, $"Error sending application for student {student.Id}");
       return false;
        }
        }

 public async Task<List<UniversityResponse>> CheckForResponsesAsync()
        {
      try
 {
         // Get unprocessed responses that are due
         var dueResponses = await Task.FromResult(
       _dbContext.UniversityResponses
.Where(r => !r.IsProcessed && r.ReceivedDate <= DateTime.Now)
   .ToList()
   );

     _logger.LogInformation($"Found {dueResponses.Count} university responses ready for processing");
  return dueResponses;
      }
   catch (Exception ex)
     {
      _logger.LogError(ex, "Error checking for university responses");
 return new List<UniversityResponse>();
      }
   }

        public async Task<bool> ProcessUniversityResponseAsync(UniversityResponse response)
 {
  try
    {
 _logger.LogInformation($"Processing university response for student {response.StudentId}");
    
  var student = await _dbContext.Students.FindAsync(response.StudentId);
            if (student == null)
    {
       _logger.LogWarning($"Student {response.StudentId} not found");
    return false;
    }

    // Parse the response data and create interview slots
      var interviewSlots = await ParseInterviewSlotsFromResponseAsync(response);
       
    if (interviewSlots.Any())
     {
         _dbContext.InterviewSlots.AddRange(interviewSlots);
     student.ApplicationStatus = "Interview Slots Received";
  
    // Create manual task for staff to contact student
     var manualTask = new ManualTask
  {
            StudentId = student.Id,
        TaskType = "CallForSlotConfirmation",
     TaskDescription = $"Call {student.FirstName} {student.LastName} to confirm interview slot selection for {response.UniversityName}",
     AssignedTo = "Staff", // Can be enhanced to assign to specific staff
        Status = "Pending",
     CreatedDate = DateTime.Now,
       Priority = "High"
    };

         _dbContext.ManualTasks.Add(manualTask);
    }

     response.IsProcessed = true;
        response.ProcessingNotes = $"Processed on {DateTime.Now}. Generated {interviewSlots.Count} interview slots.";

     await _dbContext.SaveChangesAsync();
      
     _logger.LogInformation($"Successfully processed response for student {response.StudentId}");
         return true;
        }
     catch (Exception ex)
     {
       _logger.LogError(ex, $"Error processing university response for student {response.StudentId}");
       return false;
        }
        }

public async Task<List<InterviewSlot>> GenerateMockInterviewSlotsAsync(int studentId, string universityName)
        {
     var slots = new List<InterviewSlot>();
       var baseDate = DateTime.Now.AddDays(_random.Next(7, 21)); // 1-3 weeks from now

  for (int i = 0; i < 3; i++) // Generate 3 slots as per requirement
       {
   var slotDate = baseDate.AddDays(i * _random.Next(1, 3));
    var slotTime = new TimeSpan(_random.Next(9, 17), _random.Next(0, 4) * 15, 0); // 9 AM to 5 PM, 15-minute intervals

   slots.Add(new InterviewSlot
      {
  StudentId = studentId,
       SlotDateTime = slotDate.Date.Add(slotTime),
       IsSelected = false
 });
    }

        return slots;
  }

 private async Task<string> GenerateMockResponseDataAsync(Student student)
        {
         // Generate mock XML response with interview slots
     var slots = await GenerateMockInterviewSlotsAsync(student.Id, student.University);
     
            var xmlResponse = new XElement("UniversityResponse",
   new XElement("StudentID", student.Id),
        new XElement("UniversityName", student.University),
       new XElement("ResponseDate", DateTime.Now.ToString("yyyy-MM-dd")),
    new XElement("Status", "Accepted"),
       new XElement("InterviewSlots",
  slots.Select(s => new XElement("Slot", 
        new XElement("DateTime", s.SlotDateTime.ToString("yyyy-MM-dd HH:mm")),
    new XElement("Duration", "30 minutes"),
          new XElement("Mode", "Online") // Can be Online, In-Person, Phone
  ))
   )
  );

      return xmlResponse.ToString();
     }

private async Task<List<InterviewSlot>> ParseInterviewSlotsFromResponseAsync(UniversityResponse response)
  {
       var slots = new List<InterviewSlot>();
       
     try
    {
     var xmlDoc = XDocument.Parse(response.ResponseData);
  var slotElements = xmlDoc.Descendants("Slot");

     foreach (var slotElement in slotElements)
         {
   var dateTimeStr = slotElement.Element("DateTime")?.Value;
   if (DateTime.TryParse(dateTimeStr, out var slotDateTime))
    {
   slots.Add(new InterviewSlot
       {
      StudentId = response.StudentId,
SlotDateTime = slotDateTime,
      IsSelected = false
  });
      }
 }
  }
  catch (Exception ex)
       {
   _logger.LogError(ex, $"Error parsing interview slots from university response");
    }

      return slots;
    }
 }
}