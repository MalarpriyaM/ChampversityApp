using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Champversity.DataAccess.Models
{
    public class Student
    {
  public int Id { get; set; }

 // Personal Information
 public string FirstName { get; set; } = string.Empty;
 public string MiddleName { get; set; } = string.Empty;
 public string LastName { get; set; } = string.Empty;
 public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
 public string Gender { get; set; } = string.Empty;
 public string Nationality { get; set; } = string.Empty;
 public string PassportNumber { get; set; } = string.Empty;
 public string PhoneNumber { get; set; } = string.Empty;
 public string Email { get; set; } = string.Empty;
 public string PermanentAddress { get; set; } = string.Empty;
 public string CurrentAddress { get; set; } = string.Empty;

 // Academic Background
 public string SchoolAcademicBoard { get; set; } = string.Empty;
 public string SchoolYearOfPass { get; set; } = string.Empty;
 public string SchoolGrade { get; set; } = string.Empty;
 public string HighestQualification { get; set; } = string.Empty;
 public string HighestQualificationYearOfPass { get; set; } = string.Empty;
 public string HighestQualificationGrades { get; set; } = string.Empty;
 public string IELTSScore { get; set; } = string.Empty;

 // Program Details
 public string University { get; set; } = string.Empty;
 public string UniversityCountry { get; set; } = string.Empty;
 public string UniversityCity { get; set; } = string.Empty;
 public string IntendedProgramMajor { get; set; } = string.Empty;
 public string PreferredStartSemester { get; set; } = string.Empty;
 public string ModeOfStudy { get; set; } = string.Empty;

 // Certifications (Optional)
 public List<Certification> Certifications { get; set; } = new();

 // Work Experience (Optional)
 public string TotalWorkExperience { get; set; } = string.Empty;
 public string LastRole { get; set; } = string.Empty;
 public string Domain { get; set; } = string.Empty;
 public string LastDrawnAnnualSalary { get; set; } = string.Empty;

 // Statement of Purpose
 public string MotivationOfCurrentProgram { get; set; } = string.Empty;
 public string RelevantProgramKnowledge { get; set; } = string.Empty;

 // Awards and Achievements (Optional)
 public List<Achievement> Achievements { get; set; } = new();

 // Volunteer (Optional)
 public List<Volunteer> Volunteers { get; set; } = new();

 // Financial Information
 public string FundingSource { get; set; } = string.Empty;
 public string ScholarshipExpectations { get; set; } = string.Empty;

 // Visa
 public string VisaStatus { get; set; } = string.Empty;

 // Application Fee Payment
 public string PaymentMethod { get; set; } = string.Empty;

 // Legacy fields
 public bool IsProfileComplete { get; set; } = false;
 public string UniversityFileReference { get; set; } = string.Empty;
 public DateTime ApplicationSubmittedDate { get; set; } = DateTime.Now;
 public string ApplicationStatus { get; set; } = "Draft";
 public List<InterviewSlot> InterviewSlots { get; set; } = new();
 public InterviewSlot? SelectedSlot { get; set; }
 }

 public class Certification
 {
 public int Id { get; set; }
 public string Name { get; set; } = string.Empty;
 public string Year { get; set; } = string.Empty;
 public string Score { get; set; } = string.Empty;
 public int StudentId { get; set; }
 }

 public class Achievement
 {
 public int Id { get; set; }
 public string Name { get; set; } = string.Empty;
 public string Year { get; set; } = string.Empty;
 public string Role { get; set; } = string.Empty;
 public int StudentId { get; set; }
 }

 public class Volunteer
 {
 public int Id { get; set; }
 public string ProgramName { get; set; } = string.Empty;
 public string Year { get; set; } = string.Empty;
 public string ContributionRole { get; set; } = string.Empty;
 public int StudentId { get; set; }
 }

 public class InterviewSlot
 {
 public int Id { get; set; }
 public DateTime SlotDateTime { get; set; }
 public bool IsSelected { get; set; } = false;
 public int StudentId { get; set; }
 }
}