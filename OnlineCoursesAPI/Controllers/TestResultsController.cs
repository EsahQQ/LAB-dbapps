using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesAPI.Data;
using OnlineCoursesAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCoursesAPI.Controllers;

/// <summary>
/// Контроллер для управления результатами тестов
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TestResultsController : ControllerBase
{
    private readonly Db31048Context _context;

    public TestResultsController(Db31048Context context)
    {
        _context = context;
    }


    /// <summary>
    /// Получает список всех результатов тестов.
    /// </summary>
    /// <returns>Список результатов тестов с именами студентов и названиями тестов.</returns>
    // GET: api/TestResults
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestResultViewModel>>> GetTestResults()
    {
        return await _context.TestResults
            .Include(tr => tr.Student)
            .Include(tr => tr.Test)
            .Select(tr => new TestResultViewModel
            {
                TestResultId = tr.TestResultId,
                StudentId = tr.StudentId,
                StudentName = tr.Student.FullName,
                TestId = tr.TestId,
                TestTitle = tr.Test.Title,
                Score = tr.Score,
                CompletionDate = tr.CompletionDate
            })
            .ToListAsync();
    }

    /// <summary>
    /// Получает конкретный результат теста по его ID.
    /// </summary>
    /// <param name="id">Уникальный идентификатор результата теста.</param>
    /// <returns>Найденный результат теста или ошибку 404.</returns>
    // GET: api/TestResults/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TestResultViewModel>> GetTestResult(long id) 
    {
        var testResult = await _context.TestResults
            .Include(tr => tr.Student)
            .Include(tr => tr.Test)
            .FirstOrDefaultAsync(tr => tr.TestResultId == id); 

        if (testResult == null)
        {
            return NotFound();
        }

        var viewModel = new TestResultViewModel
        {
            TestResultId = testResult.TestResultId,
            StudentId = testResult.StudentId,
            StudentName = testResult.Student.FullName, 
            TestId = testResult.TestId,
            TestTitle = testResult.Test.Title,   
            Score = testResult.Score,
            CompletionDate = testResult.CompletionDate
        };

        return viewModel;
    }

    /// <summary>
    /// Создает новую запись о результате теста.
    /// </summary>
    /// <param name="dto">Данные для создания нового результата.</param>
    /// <returns>Созданный объект результата теста.</returns>
    // POST: api/TestResults
    [HttpPost]
    public async Task<ActionResult<TestResult>> PostTestResult(TestResultCreateDto dto) 
    {
        var testResult = new TestResult
        {
            StudentId = dto.StudentId,
            TestId = dto.TestId,
            Score = dto.Score,
            CompletionDate = dto.CompletionDate
        };

        _context.TestResults.Add(testResult);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTestResult", new { id = testResult.TestResultId }, testResult);
    }

    /// <summary>
    /// Обновляет существующую запись о результате теста.
    /// </summary>
    /// <param name="id">ID результата, который нужно обновить.</param>
    /// <param name="dto">Новые данные для результата теста.</param>
    /// <returns>Ничего (статус 204), если обновление прошло успешно.</returns>
    // PUT: api/TestResults/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTestResult(long id, TestResultUpdateDto dto)
    {
        if (id != dto.TestResultId)
        {
            return BadRequest();
        }

        var testResultInDb = await _context.TestResults.FindAsync(id);
        if (testResultInDb == null)
        {
            return NotFound();
        }

        testResultInDb.StudentId = dto.StudentId;
        testResultInDb.TestId = dto.TestId;
        testResultInDb.Score = dto.Score;
        testResultInDb.CompletionDate = dto.CompletionDate;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.TestResults.Any(e => e.TestResultId == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Удаляет результат теста по его ID.
    /// </summary>
    /// <param name="id">ID результата, который нужно удалить.</param>
    /// <returns>Ничего (статус 204), если удаление прошло успешно.</returns>
    // DELETE: api/TestResults/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTestResult(long id)
    {
        var testResult = await _context.TestResults.FindAsync(id);
        if (testResult == null)
        {
            return NotFound();
        }

        _context.TestResults.Remove(testResult);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}