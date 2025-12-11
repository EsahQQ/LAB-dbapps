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
    public class StudentsController : ControllerBase
    {
        private readonly Db31048Context _context;
        public StudentsController(Db31048Context context) => _context = context;

        // GET: api/Students
        /// <summary>
        /// Получает краткий список всех студентов (ID и Имя)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetStudents()
        {
            return await _context.Students
                .Select(s => new { s.StudentId, s.FullName })
                .ToListAsync();
        }
    }
}