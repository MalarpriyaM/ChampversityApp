using System.ComponentModel.DataAnnotations;

namespace Champversity.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

 [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalApplications { get; set; }
  public int PendingTasks { get; set; }
        public int InterviewSlotsAvailable { get; set; }
        public int UniversityResponsesPending { get; set; }
   public List<RecentApplicationViewModel> RecentApplications { get; set; } = new List<RecentApplicationViewModel>();
        public List<PendingTaskViewModel> PendingTasks_List { get; set; } = new List<PendingTaskViewModel>();
    }

    public class RecentApplicationViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string University { get; set; }
        public string Status { get; set; }
 public DateTime SubmittedDate { get; set; }
    }

    public class PendingTaskViewModel
    {
      public int Id { get; set; }
        public string TaskType { get; set; }
   public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StudentName { get; set; }
    }
}