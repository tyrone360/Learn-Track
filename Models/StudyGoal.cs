using System.ComponentModel.DataAnnotations;

namespace LearnTrack.Models
{
    public class StudyGoal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime TargetDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}