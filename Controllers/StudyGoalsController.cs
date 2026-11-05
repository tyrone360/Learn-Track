
using LearnTrack.Data;
using LearnTrack.Models;
using LearnTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnTrack.Controllers
{
    [Authorize]
    public class StudyGoalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GoalReminderService _reminderService;

        public StudyGoalsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            GoalReminderService reminderService)
        {
            _context = context;
            _userManager = userManager;
            _reminderService = reminderService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            // Generate reminders for upcoming study goals.
            await _reminderService.CreateUpcomingGoalRemindersAsync(user.Id);

            var goals = await _context.StudyGoals
                .Where(g => g.UserId == user.Id)
                .OrderBy(g => g.TargetDate)
                .ToListAsync();

            return View(goals);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudyGoal goal)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            if (!ModelState.IsValid)
                return View(goal);

            goal.Id = 0;
            goal.UserId = user.Id;
            goal.IsCompleted = false;

            _context.StudyGoals.Add(goal);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Study goal created successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var goal = await _context.StudyGoals
                .FirstOrDefaultAsync(g =>
                    g.Id == id && g.UserId == user.Id);

            if (goal == null)
                return NotFound();

            goal.IsCompleted = !goal.IsCompleted;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Study goal status updated!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var goal = await _context.StudyGoals
                .FirstOrDefaultAsync(g =>
                    g.Id == id && g.UserId == user.Id);

            if (goal == null)
                return NotFound();

            _context.StudyGoals.Remove(goal);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Study goal deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
