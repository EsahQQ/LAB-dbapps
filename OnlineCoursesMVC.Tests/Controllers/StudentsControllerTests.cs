using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using OnlineCoursesMVC.Controllers;
using OnlineCoursesMVC.Data;
using OnlineCoursesMVC.Infrastructure;
using OnlineCoursesMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineCoursesMVC.Tests.Controllers
{
    public class StudentsControllerTests
    {
        private readonly Mock<Db31048Context> _mockContext;
        private readonly StudentsController _controller;
        private readonly List<Student> _students;

        // Конструктор теста - выполняется перед каждым тестом в классе
        public StudentsControllerTests()
        {
            // 1. Создаем тестовые данные (без изменений)
            _students = new List<Student>
        {
            new Student { StudentId = 1, FullName = "Тестовый Студент 1", Email = "test1@test.com" },
            new Student { StudentId = 2, FullName = "Тестовый Студент 2", Email = "test2@test.com" },
            new Student { StudentId = 3, FullName = "Тестовый Студент 3", Email = "test3@test.com" }
        };

            // 2. Создаем мок для DbContext
            _mockContext = new Mock<Db31048Context>();

            // --- ИЗМЕНЕНИЕ ЗДЕСЬ ---
            // 3. Настраиваем мок DbSet с помощью Moq.EntityFrameworkCore
            //    Эта одна строка заменяет всю сложную настройку и добавляет поддержку async!
            _mockContext.Setup(c => c.Students).ReturnsDbSet(_students);

            // 4. Создаем экземпляр контроллера (без изменений)
            _controller = new StudentsController(_mockContext.Object);
        }

        // Тест 1: Проверяем, что метод Index возвращает представление со списком студентов
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfStudents()
        {
            // Arrange (Подготовка)
            // Вся подготовка уже сделана в конструкторе

            // Act (Действие)
            var result = await _controller.Index(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Student>>(viewResult.ViewData.Model);

            // ИЗМЕНЕНО: Ожидаем количество, равное pageSize или меньше
            int expectedCount = 2; // Установите здесь ваш pageSize из контроллера
            Assert.Equal(expectedCount, model.Count);
        }

        // Тест 2: Проверяем, что метод Details возвращает NotFound, если передан null
        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Details(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Тест 3: Проверяем, что метод Details возвращает NotFound, если студент не найден
        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenStudentNotFound()
        {
            // Arrange
            int nonExistentId = 99; // ID, которого нет в наших тестовых данных

            // Act
            var result = await _controller.Details(nonExistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Тест 4: Проверяем, что метод Details возвращает ViewResult с правильным студентом
        [Fact]
        public async Task Details_ReturnsViewResult_WithCorrectStudent()
        {
            // Arrange
            int studentId = 2;

            // Act
            var result = await _controller.Details(studentId);

            // Assert
            // 1. Проверяем, что результат - это ViewResult
            var viewResult = Assert.IsType<ViewResult>(result);

            // 2. Проверяем, что модель - это Student
            var model = Assert.IsType<Student>(viewResult.ViewData.Model);

            // 3. Проверяем, что ID в модели совпадает с запрошенным ID
            Assert.Equal(studentId, model.StudentId);
            Assert.Equal("Тестовый Студент 2", model.FullName);
        }
    }
}