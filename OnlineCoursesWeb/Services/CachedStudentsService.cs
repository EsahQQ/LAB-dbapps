using OnlineCoursesWeb.Data;
using OnlineCoursesWeb.Models;
using Microsoft.Extensions.Caching.Memory;
using OnlineCoursesWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCoursesWeb.Services
{
    public class CachedStudentsService(Db28021Context dbContext, IMemoryCache memoryCache) : ICachedStudentsService
    {
        private readonly Db28021Context _dbContext = dbContext;
        private readonly IMemoryCache _memoryCache = memoryCache;

        public IEnumerable<Student> GetStudents(int rowsNumber = 20)
        {
            return _dbContext.Students.Take(rowsNumber).ToList();
        }

        public void AddStudents(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Student> students = _dbContext.Students.Take(rowsNumber).ToList();
            if (students != null)
            {
                _memoryCache.Set(cacheKey, students, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            }

        }

        public IEnumerable<Student> GetStudents(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Student> students))
            {
                students = _dbContext.Students.Take(rowsNumber).ToList();
                if (students != null)
                {
                    _memoryCache.Set(cacheKey, students,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return students;
        }
    }
}