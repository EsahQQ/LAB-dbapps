using Microsoft.Extensions.Caching.Memory;
using OnlineCoursesWeb.Data;
using System.Collections;


namespace OnlineCoursesWeb.Services;

public class DataCacheService : IDataCacheService
{
    private readonly Db28021Context _db;
    private readonly IMemoryCache _cache;
    private readonly int _cacheDurationSeconds;

    private readonly List<string> _tablesToCache = new List<string>
    {
        "students", "courses", "instructors", "enrollments",
        "testresults", "certificates", "modules", "tests"
    };

    public DataCacheService(Db28021Context context, IMemoryCache memoryCache)
    {
        _db = context;
        _cache = memoryCache;

        int number = 9;
        _cacheDurationSeconds = 2 * number + 240;
    }

    public void PrimeCache()
    {
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(_cacheDurationSeconds));

        foreach (var tableName in _tablesToCache)
        {
            var data = GetFromDb(tableName);
            if (data != null)
            {
                _cache.Set(tableName, data, cacheOptions);
            }
        }
        Console.WriteLine($"--> Все таблицы кэшированы на {_cacheDurationSeconds} секунд.");
    }

    public IEnumerable GetData(string tableName)
    {
        _cache.TryGetValue(tableName.ToLower(), out IEnumerable data);
        return data;
    }

    private IEnumerable GetFromDb(string tableName)
    {
        return tableName.ToLower() switch
        {
            "students" => _db.Students.OrderBy(s => s.StudentId).Take(20).ToList(),
            "courses" => _db.Courses.OrderBy(c => c.CourseId).Take(20).ToList(),
            "instructors" => _db.Instructors.OrderBy(i => i.InstructorId).Take(20).ToList(),
            "enrollments" => _db.Enrollments.OrderBy(e => e.EnrollmentId).Take(20).ToList(),
            "testresults" => _db.TestResults.OrderBy(tr => tr.ResultId).Take(20).ToList(),
            "certificates" => _db.Certificates.OrderBy(c => c.CertificateId).Take(20).ToList(),
            "modules" => _db.Modules.OrderBy(m => m.ModuleId).Take(20).ToList(),
            "tests" => _db.Tests.OrderBy(t => t.TestId).Take(20).ToList(),
            _ => null 
        };
    }
}