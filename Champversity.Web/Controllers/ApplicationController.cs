using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
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
        public IActionResult CheckStatus(int? id)
        {
            if (!id.HasValue)
      {
          return View();
            }
   
       var student = _dbContext.Students.Find(id.Value);
        if (student == null)
      {
      ModelState.AddModelError("", "Application not found");
       return View();
   }
 
     return View("ApplicationStatus", student);
        }
    }
}