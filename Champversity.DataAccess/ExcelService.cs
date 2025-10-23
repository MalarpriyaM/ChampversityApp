using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Champversity.DataAccess.Models;

namespace Champversity.DataAccess
{
    public class ExcelService
    {
        private readonly FileStorage _fileStorage;
        private readonly ApplicationDbContext _dbContext;

        public ExcelService(FileStorage fileStorage, ApplicationDbContext dbContext)
 {
            _fileStorage = fileStorage;
            _dbContext = dbContext;
        }

        public byte[] GenerateExcelTemplate()
        {
            using (var workbook = new XSSFWorkbook())
{
      var sheet = workbook.CreateSheet("Student Information");
        
  // Create header row
    var headerRow = sheet.CreateRow(0);
  var headers = new[]
        {
               "Full Name", "Email", "Phone Number", "Date of Birth (MM/DD/YYYY)", 
        "Nationality", "Highest Qualification", "Preferred University", "Preferred Course"
    };
  
       for (int i = 0; i < headers.Length; i++)
                {
     var cell = headerRow.CreateCell(i);
     cell.SetCellValue(headers[i]);
          }
  
                // Create example row
    var exampleRow = sheet.CreateRow(1);
     exampleRow.CreateCell(0).SetCellValue("John Doe");
      exampleRow.CreateCell(1).SetCellValue("john.doe@example.com");
      exampleRow.CreateCell(2).SetCellValue("+1234567890");
    exampleRow.CreateCell(3).SetCellValue("01/15/1995");
                exampleRow.CreateCell(4).SetCellValue("United States");
      exampleRow.CreateCell(5).SetCellValue("Bachelor's Degree");
     exampleRow.CreateCell(6).SetCellValue("Oxford University");
     exampleRow.CreateCell(7).SetCellValue("Computer Science");
       
            // Auto-size columns
     for (int i = 0; i < headers.Length; i++)
        {
       sheet.AutoSizeColumn(i);
       }
             
    // Convert workbook to byte array
              using (var ms = new MemoryStream())
                {
         workbook.Write(ms);
       return ms.ToArray();
    }
        }
        }

        public async Task<Student> ProcessUploadedExcel(IFormFile file)
        {
            if (!_fileStorage.IsValidExcelFile(file))
         {
    throw new InvalidOperationException("Invalid file format. Please upload an Excel file (.xlsx or .xls).");
 }
            
   var fileName = await _fileStorage.SaveUploadedFileAsync(file);
            if (string.IsNullOrEmpty(fileName))
    {
    throw new InvalidOperationException("Failed to save the file.");
       }
            
          // Parse Excel and extract student data
            Student student = new Student();
          
using (var stream = file.OpenReadStream())
            {
          IWorkbook workbook = new XSSFWorkbook(stream);
      ISheet sheet = workbook.GetSheetAt(0);
          IRow dataRow = sheet.GetRow(1); // Assuming data is in row 1 (after header)
     
        if (dataRow != null)
  {
    student.FullName = dataRow.GetCell(0)?.StringCellValue;
           student.Email = dataRow.GetCell(1)?.StringCellValue;
 student.PhoneNumber = dataRow.GetCell(2)?.StringCellValue;
          
         var dobCell = dataRow.GetCell(3);
if (dobCell != null && dobCell.CellType == CellType.Numeric)
        {
     student.DateOfBirth = dobCell.DateCellValue;
 }
 
        student.Nationality = dataRow.GetCell(4)?.StringCellValue;
            student.HighestQualification = dataRow.GetCell(5)?.StringCellValue;
    student.PreferredUniversity = dataRow.GetCell(6)?.StringCellValue;
 student.PreferredCourse = dataRow.GetCell(7)?.StringCellValue;
        student.IsProfileComplete = true;
        student.ApplicationStatus = "Submitted";
     student.ApplicationSubmittedDate = DateTime.Now;
            }
  }
       
     return student;
        }
 
        public async Task<string> GenerateUniversityFile(Student student)
    {
    if (student == null || !student.IsProfileComplete)
   {
      return null;
            }
            
   string universityName = student.PreferredUniversity.Replace(" ", "");
            string filePath = _fileStorage.GetUniversityFilePath(universityName, DateTime.Today);
            
       bool fileExists = File.Exists(filePath);
            
     using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
   if (!fileExists)
     {
    // Write header if creating a new file
    writer.WriteLine("STUDENT_ID|FULL_NAME|EMAIL|PHONE|DOB|NATIONALITY|QUALIFICATION|COURSE");
   }
     
        // Write student data
             writer.WriteLine($"{student.Id}|{student.FullName}|{student.Email}|{student.PhoneNumber}|" +
           $"{student.DateOfBirth:MM/dd/yyyy}|{student.Nationality}|{student.HighestQualification}|{student.PreferredCourse}");
        }
 
         return filePath;
  }
        
  public async Task<string> GenerateXmlFile(Student student)
        {
  if (student == null)
       {
             return null;
   }
     
            string universityName = student.PreferredUniversity.Replace(" ", "");
  string xmlFilePath = _fileStorage.GetXmlFilePath(student.Id.ToString(), universityName);
            
         XDocument xmlDoc = new XDocument(
        new XElement("StudentApplication",
         new XElement("StudentID", student.Id),
      new XElement("FullName", student.FullName),
      new XElement("Email", student.Email),
    new XElement("PhoneNumber", student.PhoneNumber),
   new XElement("DateOfBirth", student.DateOfBirth.ToString("MM/dd/yyyy")),
new XElement("Nationality", student.Nationality),
          new XElement("HighestQualification", student.HighestQualification),
    new XElement("PreferredUniversity", student.PreferredUniversity),
 new XElement("PreferredCourse", student.PreferredCourse),
       new XElement("ApplicationDate", student.ApplicationSubmittedDate.ToString("MM/dd/yyyy")),
               new XElement("InterviewSlots")
      )
    );
            
    xmlDoc.Save(xmlFilePath);
            return xmlFilePath;
        }
        
      public async Task<Student> ProcessUniversityResponse(string xmlFilePath)
{
            if (!File.Exists(xmlFilePath))
    {
  throw new FileNotFoundException("University response file not found.", xmlFilePath);
            }
   
            XDocument xmlDoc = XDocument.Load(xmlFilePath);
            
            // Extract student ID
   int studentId = int.Parse(xmlDoc.Root.Element("StudentID").Value);
            var student = await _dbContext.Students.FindAsync(studentId);
     
          if (student == null)
    {
          throw new InvalidOperationException($"Student with ID {studentId} not found.");
    }
   
         // Extract interview slots
     var slotsElement = xmlDoc.Root.Element("InterviewSlots");
            if (slotsElement != null && slotsElement.HasElements)
         {
    student.InterviewSlots = new List<InterviewSlot>();
     
            foreach (var slotElement in slotsElement.Elements("Slot"))
                {
          DateTime slotDateTime = DateTime.Parse(slotElement.Value);
              
       student.InterviewSlots.Add(new InterviewSlot
    {
         SlotDateTime = slotDateTime,
    IsSelected = false,
    StudentId = student.Id
         });
      }
 
     student.ApplicationStatus = "Interview Slots Received";
            }
     
            return student;
 }
  }
}