using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Data;
using System.Threading.Tasks;

namespace OnlineCoursesMVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly Db31048Context _context;

        public CoursesController(Db31048Context context)
        {
            _context = context;
        }

        // GET: /Courses
        [ResponseCache(CacheProfileName = "DefaultCache")]
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.Include(c => c.Instructor).ToListAsync();
            return View(courses);
        }
    }
}