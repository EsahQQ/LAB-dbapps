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
    public class InstructorsControllerTests
    {
        private readonly Mock<Db31048Context> _mockContext;
        private readonly InstructorsController _controller;
        private readonly List<Instructor> _instructors;

        public InstructorsControllerTests()
        {
            _instructors = new List<Instructor>();

            for (int i = 0; i < 100; i++)
            {
                _instructors.Add(new Instructor
                {
                    InstructorId = i,
                    FullName = $"Тестовый Преподаватель {i}"
                });
            }

            _mockContext = new Mock<Db31048Context>();

            _mockContext.Setup(c => c.Instructors).ReturnsDbSet(_instructors);

            var mockSession = new Mock<ISession>();

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            _controller = new InstructorsController(_mockContext.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithCorrectTotalCount()
        {
            var result = await _controller.Index(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Instructor>>(viewResult.ViewData.Model);
            Assert.Equal(_instructors.Count, model.TotalCount);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenIdIsNull()
        {
            var result = await _controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenInstructorNotFound()
        {
            int nonExistentId = 101;

            var result = await _controller.Details(nonExistentId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithCorrectInstructor()
        {
            int instructorId = 2;

            var result = await _controller.Details(instructorId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Instructor>(viewResult.ViewData.Model);
            Assert.Equal(instructorId, model.InstructorId);
            Assert.Equal("Тестовый Преподаватель 2", model.FullName);
        }
    }
}