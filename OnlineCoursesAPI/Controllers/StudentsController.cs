using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesAPI.Data;
using OnlineCoursesAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineCoursesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly Db31048Context _context;
        public TestsController(Db31048Context context) => _context = context;

        // GET: api/Tests
        /// <summary>
        /// Получает краткий список всех тестов (ID и Название)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTests()
        {
            return await _context.Tests
                .Select(t => new { t.TestId, t.Title })
                .ToListAsync();
        }
    }
}