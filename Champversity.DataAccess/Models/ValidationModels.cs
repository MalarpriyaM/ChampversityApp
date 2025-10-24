using System.ComponentModel.DataAnnotations;

namespace Champversity.DataAccess.Models
{
    public class ValidationRule
    {
 public int Id { get; set; }
  public string FieldName { get; set; }
        public string RuleType { get; set; } // Required, MinLength, MaxLength, Range, Regex, etc.
      public string RuleValue { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsActive { get; set; }
    }

    public class ManualTask
  {
        public int Id { get; set; }
 public int StudentId { get; set; }
 public string TaskType { get; set; } // CallForSlotConfirmation, DocumentVerification, etc.
    public string TaskDescription { get; set; }
      public string AssignedTo { get; set; }
     public string Status { get; set; } // Pending, InProgress, Completed, Cancelled
   public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; }
        public string Priority { get; set; } // High, Medium, Low
    }

    public class UniversityResponse
    {
        public int Id { get; set; }
  public int StudentId { get; set; }
        public string UniversityName { get; set; }
     public string ResponseType { get; set; } // InterviewSlots, Acceptance, Rejection
        public string ResponseData { get; set; } // JSON or XML data
      public DateTime ReceivedDate { get; set; }
     public bool IsProcessed { get; set; }
   public string ProcessingNotes { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
     public List<string> Errors { get; set; } = new List<string>();
        public string FieldName { get; set; }
   public string ErrorMessage { get; set; }
    }
}