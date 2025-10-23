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

namespace Champversity.DataAccess
{
    public class BatchProcessingService : BackgroundService
    {
private readonly ILogger<BatchProcessingService> _logger;
     private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _processingInterval = TimeSpan.FromHours(24); // Run once a day
        
     public BatchProcessingService(
      ILogger<BatchProcessingService> logger,
      IServiceScopeFactory scopeFactory)
       {
        _logger = logger;
        _scopeFactory = scopeFactory;
        }
     
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
     {
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
             
   // Get all students with completed profiles that haven't been processed yet
   var pendingStudents = await dbContext.Students
       .Where(s => s.IsProfileComplete && s.UniversityFileReference == null)
      .ToListAsync();
  
   _logger.LogInformation("Found {count} pending student applications to process", pendingStudents.Count);
       
foreach (var student in pendingStudents)
{
          // Generate university file
    string universityFilePath = await excelService.GenerateUniversityFile(student);
         student.UniversityFileReference = Path.GetFileName(universityFilePath);
  
   // Generate XML file for university
   string xmlFilePath = await excelService.GenerateXmlFile(student);
      student.ApplicationStatus = "Sent to University";
   }
   
await dbContext.SaveChangesAsync();
 _logger.LogInformation("Processed {count} student applications", pendingStudents.Count);
         }
 }
        
private async Task ProcessUniversityResponsesAsync()
  {
    // This would simulate checking for university responses
    // In a real system, this might check an API, SFTP, or email inbox
       
      // For this sample, we'll just log that we're checking
        _logger.LogInformation("Checking for university responses");
        }
    }
}