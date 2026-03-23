using Microsoft.AspNetCore.Identity;

namespace Champversity.DataAccess.Models
{
    public class ApplicationUser : IdentityUser
    {
    public string FirstName { get; set; } = string.Empty;
public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}