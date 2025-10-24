using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Champversity.DataAccess;
using Champversity.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Champversity.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("Admin/[controller]")]
    public class UniversityResponsesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UniversityResponsesController(ApplicationDbContext dbContext)
  {
  _dbContext = dbContext;
   }

[Route("")]
        [Route("Index")]
     public async Task<IActionResult> Index()
        {
            var responses = await _dbContext.UniversityResponses
    .OrderByDescending(r => r.ReceivedDate)
     .ToListAsync();

            var responsesWithStudents = new List<object>();
            foreach (var response in responses)
            {
 var student = await _dbContext.Students.FindAsync(response.StudentId);
          responsesWithStudents.Add(new
          {
             Response = response,
   Student = student
       });
            }

            ViewBag.ResponsesWithStudents = responsesWithStudents;
         return View(responses);
        }

  [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
          var response = await _dbContext.UniversityResponses.FindAsync(id);
     if (response == null)
    {
     return NotFound();
            }

   var student = await _dbContext.Students.FindAsync(response.StudentId);
      ViewBag.Student = student;
         
       return View(response);
    }

        [Route("Pending")]
        public async Task<IActionResult> Pending()
        {
            var pendingResponses = await _dbContext.UniversityResponses
     .Where(r => !r.IsProcessed)
  .OrderBy(r => r.ReceivedDate)
          .ToListAsync();

  var responsesWithStudents = new List<object>();
       foreach (var response in pendingResponses)
 {
                var student = await _dbContext.Students.FindAsync(response.StudentId);
        responsesWithStudents.Add(new
     {
Response = response,
         Student = student
       });
     }

   ViewBag.ResponsesWithStudents = responsesWithStudents;
            return View(pendingResponses);
     }

        [Route("Processed")]
  public async Task<IActionResult> Processed()
        {
            var processedResponses = await _dbContext.UniversityResponses
   .Where(r => r.IsProcessed)
           .OrderByDescending(r => r.ReceivedDate)
            .ToListAsync();

  var responsesWithStudents = new List<object>();
   foreach (var response in processedResponses)
         {
 var student = await _dbContext.Students.FindAsync(response.StudentId);
    responsesWithStudents.Add(new
    {
        Response = response,
      Student = student
       });
            }

       ViewBag.ResponsesWithStudents = responsesWithStudents;
    return View(processedResponses);
  }

        [HttpPost]
 [Route("MarkAsProcessed/{id}")]
        public async Task<IActionResult> MarkAsProcessed(int id, string notes)
  {
            var response = await _dbContext.UniversityResponses.FindAsync(id);
            if (response == null)
            {
  return NotFound();
    }

       response.IsProcessed = true;
    response.ProcessingNotes = notes ?? "Manually marked as processed";
         
            await _dbContext.SaveChangesAsync();
       
 TempData["SuccessMessage"] = "University response marked as processed.";
          return RedirectToAction("Details", new { id });
      }

        [HttpPost]
        [Route("Reprocess/{id}")]
        public async Task<IActionResult> Reprocess(int id)
        {
      var response = await _dbContext.UniversityResponses.FindAsync(id);
      if (response == null)
       {
       return NotFound();
   }

    response.IsProcessed = false;
          response.ProcessingNotes = $"Marked for reprocessing on {DateTime.Now}";
     
    await _dbContext.SaveChangesAsync();
    
        TempData["SuccessMessage"] = "University response marked for reprocessing.";
      return RedirectToAction("Details", new { id });
        }
    }
}