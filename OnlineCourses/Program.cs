using Azure;
using Microsoft.EntityFrameworkCore;
using OnlineCourses.Data;
using OnlineCourses.Models;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCourses;
public class Program
{
    public static void Main(string[] args)
    {
        using (Db28021Context db = new Db28021Context())
        {
            Console.WriteLine("Нажите любую клавишу...");
            Console.Read();
            SelectAllStudents(db);
            Console.WriteLine("Нажите любую клавишу...");
            Console.Read();
            SelectActiveCourses(db);
            Console.WriteLine("Нажите любую клавишу...");
            Console.Read();
            SelectTestCountPerStudent(db);
            Console.WriteLine("Нажите любую клавишу...");
            Console.Read();
            SelectCourseAndInstructorNames(db);
            Console.WriteLine("Нажите любую клавишу...");
            Console.Read();
            SelectHighScoringResultsWithStudentNames(db);
            Console.WriteLine("Нажите любую клавишу...");
            Console.Read();
            InsertStudentAndEnrollment(db);
            Console.WriteLine("Нажите любую клавишу...");
            Console.Read();
            DeleteStudentAndEnrollment(db);
            Console.WriteLine("Нажите любую клавишу...");
            Console.Read();
            UpdateEnrollment(db);
        }
        Console.Read();
    }

    static void Print(string sqltext, IEnumerable items)
    {
        Console.WriteLine(sqltext);
        Console.WriteLine("Записи: ");
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
    }


    static void SelectAllStudents(Db28021Context db) //2.1
    {
        string comment = "2.1. Выборка всех данных из таблицы «один» (Students):";

        var allStudents = db.Students
            .Select(s => new { s.StudentId, s.FullName, s.Email, s.RegistrationDate })
            .ToList();

        Print(comment, allStudents.Take(5));
    } 

    static void SelectActiveCourses(Db28021Context db) //2.2
    {
        string comment = "2.2. Выборка данных из таблицы «один» с фильтром (Активные курсы):";

        var activeCourses = db.Courses
            .Where(c => c.Status == "Активен")
            .Select(c => new { c.CourseId, c.Title, c.Status })
            .ToList();

        Print(comment, activeCourses.Take(5));
    }

    static void SelectTestCountPerStudent(Db28021Context db) //2.3
    {
        string comment = "2.3. Группировка данных с итоговым результатом (Количество тестов на студента):";

        var testCounts = db.TestResults
            .GroupBy(tr => tr.StudentId)
            .Select(g => new
            {
                Студент_ID = g.Key,
                Количество_тестов = g.Count()
            })
            .OrderByDescending(x => x.Количество_тестов)
            .ToList();

        Print(comment, testCounts.Take(5));
    }

    static void SelectCourseAndInstructorNames(Db28021Context db) //2.4
    {
        string comment = "2.4. Выборка из двух связанных таблиц (Курс и Преподаватель):";

        var courseInstructors = db.Courses
            .Include(c => c.Instructor)
            .Select(c => new
            {
                Курс = c.Title,
                Преподаватель = c.Instructor.FullName
            })
            .ToList();

        Print(comment, courseInstructors.Take(5));
    }

    static void SelectHighScoringResultsWithStudentNames(Db28021Context db) //2.5
    {
        string comment = "2.5. Выборка из двух таблиц с фильтром (Студенты с баллом 95+):";

        var highScores = db.TestResults
            .Include(tr => tr.Student)
            .Where(tr => tr.Score >= 95)
            .Select(tr => new
            {
                Студент = tr.Student.FullName,
                Балл = tr.Score,
                Дата = tr.CompletionDate
            })
            .ToList();

        Print(comment, highScores.Take(5));
    }

    static void InsertStudentAndEnrollment(Db28021Context db)// 2.6; 2.7
    {
        Console.WriteLine("2.6 Вставка данных в таблицу Students...");
        Student student = new()
        {
            FullName = "Александр Пушкин",
            Email = "alex.pushka@gstu.com",
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now.Date)
        };

        db.Students.Add(student);

        db.SaveChanges();
        Console.WriteLine($"Создан студент (ID: {student.StudentId})");

        Console.WriteLine("2.7 Вставка данных в таблицу Enrollments...");
        var courseToEnroll = db.Courses.FirstOrDefault(c => c.Status == "Активен");
        Enrollment enrollment = new()
        {
            StudentId = student.StudentId, 
            CourseId = courseToEnroll.CourseId,  
            Progress = 0.00m,
            StartDate = DateOnly.FromDateTime(DateTime.Now.Date)
        };

        db.Enrollments.Add(enrollment);

        db.SaveChanges();
        Console.WriteLine("Студент успешно записан на курс.");
    }

    static void DeleteStudentAndEnrollment(Db28021Context db)// 2.8; 2.9
    {
        Console.WriteLine("2.8 Удаление данных из таблицы Enrollments...");
        string studentEmailToDelete = "alex.pushka@gstu.com";
        IQueryable<Student> studentsToDelete = db.Students.Where(s => s.Email == studentEmailToDelete);

        IQueryable<Enrollment> enrollmentsToDelete = db.Enrollments
            .Include(e => e.Student) 
            .Where(e => e.Student.Email == studentEmailToDelete);

        if (enrollmentsToDelete.Any())
        {
            db.Enrollments.RemoveRange(enrollmentsToDelete);
            Console.WriteLine("Найденные записи на курс помечены к удалению.");
        }

        db.SaveChanges();

        Console.WriteLine("2.9 Удаление данных из таблицы Students...");
        if (studentsToDelete.Any())
        {
            db.Students.RemoveRange(studentsToDelete);
            Console.WriteLine("Найденные студенты помечены к удалению.");
        }

        db.SaveChanges();

        Console.WriteLine("Удаление завершено.");
    }

    static void UpdateEnrollment(Db28021Context db)// 2.10
    {
        Console.WriteLine("2.10 Обновление данных в таблице Enrollment...");
        Course courseToUpdate = db.Courses
            .Where(c => c.CourseId == 1)
            .FirstOrDefault();


        if (courseToUpdate != null)
        {
            courseToUpdate.Title = "Курс анекдотов";
            Console.WriteLine("Курс помечен к обновлению.");
        }

        db.SaveChanges();
        Console.WriteLine("Все изменения сохранены в базе данных.");
    }
}



