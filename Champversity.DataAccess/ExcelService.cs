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
 student.FirstName = dataRow.GetCell(0)?.StringCellValue;
 student.MiddleName = dataRow.GetCell(1)?.StringCellValue;
 student.LastName = dataRow.GetCell(2)?.StringCellValue;
 student.DateOfBirth = DateTime.TryParse(dataRow.GetCell(3)?.StringCellValue, out var dob) ? dob : DateTime.MinValue;
 student.Gender = dataRow.GetCell(4)?.StringCellValue;
 student.Nationality = dataRow.GetCell(5)?.StringCellValue;
 student.PassportNumber = dataRow.GetCell(6)?.StringCellValue;
 student.PhoneNumber = dataRow.GetCell(7)?.StringCellValue;
 student.Email = dataRow.GetCell(8)?.StringCellValue;
 student.PermanentAddress = dataRow.GetCell(9)?.StringCellValue;
 student.CurrentAddress = dataRow.GetCell(10)?.StringCellValue;
 // Academic Background
 student.SchoolAcademicBoard = dataRow.GetCell(11)?.StringCellValue;
 student.SchoolYearOfPass = dataRow.GetCell(12)?.StringCellValue;
 student.SchoolGrade = dataRow.GetCell(13)?.StringCellValue;
 student.HighestQualification = dataRow.GetCell(14)?.StringCellValue;
 student.HighestQualificationYearOfPass = dataRow.GetCell(15)?.StringCellValue;
 student.HighestQualificationGrades = dataRow.GetCell(16)?.StringCellValue;
 student.IELTSScore = dataRow.GetCell(17)?.StringCellValue;
 // Program Details
 student.University = dataRow.GetCell(18)?.StringCellValue;
 student.UniversityCountry = dataRow.GetCell(19)?.StringCellValue;
 student.UniversityCity = dataRow.GetCell(20)?.StringCellValue;
 student.IntendedProgramMajor = dataRow.GetCell(21)?.StringCellValue;
 student.PreferredStartSemester = dataRow.GetCell(22)?.StringCellValue;
 student.ModeOfStudy = dataRow.GetCell(23)?.StringCellValue;
 // Certifications
 for (int i = 0; i < 3; i++)
 {
 var certName = dataRow.GetCell(24 + i * 3)?.StringCellValue;
 var certYear = dataRow.GetCell(25 + i * 3)?.StringCellValue;
 var certScore = dataRow.GetCell(26 + i * 3)?.StringCellValue;
 if (!string.IsNullOrWhiteSpace(certName))
 {
 student.Certifications.Add(new Certification { Name = certName, Year = certYear, Score = certScore });
 }
 }
 // Work Experience
 student.TotalWorkExperience = dataRow.GetCell(33)?.StringCellValue;
 student.LastRole = dataRow.GetCell(34)?.StringCellValue;
 student.Domain = dataRow.GetCell(35)?.StringCellValue;
 student.LastDrawnAnnualSalary = dataRow.GetCell(36)?.StringCellValue;
 // Statement of Purpose
 student.MotivationOfCurrentProgram = dataRow.GetCell(37)?.StringCellValue;
 student.RelevantProgramKnowledge = dataRow.GetCell(38)?.StringCellValue;
 // Achievements
 for (int i = 0; i < 3; i++)
 {
 var achName = dataRow.GetCell(39 + i * 3)?.StringCellValue;
 var achYear = dataRow.GetCell(40 + i * 3)?.StringCellValue;
 var achRole = dataRow.GetCell(41 + i * 3)?.StringCellValue;
 if (!string.IsNullOrWhiteSpace(achName))
 {
 student.Achievements.Add(new Achievement { Name = achName, Year = achYear, Role = achRole });
 }
 }
 // Volunteer
 for (int i = 0; i < 3; i++)
 {
 var volName = dataRow.GetCell(48 + i * 3)?.StringCellValue;
 var volYear = dataRow.GetCell(49 + i * 3)?.StringCellValue;
 var volRole = dataRow.GetCell(50 + i * 3)?.StringCellValue;
 if (!string.IsNullOrWhiteSpace(volName))
 {
 student.Volunteers.Add(new Volunteer { ProgramName = volName, Year = volYear, ContributionRole = volRole });
 }
 }
 // Financial
 student.FundingSource = dataRow.GetCell(57)?.StringCellValue;
 student.ScholarshipExpectations = dataRow.GetCell(58)?.StringCellValue;
 // Visa
 student.VisaStatus = dataRow.GetCell(59)?.StringCellValue;
 // Payment
 student.PaymentMethod = dataRow.GetCell(60)?.StringCellValue;
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