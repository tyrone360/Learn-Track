
using LearnTrack.Data;
using LearnTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnTrack.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Display the signed-in user's courses
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var courses = await _context.Courses
                .Where(c => c.UserId == user.Id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(courses);
        }

        // Display the create form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Save a new course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            ModelState.Remove(nameof(course.UserId));
            ModelState.Remove(nameof(course.User));

            if (!ModelState.IsValid)
                return View(course);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            course.Id = 0;
            course.UserId = user.Id;
            course.User = null;
            course.CreatedAt = DateTime.UtcNow;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course created successfully!";

            return RedirectToAction(nameof(Index));
        }

        // Display the edit form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.Id == id && c.UserId == user.Id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        // Save course changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Description,Category")] Course updatedCourse)
        {
            if (id != updatedCourse.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(updatedCourse);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.Id == id && c.UserId == user.Id);

            if (course == null)
                return NotFound();

            course.Title = updatedCourse.Title;
            course.Description = updatedCourse.Description;
            course.Category = updatedCourse.Category;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Course updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        // Delete a course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.Id == id && c.UserId == user.Id);

            if (course == null)
                return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
