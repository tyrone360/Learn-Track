
using LearnTrack.Data;
using LearnTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnTrack.Services
{
    public class GoalReminderService
    {
        private readonly ApplicationDbContext _context;

        public GoalReminderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateUpcomingGoalRemindersAsync(string userId)
        {
            var today = DateTime.Today;
            var reminderLimit = today.AddDays(3);

            var upcomingGoals = await _context.StudyGoals
                .Where(g =>
                    g.UserId == userId &&
                    !g.IsCompleted &&
                    g.TargetDate.Date >= today &&
                    g.TargetDate.Date <= reminderLimit)
                .ToListAsync();

            foreach (var goal in upcomingGoals)
            {
                var message =
                    $"Reminder: Your study goal '{goal.Title}' is due on {goal.TargetDate:dd MMM yyyy}.";

                var alreadyExists = await _context.Notifications
                    .AnyAsync(n =>
                        n.UserId == userId &&
                        n.Message == message);

                if (!alreadyExists)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = userId,
                        Message = message,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
