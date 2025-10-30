using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Data;
using System.Threading.Tasks;

namespace OnlineCoursesMVC.Controllers
{
    public class TestResultsController : Controller
    {
        private readonly Db31048Context _context;

        public TestResultsController(Db31048Context context)
        {
            _context = context;
        }

        // GET: /TestResults
        [ResponseCache(CacheProfileName = "DefaultCache")]
        public async Task<IActionResult> Index()
        {
            var testResults = _context.TestResults
                .Include(tr => tr.Student) 
                .Include(tr => tr.Test);  

            return View(await testResults.ToListAsync());
        }
    }
}