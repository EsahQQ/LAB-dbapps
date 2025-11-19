using OnlineCoursesMVC.Models;
using System;
using System.Linq;

namespace OnlineCoursesMVC.Data
{
    public static class DbInitializer
    {
        public static void Initialize(Db31048Context context)
        {
            context.Database.EnsureCreated();

            if (context.Students.Any())
            {
                return;
            }

            var instructors = new Instructor[]
            {
                new Instructor { FullName = "Профессор Андрей Соколов" },
                new Instructor { FullName = "Доцент Мария Петрова" },
                new Instructor { FullName = "Лектор Сергей Васильев" }
            };
            context.Instructors.AddRange(instructors);
            context.SaveChanges();

            var courses = new Course[]
            {
                new Course { Title = "Введение в C#", Difficulty = "Начальный", Category = "Программирование", Status = "Активен", InstructorId = instructors[0].InstructorId },
                new Course { Title = "Продвинутый SQL", Difficulty = "Продвинутый", Category = "Базы данных", Status = "Активен", InstructorId = instructors[1].InstructorId },
                new Course { Title = "Основы UI/UX", Difficulty = "Начальный", Category = "Дизайн", Status = "В архиве", InstructorId = instructors[2].InstructorId },
                new Course { Title = "ASP.NET Core MVC", Difficulty = "Средний", Category = "Программирование", Status = "Активен", InstructorId = instructors[0].InstructorId }
            };
            context.Courses.AddRange(courses);
            context.SaveChanges();

            var students = new Student[]
            {
                new Student { FullName = "Алексей Смирнов", Email = "alex.smirnov@example.com", RegistrationDate = DateOnly.FromDateTime(DateTime.Parse("2023-01-15")) },
                new Student { FullName = "Елена Кузнецова", Email = "elena.kuznetsova@example.com", RegistrationDate = DateOnly.FromDateTime(DateTime.Parse("2023-02-20")) },
                new Student { FullName = "Дмитрий Попов", Email = "dmitry.popov@example.com", RegistrationDate = DateOnly.FromDateTime(DateTime.Parse("2023-03-10")) }
            };
            context.Students.AddRange(students);
            context.SaveChanges();

            var enrollments = new Enrollment[]
            {
                new Enrollment { StudentId = students[0].StudentId, CourseId = courses[0].CourseId, Progress = 50.00m, StartDate = DateOnly.FromDateTime(DateTime.Parse("2023-09-01")) },
                new Enrollment { StudentId = students[0].StudentId, CourseId = courses[1].CourseId, Progress = 25.50m, StartDate = DateOnly.FromDateTime(DateTime.Parse("2023-09-15")) },
                new Enrollment { StudentId = students[1].StudentId, CourseId = courses[0].CourseId, Progress = 100.00m, StartDate = DateOnly.FromDateTime(DateTime.Parse("2023-08-01")), EndDate = DateOnly.FromDateTime(DateTime.Parse("2023-09-30")) },
                new Enrollment { StudentId = students[2].StudentId, CourseId = courses[3].CourseId, Progress = 10.00m, StartDate = DateOnly.FromDateTime(DateTime.Parse("2023-10-01")) }
            };
            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();

            var modules = new Module[]
            {
                new Module { CourseId = courses[0].CourseId, Title = "Основы C#: Переменные и типы", OrderNumber = 1 },
                new Module { CourseId = courses[0].CourseId, Title = "Основы C#: Управляющие конструкции", OrderNumber = 2 }
            };
            context.Modules.AddRange(modules);
            context.SaveChanges();

            var tests = new Test[]
            {
                new Test { ModuleId = modules[0].ModuleId, Title = "Тест по переменным" },
                new Test { ModuleId = modules[1].ModuleId, Title = "Тест по циклам и условиям" }
            };
            context.Tests.AddRange(tests);
            context.SaveChanges();

            var testResults = new TestResult[]
            {
                new TestResult { StudentId = students[0].StudentId, TestId = tests[0].TestId, Score = 95, CompletionDate = DateOnly.FromDateTime(DateTime.Parse("2023-09-10")) },
                new TestResult { StudentId = students[1].StudentId, TestId = tests[0].TestId, Score = 88, CompletionDate = DateOnly.FromDateTime(DateTime.Parse("2023-08-15")) },
                new TestResult { StudentId = students[1].StudentId, TestId = tests[1].TestId, Score = 92, CompletionDate = DateOnly.FromDateTime(DateTime.Parse("2023-09-05")) }
            };
            context.TestResults.AddRange(testResults);
            context.SaveChanges();
        }
    }
}