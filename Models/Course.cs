
using System.ComponentModel.DataAnnotations;

namespace LearnTrack.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(0, 100)]
        public int ProgressPercentage { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}
