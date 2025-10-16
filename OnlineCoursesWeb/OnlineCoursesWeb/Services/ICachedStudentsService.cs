using OnlineCoursesWeb;
using OnlineCoursesWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineCoursesWeb.Services
{
    public interface ICachedStudentsService
    {
        public IEnumerable<Student> GetStudents(int rowsNumber = 20);
        public void AddStudents(string cacheKey, int rowsNumber = 20);
        public IEnumerable<Student> GetStudents(string cacheKey, int rowsNumber = 20);
    }
}