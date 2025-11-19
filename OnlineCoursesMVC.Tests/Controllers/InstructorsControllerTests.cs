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
    public class InstructorsControllerTests
    {
        private readonly Mock<Db31048Context> _mockContext;
        private readonly InstructorsController _controller;
        private readonly List<Instructor> _instructors;

        // Конструктор теста
        public InstructorsControllerTests()
        {
            // 1. Создаем тестовые данные
            _instructors = new List<Instructor>
            {
                new Instructor { InstructorId = 1, FullName = "Тестовый Преподаватель 1" },
                new Instructor { InstructorId = 2, FullName = "Тестовый Преподаватель 2" },
                new Instructor { InstructorId = 3, FullName = "Тестовый Преподаватель 3" }
            };

            // 2. Создаем мок для DbContext
            _mockContext = new Mock<Db31048Context>();

            // 3. Настраиваем мок DbSet<Instructor> с поддержкой async
            _mockContext.Setup(c => c.Instructors).ReturnsDbSet(_instructors);

            // 4. Создаем экземпляр контроллера
            _controller = new InstructorsController(_mockContext.Object);
        }

        // Тест 1: Проверяем, что Index возвращает представление с правильным количеством преподавателей
        [Fact]
        public async Task Index_ReturnsAViewResult_WithCorrectTotalCount()
        {
            // Act
            var result = await _controller.Index(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Instructor>>(viewResult.ViewData.Model);
            Assert.Equal(_instructors.Count, model.TotalCount);
        }

        // Тест 2: Проверяем, что Details возвращает NotFound, если передан null
        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Тест 3: Проверяем, что Details возвращает NotFound, если преподаватель не найден
        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenInstructorNotFound()
        {
            // Arrange
            int nonExistentId = 99;

            // Act
            var result = await _controller.Details(nonExistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Тест 4: Проверяем, что Details возвращает ViewResult с правильным преподавателем
        [Fact]
        public async Task Details_ReturnsViewResult_WithCorrectInstructor()
        {
            // Arrange
            int instructorId = 2;

            // Act
            var result = await _controller.Details(instructorId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Instructor>(viewResult.ViewData.Model);
            Assert.Equal(instructorId, model.InstructorId);
            Assert.Equal("Тестовый Преподаватель 2", model.FullName);
        }
    }
}