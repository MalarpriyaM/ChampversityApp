using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Champversity.DataAccess.Models
{
    public class Student
    {
  public int Id { get; set; }
        
      [Required]
  public string FullName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
    public string PhoneNumber { get; set; }
        
 [Required]
     public DateTime DateOfBirth { get; set; }
   
        [Required]
        public string Nationality { get; set; }
        
        [Required]
        public string HighestQualification { get; set; }
     
        [Required]
        public string PreferredUniversity { get; set; }
  
        [Required]
        public string PreferredCourse { get; set; }
        
  public bool IsProfileComplete { get; set; }
        
      public string UniversityFileReference { get; set; }
     
        public DateTime ApplicationSubmittedDate { get; set; }
        
        public string ApplicationStatus { get; set; }
   
        public List<InterviewSlot> InterviewSlots { get; set; }
  
     public InterviewSlot SelectedSlot { get; set; }
    }

    public class InterviewSlot
    {
     public int Id { get; set; }
 public DateTime SlotDateTime { get; set; }
        public bool IsSelected { get; set; }
        public int StudentId { get; set; }
    }
}