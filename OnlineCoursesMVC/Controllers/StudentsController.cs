using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Data;
using System.Threading.Tasks;

namespace OnlineCoursesMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly Db31048Context _context;

        public StudentsController(Db31048Context context)
        {
            _context = context;
        }

        [ResponseCache(CacheProfileName = "DefaultCache")]
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.ToListAsync();

            return View(students);
        }
    }
}