
using Microsoft.AspNetCore.Identity;

namespace LearnTrack.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? OrganisationName { get; set; }
    }
}
