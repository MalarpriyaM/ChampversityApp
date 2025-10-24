using System.ComponentModel.DataAnnotations;

namespace Champversity.DataAccess.Models
{
    public class ValidationRule
    {
      public int Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string RuleType { get; set; } = string.Empty;
        public string RuleValue { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
      public bool IsActive { get; set; } = true;
    }

    public class ManualTask
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
  public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
    }

    public class UniversityResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string UniversityName { get; set; } = string.Empty;
        public string ResponseType { get; set; } = string.Empty;
        public string ResponseData { get; set; } = string.Empty;
        public DateTime ReceivedDate { get; set; } = DateTime.Now;
     public bool IsProcessed { get; set; } = false;
     public string ProcessingNotes { get; set; } = string.Empty;
 }

    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
 public List<string> Errors { get; set; } = new List<string>();
    public string FieldName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    }
}