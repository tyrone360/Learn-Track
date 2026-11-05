
using System.ComponentModel.DataAnnotations;

namespace LearnTrack.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}
