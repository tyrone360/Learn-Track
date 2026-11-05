
using LearnTrack.Data;
using LearnTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnTrack.Controllers
{
    [Authorize]
    public class ProgressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProgressController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Display progress for the signed-in user's courses
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var courses = await _context.Courses
                .Where(c => c.UserId == user.Id)
                .OrderBy(c => c.Title)
                .ToListAsync();

            ViewBag.TotalCourses = courses.Count;

            ViewBag.AverageProgress = courses.Count > 0
                ? Math.Round(courses.Average(c => c.ProgressPercentage))
                : 0;

            return View(courses);
        }

        // Update progress for a specific course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            int id, int progressPercentage)
        {
            if (progressPercentage < 0 || progressPercentage > 100)
            {
                TempData["Error"] =
                    "Progress must be between 0 and 100.";

                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.Id == id && c.UserId == user.Id);

            if (course == null)
                return NotFound();

            course.ProgressPercentage = progressPercentage;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Course progress updated successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
