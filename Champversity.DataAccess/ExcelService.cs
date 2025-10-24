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
 var sheet = workbook.CreateSheet("Applicant Information");
 var headerRow = sheet.CreateRow(0);
 var headers = new[]
 {
 // Personal Information
 "First Name", "Middle Name", "Last Name", "Date of Birth", "Gender", "Nationality", "Passport Number", "Phone Number", "Email ID", "Permanent Address", "Current Address",
 // Academic Background
 "School Academic Board", "School Year of Pass", "School Grade", "Highest Qualification", "Highest Qualification Year of Pass", "Highest Qualification Grades", "IELTS Score",
 // Program Details
 "University", "University Country", "University City", "Intended Program/Major", "Preferred Start Semester", "Mode of Study",
 // Certifications (Optional)
 "Certification1", "Certification1 Year", "Certification1 Score", "Certification2", "Certification2 Year", "Certification2 Score", "Certification3", "Certification3 Year", "Certification3 Score",
 // Work Experience (Optional)
 "Total Work Experience", "Last Role", "Domain", "Last Drawn Annual Salary",
 // Statement of Purpose
 "Motivation of Current Program", "Relevant Program Knowledge",
 // Awards and Achievements (Optional)
 "Achievement1", "Achievement1 Year", "Achievement1 Role", "Achievement2", "Achievement2 Year", "Achievement2 Role", "Achievement3", "Achievement3 Year", "Achievement3 Role",
 // Volunteer (Optional)
 "Volunteer1 Program", "Volunteer1 Year", "Volunteer1 Contribution Role", "Volunteer2 Program", "Volunteer2 Year", "Volunteer2 Contribution Role", "Volunteer3 Program", "Volunteer3 Year", "Volunteer3 Contribution Role",
 // Financial Information
 "Funding Source", "Scholarship Expectations",
 // Visa
 "Visa Status",
 // Application Fee Payment
 "Payment Method"
 };
 for (int i =0; i < headers.Length; i++)
 {
 var cell = headerRow.CreateCell(i);
 cell.SetCellValue(headers[i]);
 }
 // Example row
 var exampleRow = sheet.CreateRow(1);
 exampleRow.CreateCell(0).SetCellValue("John");
 exampleRow.CreateCell(1).SetCellValue("");
 exampleRow.CreateCell(2).SetCellValue("Doe");
 exampleRow.CreateCell(3).SetCellValue("01/15/1995");
 exampleRow.CreateCell(4).SetCellValue("Male");
 exampleRow.CreateCell(5).SetCellValue("Indian");
 exampleRow.CreateCell(6).SetCellValue("A1234567");
 exampleRow.CreateCell(7).SetCellValue("+1234567890");
 exampleRow.CreateCell(8).SetCellValue("john.doe@example.com");
 exampleRow.CreateCell(9).SetCellValue("123 Main St");
 exampleRow.CreateCell(10).SetCellValue("456 College Ave");
 exampleRow.CreateCell(11).SetCellValue("CBSE");
 exampleRow.CreateCell(12).SetCellValue("2012");
 exampleRow.CreateCell(13).SetCellValue("A+");
 exampleRow.CreateCell(14).SetCellValue("Bachelor's Degree");
 exampleRow.CreateCell(15).SetCellValue("2016");
 exampleRow.CreateCell(16).SetCellValue("A");
 exampleRow.CreateCell(17).SetCellValue("7.5");
 exampleRow.CreateCell(18).SetCellValue("Oxford University");
 exampleRow.CreateCell(19).SetCellValue("UK");
 exampleRow.CreateCell(20).SetCellValue("Oxford");
 exampleRow.CreateCell(21).SetCellValue("Computer Science");
 exampleRow.CreateCell(22).SetCellValue("H1");
 exampleRow.CreateCell(23).SetCellValue("Full-time");
 // Certifications
 exampleRow.CreateCell(24).SetCellValue("Java");
 exampleRow.CreateCell(25).SetCellValue("2015");
 exampleRow.CreateCell(26).SetCellValue("90");
 exampleRow.CreateCell(27).SetCellValue("");
 exampleRow.CreateCell(28).SetCellValue("");
 exampleRow.CreateCell(29).SetCellValue("");
 exampleRow.CreateCell(30).SetCellValue("");
 exampleRow.CreateCell(31).SetCellValue("");
 exampleRow.CreateCell(32).SetCellValue("");
 // Work Experience
 exampleRow.CreateCell(33).SetCellValue("2 years");
 exampleRow.CreateCell(34).SetCellValue("Developer");
 exampleRow.CreateCell(35).SetCellValue("IT");
 exampleRow.CreateCell(36).SetCellValue("50000");
 // Statement of Purpose
 exampleRow.CreateCell(37).SetCellValue("To pursue advanced studies");
 exampleRow.CreateCell(38).SetCellValue("Expert");
 // Achievements
 exampleRow.CreateCell(39).SetCellValue("Math Olympiad");
 exampleRow.CreateCell(40).SetCellValue("2011");
 exampleRow.CreateCell(41).SetCellValue("Winner");
 exampleRow.CreateCell(42).SetCellValue("");
 exampleRow.CreateCell(43).SetCellValue("");
 exampleRow.CreateCell(44).SetCellValue("");
 exampleRow.CreateCell(45).SetCellValue("");
 exampleRow.CreateCell(46).SetCellValue("");
 exampleRow.CreateCell(47).SetCellValue("");
 // Volunteer
 exampleRow.CreateCell(48).SetCellValue("Red Cross");
 exampleRow.CreateCell(49).SetCellValue("2013");
 exampleRow.CreateCell(50).SetCellValue("Helper");
 exampleRow.CreateCell(51).SetCellValue("");
 exampleRow.CreateCell(52).SetCellValue("");
 exampleRow.CreateCell(53).SetCellValue("");
 exampleRow.CreateCell(54).SetCellValue("");
 exampleRow.CreateCell(55).SetCellValue("");
 exampleRow.CreateCell(56).SetCellValue("");
 // Financial
 exampleRow.CreateCell(57).SetCellValue("Self-funded");
 exampleRow.CreateCell(58).SetCellValue("No");
 // Visa
 exampleRow.CreateCell(59).SetCellValue("Applied");
 // Payment
 exampleRow.CreateCell(60).SetCellValue("Credit Card");
 // Auto-size columns
 for (int i =0; i < headers.Length; i++)
 {
 sheet.AutoSizeColumn(i);
 }
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
 
 Student student = new Student();
 using (var stream = file.OpenReadStream())
 {
 IWorkbook workbook = new XSSFWorkbook(stream);
 ISheet sheet = workbook.GetSheetAt(0);
 IRow dataRow = sheet.GetRow(1); // Data row
 if (dataRow != null)
 {
 // Personal Information
 student.FirstName = GetCellValueSafely(dataRow, 0);
 student.MiddleName = GetCellValueSafely(dataRow, 1);
 student.LastName = GetCellValueSafely(dataRow, 2);
 student.DateOfBirth = ParseDateSafely(GetCellValueSafely(dataRow, 3));
 student.Gender = GetCellValueSafely(dataRow, 4);
 student.Nationality = GetCellValueSafely(dataRow, 5);
 student.PassportNumber = GetCellValueSafely(dataRow, 6);
 student.PhoneNumber = GetCellValueSafely(dataRow, 7);
 student.Email = GetCellValueSafely(dataRow, 8);
 student.PermanentAddress = GetCellValueSafely(dataRow, 9);
 student.CurrentAddress = GetCellValueSafely(dataRow, 10);
 
 // Academic Background
 student.SchoolAcademicBoard = GetCellValueSafely(dataRow, 11);
 student.SchoolYearOfPass = GetCellValueSafely(dataRow, 12);
 student.SchoolGrade = GetCellValueSafely(dataRow, 13);
 student.HighestQualification = GetCellValueSafely(dataRow, 14);
 student.HighestQualificationYearOfPass = GetCellValueSafely(dataRow, 15);
 student.HighestQualificationGrades = GetCellValueSafely(dataRow, 16);
 student.IELTSScore = GetCellValueSafely(dataRow, 17);
 
 // Program Details
 student.University = GetCellValueSafely(dataRow, 18);
 student.UniversityCountry = GetCellValueSafely(dataRow, 19);
 student.UniversityCity = GetCellValueSafely(dataRow, 20);
 student.IntendedProgramMajor = GetCellValueSafely(dataRow, 21);
 student.PreferredStartSemester = GetCellValueSafely(dataRow, 22);
 student.ModeOfStudy = GetCellValueSafely(dataRow, 23);
 
 // Certifications
 for (int i = 0; i < 3; i++)
 {
 var certName = GetCellValueSafely(dataRow, 24 + i * 3);
 var certYear = GetCellValueSafely(dataRow, 25 + i * 3);
 var certScore = GetCellValueSafely(dataRow, 26 + i * 3);
 if (!string.IsNullOrWhiteSpace(certName))
 {
 student.Certifications.Add(new Certification 
 { 
 Name = certName, 
 Year = certYear, 
 Score = certScore,
 StudentId = 0 // Will be set by EF when saving
 });
 }
 }
 
 // Work Experience
 student.TotalWorkExperience = GetCellValueSafely(dataRow, 33);
 student.LastRole = GetCellValueSafely(dataRow, 34);
 student.Domain = GetCellValueSafely(dataRow, 35);
 student.LastDrawnAnnualSalary = GetCellValueSafely(dataRow, 36);
 
 // Statement of Purpose
 student.MotivationOfCurrentProgram = GetCellValueSafely(dataRow, 37);
 student.RelevantProgramKnowledge = GetCellValueSafely(dataRow, 38);
 
 // Achievements
 for (int i = 0; i < 3; i++)
 {
 var achName = GetCellValueSafely(dataRow, 39 + i * 3);
 var achYear = GetCellValueSafely(dataRow, 40 + i * 3);
 var achRole = GetCellValueSafely(dataRow, 41 + i * 3);
 if (!string.IsNullOrWhiteSpace(achName))
 {
 student.Achievements.Add(new Achievement 
 { 
 Name = achName, 
 Year = achYear, 
 Role = achRole,
 StudentId = 0 // Will be set by EF when saving
 });
 }
 }
 
 // Volunteer
 for (int i = 0; i < 3; i++)
 {
 var volName = GetCellValueSafely(dataRow, 48 + i * 3);
 var volYear = GetCellValueSafely(dataRow, 49 + i * 3);
 var volRole = GetCellValueSafely(dataRow, 50 + i * 3);
 if (!string.IsNullOrWhiteSpace(volName))
 {
 student.Volunteers.Add(new Volunteer 
 { 
 ProgramName = volName, 
 Year = volYear, 
 ContributionRole = volRole,
 StudentId = 0 // Will be set by EF when saving
 });
 }
 }
 
 // Financial
 student.FundingSource = GetCellValueSafely(dataRow, 57);
 student.ScholarshipExpectations = GetCellValueSafely(dataRow, 58);
 
 // Visa
 student.VisaStatus = GetCellValueSafely(dataRow, 59);
 
 // Payment
 student.PaymentMethod = GetCellValueSafely(dataRow, 60);
 }
 }

 // Validate the student data
 var validationService = new ValidationService(_dbContext);
 var validationResults = await validationService.ValidateStudentAsync(student);
 
 if (validationResults.Any())
 {
 var errorMessages = string.Join("; ", validationResults.Select(v => v.ErrorMessage));
 throw new InvalidOperationException($"Validation failed: {errorMessages}");
 }

 student.IsProfileComplete = true;
 student.ApplicationStatus = "Submitted";
 student.ApplicationSubmittedDate = DateTime.Now;
 
 return student;
 }

 private string GetCellValueSafely(IRow row, int cellIndex)
 {
 try
 {
 var cell = row.GetCell(cellIndex);
 if (cell == null) return string.Empty;
 
 return cell.CellType switch
 {
 CellType.String => cell.StringCellValue ?? string.Empty,
 CellType.Numeric => cell.NumericCellValue.ToString(),
 CellType.Boolean => cell.BooleanCellValue.ToString(),
 CellType.Formula => cell.StringCellValue ?? string.Empty,
 _ => string.Empty
 };
 }
 catch
 {
 return string.Empty;
 }
 }

 private DateTime ParseDateSafely(string dateString)
 {
 if (string.IsNullOrEmpty(dateString))
 return DateTime.MinValue;
 
 if (DateTime.TryParse(dateString, out var date))
 return date;
 
 return DateTime.MinValue;
 }
 
 public async Task<string> GenerateUniversityFile(Student student)
 {
 if (student == null || !student.IsProfileComplete)
 {
 return null;
 }
 string universityName = student.University?.Replace(" ", "") ?? "Unknown";
 string filePath = _fileStorage.GetUniversityFilePath(universityName, DateTime.Today);
 bool fileExists = File.Exists(filePath);
 using (StreamWriter writer = new StreamWriter(filePath, append: true))
 {
 if (!fileExists)
 {
 writer.WriteLine("STUDENT_ID|FIRST_NAME|MIDDLE_NAME|LAST_NAME|EMAIL|PHONE|DOB|NATIONALITY|QUALIFICATION|UNIVERSITY|PROGRAM|SEMESTER|MODE|PAYMENT_METHOD");
 }
 writer.WriteLine($"{student.Id}|{student.FirstName}|{student.MiddleName}|{student.LastName}|{student.Email}|{student.PhoneNumber}|{student.DateOfBirth:MM/dd/yyyy}|{student.Nationality}|{student.HighestQualification}|{student.University}|{student.IntendedProgramMajor}|{student.PreferredStartSemester}|{student.ModeOfStudy}|{student.PaymentMethod}");
 }
 return filePath;
 }

 public async Task<string> GenerateXmlFile(Student student)
 {
 if (student == null)
 {
 return null;
 }
 string universityName = student.University?.Replace(" ", "") ?? "Unknown";
 string xmlFilePath = _fileStorage.GetXmlFilePath(student.Id.ToString(), universityName);
 XDocument xmlDoc = new XDocument(
 new XElement("StudentApplication",
 new XElement("StudentID", student.Id),
 new XElement("FirstName", student.FirstName),
 new XElement("MiddleName", student.MiddleName),
 new XElement("LastName", student.LastName),
 new XElement("Email", student.Email),
 new XElement("PhoneNumber", student.PhoneNumber),
 new XElement("DateOfBirth", student.DateOfBirth.ToString("MM/dd/yyyy")),
 new XElement("Gender", student.Gender),
 new XElement("Nationality", student.Nationality),
 new XElement("PassportNumber", student.PassportNumber),
 new XElement("PermanentAddress", student.PermanentAddress),
 new XElement("CurrentAddress", student.CurrentAddress),
 new XElement("SchoolAcademicBoard", student.SchoolAcademicBoard),
 new XElement("SchoolYearOfPass", student.SchoolYearOfPass),
 new XElement("SchoolGrade", student.SchoolGrade),
 new XElement("HighestQualification", student.HighestQualification),
 new XElement("HighestQualificationYearOfPass", student.HighestQualificationYearOfPass),
 new XElement("HighestQualificationGrades", student.HighestQualificationGrades),
 new XElement("IELTSScore", student.IELTSScore),
 new XElement("University", student.University),
 new XElement("UniversityCountry", student.UniversityCountry),
 new XElement("UniversityCity", student.UniversityCity),
 new XElement("IntendedProgramMajor", student.IntendedProgramMajor),
 new XElement("PreferredStartSemester", student.PreferredStartSemester),
 new XElement("ModeOfStudy", student.ModeOfStudy),
 new XElement("Certifications",
 student.Certifications != null ?
 new List<XElement>(student.Certifications.ConvertAll(c =>
 new XElement("Certification",
 new XElement("Name", c.Name),
 new XElement("Year", c.Year),
 new XElement("Score", c.Score)
))) : null
 ),
 new XElement("TotalWorkExperience", student.TotalWorkExperience),
 new XElement("LastRole", student.LastRole),
 new XElement("Domain", student.Domain),
 new XElement("LastDrawnAnnualSalary", student.LastDrawnAnnualSalary),
 new XElement("MotivationOfCurrentProgram", student.MotivationOfCurrentProgram),
 new XElement("RelevantProgramKnowledge", student.RelevantProgramKnowledge),
 new XElement("Achievements",
 student.Achievements != null ?
 new List<XElement>(student.Achievements.ConvertAll(a =>
 new XElement("Achievement",
 new XElement("Name", a.Name),
 new XElement("Year", a.Year),
 new XElement("Role", a.Role)
))) : null
 ),
 new XElement("Volunteers",
 student.Volunteers != null ?
 new List<XElement>(student.Volunteers.ConvertAll(v =>
 new XElement("Volunteer",
 new XElement("ProgramName", v.ProgramName),
 new XElement("Year", v.Year),
 new XElement("ContributionRole", v.ContributionRole)
))) : null
 ),
 new XElement("FundingSource", student.FundingSource),
 new XElement("ScholarshipExpectations", student.ScholarshipExpectations),
 new XElement("VisaStatus", student.VisaStatus),
 new XElement("PaymentMethod", student.PaymentMethod),
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
 int studentId = int.Parse(xmlDoc.Root.Element("StudentID").Value);
 var student = await _dbContext.Students.FindAsync(studentId);
 if (student == null)
 {
 throw new InvalidOperationException($"Student with ID {studentId} not found.");
 }
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