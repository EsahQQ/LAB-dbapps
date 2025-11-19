using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using OnlineCoursesMVC.Controllers;
using OnlineCoursesMVC.Data;
using OnlineCoursesMVC.Infrastructure;
using OnlineCoursesMVC.Models;
using Xunit;

namespace OnlineCoursesMVC.Tests.Controllers
{
    public class CoursesControllerTests
    {
        private readonly Mock<Db31048Context> _mockContext;
        private readonly CoursesController _controller;
        private readonly List<Course> _courses;
        private readonly List<Instructor> _instructors;

        public CoursesControllerTests()
        {
            // 1. Создаем тестовые данные для Преподавателей
            _instructors = new List<Instructor>
            {
                new Instructor { InstructorId = 1, FullName = "Тестовый Преподаватель 1" },
                new Instructor { InstructorId = 2, FullName = "Тестовый Преподаватель 2" }
            };

            // 2. Создаем тестовые данные для Курсов, связывая их с преподавателями
            _courses = new List<Course>
            {
                new Course { CourseId = 1, Title = "Курс 1", InstructorId = 1, Instructor = _instructors[0] },
                new Course { CourseId = 2, Title = "Курс 2", InstructorId = 2, Instructor = _instructors[1] },
                new Course { CourseId = 3, Title = "Курс 3", InstructorId = 1, Instructor = _instructors[0] }
            };

            // 3. Создаем мок для DbContext
            _mockContext = new Mock<Db31048Context>();

            // 4. Настраиваем моки для DbSet<Course> и DbSet<Instructor>
            // Это позволит .Include() работать корректно
            _mockContext.Setup(c => c.Courses).ReturnsDbSet(_courses);
            _mockContext.Setup(c => c.Instructors).ReturnsDbSet(_instructors);

            // 5. Создаем экземпляр контроллера
            _controller = new CoursesController(_mockContext.Object);
        }

        // Тест 1: Проверяем, что Index возвращает правильное общее количество курсов
        [Fact]
        public async Task Index_ReturnsAViewResult_WithCorrectTotalCount()
        {
            // Arrange
            // (в конструкторе)

            // Act
            var result = await _controller.Index(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Course>>(viewResult.ViewData.Model);
            Assert.Equal(_courses.Count, model.TotalCount);
        }

        // Тест 2: Проверяем, что фильтрация в Index работает
        [Fact]
        public async Task Index_ReturnsFilteredResults_WhenSearchStringIsProvided()
        {
            // Arrange
            string searchString = "Курс 2"; // Ожидаем найти 1 результат

            // Act
            var result = await _controller.Index(searchString, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Course>>(viewResult.ViewData.Model);
            Assert.Single(model); // Проверяем, что в результате только один элемент
            Assert.Equal(searchString, model[0].Title);
        }

        // Тест 3: Проверяем Details для существующего курса
        [Fact]
        public async Task Details_ReturnsViewResult_WithCorrectCourse()
        {
            // Arrange
            int courseId = 2;

            // Act
            var result = await _controller.Details(courseId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Course>(viewResult.ViewData.Model);
            Assert.Equal(courseId, model.CourseId);
            Assert.NotNull(model.Instructor); // Проверяем, что Include сработал
            Assert.Equal("Тестовый Преподаватель 2", model.Instructor.FullName);
        }

        // Тест 4: Проверяем Create (GET)
        [Fact]
        public void Create_GET_ReturnsViewResult_WithViewModel()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CourseViewModel>(viewResult.ViewData.Model);
            // Проверяем, что выпадающий список преподавателей был заполнен
            Assert.NotNull(model.InstructorOptions); // <-- ИЗМЕНЕНИЕ ЗДЕСЬ
            Assert.Equal(_instructors.Count, model.InstructorOptions.Count());
        }

        // Дополнительно можно написать тесты для Create (POST), Edit, Delete,
        // но для лабораторной этого набора уже более чем достаточно.
    }
}