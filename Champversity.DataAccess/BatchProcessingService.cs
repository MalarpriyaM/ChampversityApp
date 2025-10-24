using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Champversity.DataAccess.Models;

namespace Champversity.DataAccess
{
    public class BatchProcessingService : BackgroundService
    {
     private readonly ILogger<BatchProcessingService> _logger;
      private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _processingInterval = TimeSpan.FromHours(6); // Run every 6 hours

        public BatchProcessingService(
      ILogger<BatchProcessingService> logger,
          IServiceScopeFactory scopeFactory)
     {
        _logger = logger;
            _scopeFactory = scopeFactory;
   }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
  // Wait a bit before starting the first run
  await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

  while (!stoppingToken.IsCancellationRequested)
        {
      try
         {
       _logger.LogInformation("Starting batch processing at {time}", DateTimeOffset.Now);

    await ProcessPendingApplicationsAsync();
       await ProcessUniversityResponsesAsync();

    _logger.LogInformation("Batch processing completed at {time}", DateTimeOffset.Now);
     }
  catch (Exception ex)
      {
     _logger.LogError(ex, "Error during batch processing");
    }

  // Wait until next execution time
      await Task.Delay(_processingInterval, stoppingToken);
     }
        }

        private async Task ProcessPendingApplicationsAsync()
        {
     using (var scope = _scopeFactory.CreateScope())
     {
     var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
       var excelService = scope.ServiceProvider.GetRequiredService<ExcelService>();
  var universityService = scope.ServiceProvider.GetRequiredService<IUniversityService>();
       var validationService = scope.ServiceProvider.GetRequiredService<ValidationService>();

         // Get all students with completed profiles that haven't been processed yet
     var pendingStudents = await dbContext.Students
      .Where(s => s.IsProfileComplete && 
(s.ApplicationStatus == "Submitted" || s.ApplicationStatus == "Validation Failed"))
   .ToListAsync();

  _logger.LogInformation("Found {count} pending student applications to process", pendingStudents.Count);

       foreach (var student in pendingStudents)
   {
      try
    {
            // First, validate the student data
     var validationResults = await validationService.ValidateStudentAsync(student);
  if (validationResults.Any())
         {
    student.ApplicationStatus = "Validation Failed";
              var errorMessages = string.Join("; ", validationResults.Select(v => v.ErrorMessage));
      _logger.LogWarning("Validation failed for student {studentId}: {errors}", student.Id, errorMessages);
            
         // Create manual task for fixing validation issues
       var validationTask = new ManualTask
        {
      StudentId = student.Id,
     TaskType = "ValidationFailed",
      TaskDescription = $"Fix validation issues for student {student.FirstName} {student.LastName}: {errorMessages}",
   Status = "Pending",
          Priority = "Medium",
    CreatedDate = DateTime.Now,
    AssignedTo = "Staff"
            };
       dbContext.ManualTasks.Add(validationTask);
    continue;
       }

  // Generate university file
       string universityFilePath = await excelService.GenerateUniversityFile(student);
   student.UniversityFileReference = Path.GetFileName(universityFilePath);

       // Generate XML file for university
       string xmlFilePath = await excelService.GenerateXmlFile(student);

     // Send to university (mock service)
       bool sentSuccessfully = await universityService.SendApplicationToUniversityAsync(student, xmlFilePath);

     if (sentSuccessfully)
           {
        student.ApplicationStatus = "Sent to University";
         _logger.LogInformation("Successfully processed application for student {studentId}", student.Id);
    }
        else
        {
   student.ApplicationStatus = "Send Failed";
     _logger.LogWarning("Failed to send application for student {studentId}", student.Id);

       // Create manual task for retry
 var retryTask = new ManualTask
  {
        StudentId = student.Id,
         TaskType = "RetryUniversitySubmission",
         TaskDescription = $"Retry sending application for {student.FirstName} {student.LastName} to {student.University}",
        Status = "Pending",
         Priority = "High",
            CreatedDate = DateTime.Now,
   AssignedTo = "Staff"
       };
       dbContext.ManualTasks.Add(retryTask);
   }
       }
         catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing application for student {studentId}", student.Id);
       student.ApplicationStatus = "Processing Error";
       }
        }

      await dbContext.SaveChangesAsync();
     _logger.LogInformation("Processed {count} student applications", pendingStudents.Count);
        }
   }

        private async Task ProcessUniversityResponsesAsync()
     {
        using (var scope = _scopeFactory.CreateScope())
     {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
   var universityService = scope.ServiceProvider.GetRequiredService<IUniversityService>();

try
       {
  // Check for new university responses
      var responses = await universityService.CheckForResponsesAsync();
     _logger.LogInformation("Found {count} university responses to process", responses.Count);

     foreach (var response in responses)
   {
     try
       {
 bool processed = await universityService.ProcessUniversityResponseAsync(response);
 if (processed)
     {
        _logger.LogInformation("Successfully processed university response for student {studentId}", response.StudentId);
        }
      else
        {
         _logger.LogWarning("Failed to process university response for student {studentId}", response.StudentId);
         }
    }
           catch (Exception ex)
        {
          _logger.LogError(ex, "Error processing university response for student {studentId}", response.StudentId);
            }
   }

     await dbContext.SaveChangesAsync();
 }
       catch (Exception ex)
  {
    _logger.LogError(ex, "Error checking for university responses");
       }
        }
     }
    }
}