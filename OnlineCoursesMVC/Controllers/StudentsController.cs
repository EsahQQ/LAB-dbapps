using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesMVC.Data;
using OnlineCoursesMVC.Infrastructure;
using OnlineCoursesMVC.Models;
using System.Threading.Tasks;

namespace OnlineCoursesMVC.Controllers;

[Authorize]
public class StudentsController : Controller
{
    private readonly Db31048Context _context;

    public StudentsController(Db31048Context context)
    {
        _context = context;
    }

    // GET: Students
    public async Task<IActionResult> Index(string searchString, int? pageNumber, string clearFilter)
    {
        if (clearFilter != null)
        {
            HttpContext.Session.Remove("StudentsSearchString");
            return RedirectToAction(nameof(Index));
        }

        if (searchString != null)
        {
            HttpContext.Session.SetString("StudentsSearchString", searchString);
        }
        else
        {
            searchString = HttpContext.Session.GetString("StudentsSearchString");
        }

        ViewData["CurrentFilter"] = searchString;

        var students = from s in _context.Students
                       select s;

        if (!String.IsNullOrEmpty(searchString))
        {
            students = students.Where(s => s.FullName.Contains(searchString) || s.Email.Contains(searchString));
        }

        students = students.OrderBy(s => s.FullName);

        int pageSize = 2;
        return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FirstOrDefaultAsync(m => m.StudentId == id);
        if (student == null) return NotFound();
        return View(student);
    }

    // GET: Students/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Students/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FullName,Email,RegistrationDate")] Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    // POST: Students/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("StudentId,FullName,Email,RegistrationDate")] Student student)
    {
        if (id != student.StudentId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.StudentId == student.StudentId)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FirstOrDefaultAsync(m => m.StudentId == id);
        if (student == null) return NotFound();
        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}