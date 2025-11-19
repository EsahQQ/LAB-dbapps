using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Data;
using OnlineCoursesMVC.Infrastructure;
using OnlineCoursesMVC.Models;
using System.Threading.Tasks;

namespace OnlineCoursesMVC.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly Db31048Context _context;

        public CoursesController(Db31048Context context)
        {
            _context = context;
        }

        // GET: /Courses
        public async Task<IActionResult> Index(string searchString, int? pageNumber, string clearFilter)
        {
            if (clearFilter != null)
            {
                HttpContext.Session.Remove("CoursesSearchString");
            }

            if (searchString != null)
            {
                HttpContext.Session.SetString("CoursesSearchString", searchString);
            }
            else
            {
                searchString = HttpContext.Session.GetString("CoursesSearchString");
            }

            ViewData["CurrentFilter"] = searchString;

            var courses = from c in _context.Courses.Include(c => c.Instructor)
                          select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c => c.Title.Contains(searchString));
            }

            int pageSize = 2; //2
            return View(await PaginatedList<Course>.CreateAsync(courses.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            var viewModel = new CourseViewModel
            {
                InstructorOptions = _context.Instructors.Select(i => new SelectListItem
                {
                    Value = i.InstructorId.ToString(),
                    Text = i.FullName
                }).ToList()
            };
            return View(viewModel);
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Difficulty = viewModel.Difficulty,
                    Category = viewModel.Category,
                    Status = viewModel.Status,
                    InstructorId = viewModel.InstructorId
                };

                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            viewModel.InstructorOptions = _context.Instructors.Select(i => new SelectListItem
            {
                Value = i.InstructorId.ToString(),
                Text = i.FullName
            }).ToList();
            return View(viewModel);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Instructor) 
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new CourseViewModel
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                Category = course.Category,
                Difficulty = course.Difficulty,
                Status = course.Status,
                InstructorId = course.InstructorId,

                InstructorOptions = _context.Instructors.Select(i => new SelectListItem
                {
                    Value = i.InstructorId.ToString(),
                    Text = i.FullName
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseViewModel viewModel)
        {
            if (id != viewModel.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var course = await _context.Courses.FindAsync(id);
                    if (course == null)
                    {
                        return NotFound();
                    }

                    course.Title = viewModel.Title;
                    course.Description = viewModel.Description;
                    course.Category = viewModel.Category;
                    course.Difficulty = viewModel.Difficulty;
                    course.Status = viewModel.Status;
                    course.InstructorId = viewModel.InstructorId;

                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Courses.Any(e => e.CourseId == viewModel.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.InstructorOptions = new SelectList(_context.Instructors, "InstructorId", "FullName", viewModel.InstructorId);
            return View(viewModel);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Instructor) 
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}