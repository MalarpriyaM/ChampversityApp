using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Champversity.DataAccess.Models
{
    public class Student
    {
  public int Id { get; set; }

 // Personal Information
 [Required] public string FirstName { get; set; }
 public string MiddleName { get; set; }
 [Required] public string LastName { get; set; }
 [Required] public DateTime DateOfBirth { get; set; }
 public string Gender { get; set; }
 [Required] public string Nationality { get; set; }
 public string PassportNumber { get; set; }
 [Required] public string PhoneNumber { get; set; }
 [Required, EmailAddress] public string Email { get; set; }
 public string PermanentAddress { get; set; }
 public string CurrentAddress { get; set; }

 // Academic Background
 public string SchoolAcademicBoard { get; set; }
 public string SchoolYearOfPass { get; set; }
 public string SchoolGrade { get; set; }
 public string HighestQualification { get; set; }
 public string HighestQualificationYearOfPass { get; set; }
 public string HighestQualificationGrades { get; set; }
 public string IELTSScore { get; set; }

 // Program Details
 public string University { get; set; }
 public string UniversityCountry { get; set; }
 public string UniversityCity { get; set; }
 public string IntendedProgramMajor { get; set; }
 public string PreferredStartSemester { get; set; }
 public string ModeOfStudy { get; set; }

 // Certifications (Optional)
 public List<Certification> Certifications { get; set; } = new();

 // Work Experience (Optional)
 public string TotalWorkExperience { get; set; }
 public string LastRole { get; set; }
 public string Domain { get; set; }
 public string LastDrawnAnnualSalary { get; set; }

 // Statement of Purpose
 public string MotivationOfCurrentProgram { get; set; }
 public string RelevantProgramKnowledge { get; set; }

 // Awards and Achievements (Optional)
 public List<Achievement> Achievements { get; set; } = new();

 // Volunteer (Optional)
 public List<Volunteer> Volunteers { get; set; } = new();

 // Financial Information
 public string FundingSource { get; set; }
 public string ScholarshipExpectations { get; set; }

 // Visa
 public string VisaStatus { get; set; }

 // Application Fee Payment
 public string PaymentMethod { get; set; }

 // Legacy fields
 public bool IsProfileComplete { get; set; }
 public string UniversityFileReference { get; set; }
 public DateTime ApplicationSubmittedDate { get; set; }
 public string ApplicationStatus { get; set; }
 public List<InterviewSlot> InterviewSlots { get; set; }
 public InterviewSlot SelectedSlot { get; set; }
 }

 public class Certification
 {
 public int Id { get; set; }
 public string Name { get; set; }
 public string Year { get; set; }
 public string Score { get; set; }
 public int StudentId { get; set; }
 }

 public class Achievement
 {
 public int Id { get; set; }
 public string Name { get; set; }
 public string Year { get; set; }
 public string Role { get; set; }
 public int StudentId { get; set; }
 }

 public class Volunteer
 {
 public int Id { get; set; }
 public string ProgramName { get; set; }
 public string Year { get; set; }
 public string ContributionRole { get; set; }
 public int StudentId { get; set; }
 }

 public class InterviewSlot
 {
 public int Id { get; set; }
 public DateTime SlotDateTime { get; set; }
 public bool IsSelected { get; set; }
 public int StudentId { get; set; }
 }
}