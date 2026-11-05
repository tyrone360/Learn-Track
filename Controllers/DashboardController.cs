
using LearnTrack.Data;
using LearnTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnTrack.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var courses = await _context.Courses
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            var goals = await _context.StudyGoals
                .Where(g => g.UserId == user.Id)
                .ToListAsync();

            ViewBag.TotalCourses = courses.Count;

ViewBag.CourseTitles = courses
    .Select(c => c.Title)
    .ToList();

ViewBag.CourseProgress = courses
    .Select(c => c.ProgressPercentage)
    .ToList();
            ViewBag.AverageProgress = courses.Any()
                ? Math.Round(courses.Average(c => c.ProgressPercentage))
                : 0;

            ViewBag.TotalGoals = goals.Count;

            ViewBag.CompletedGoals = goals.Count(g => g.IsCompleted);

            return View(user);
        }
    }
}
