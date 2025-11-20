using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;
using OnlineCoursesMVC.Controllers;
using OnlineCoursesMVC.Data;
using OnlineCoursesMVC.Infrastructure;
using OnlineCoursesMVC.Models;


namespace OnlineCoursesMVC.Tests.Controllers
{
    public class StudentsControllerTests
    {
        private readonly Mock<Db31048Context> _mockContext;
        private readonly StudentsController _controller;
        private readonly List<Student> _students;

        public StudentsControllerTests()
        {
            _students = new List<Student>();

            for (int i = 0; i < 100; i++)
            {
                _students.Add(new Student
                {
                    StudentId = i,
                    FullName = $"Тестовый Студент {i}",
                    Email = $"test{i}@test.com"
                });
            }
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

            int expectedCount = 25;
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
            int nonExistentId = 101;

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