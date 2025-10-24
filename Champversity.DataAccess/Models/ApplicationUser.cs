using Microsoft.AspNetCore.Identity;

namespace Champversity.DataAccess.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
public string LastName { get; set; }
        public string Role { get; set; } // Admin, Staff, etc.
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}