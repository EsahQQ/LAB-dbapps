using Microsoft.AspNetCore.Http;
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

        public StudentsControllerTests()
        {
            _students = new List<Student>
        {
            new Student { StudentId = 1, FullName = "Тестовый Студент 1", Email = "test1@test.com" },
            new Student { StudentId = 2, FullName = "Тестовый Студент 2", Email = "test2@test.com" },
            new Student { StudentId = 3, FullName = "Тестовый Студент 3", Email = "test3@test.com" }
        };

            _mockContext = new Mock<Db31048Context>();

            _mockContext.Setup(c => c.Students).ReturnsDbSet(_students);

            var mockSession = new Mock<ISession>();

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            _controller = new StudentsController(_mockContext.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfStudents()
        {
            var result = await _controller.Index(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Student>>(viewResult.ViewData.Model);

            int expectedCount = 2;
            Assert.Equal(expectedCount, model.Count);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenIdIsNull()
        {
            int? id = null;

            var result = await _controller.Details(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenStudentNotFound()
        {
            int nonExistentId = 99;

            var result = await _controller.Details(nonExistentId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithCorrectStudent()
        {
            int studentId = 2;

            var result = await _controller.Details(studentId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<Student>(viewResult.ViewData.Model);

            Assert.Equal(studentId, model.StudentId);
            Assert.Equal("Тестовый Студент 2", model.FullName);
        }
    }
}