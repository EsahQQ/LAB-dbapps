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
    public class CoursesControllerTests
    {
        private readonly Mock<Db31048Context> _mockContext;
        private readonly CoursesController _controller;
        private readonly List<Course> _courses;
        private readonly List<Instructor> _instructors;

        public CoursesControllerTests()
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

            _courses = new List<Course>();

            for (int i = 0; i < 100; i++)
            {
                _courses.Add(new Course
                {
                    CourseId = i,
                    Title = $"Курс {i}",
                    InstructorId = i,
                    Instructor = _instructors[i]
                });
            }

            _mockContext = new Mock<Db31048Context>();

            _mockContext.Setup(c => c.Courses).ReturnsDbSet(_courses);
            _mockContext.Setup(c => c.Instructors).ReturnsDbSet(_instructors);

            var mockSession = new Mock<ISession>();

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            _controller = new CoursesController(_mockContext.Object);

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
            var model = Assert.IsAssignableFrom<PaginatedList<Course>>(viewResult.ViewData.Model);
            Assert.Equal(_courses.Count, model.TotalCount);
        }

        [Fact]
        public async Task Index_ReturnsFilteredResults_WhenSearchStringIsProvided()
        {
            string searchString = "Курс 1"; 

            var result = await _controller.Index(searchString, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Course>>(viewResult.ViewData.Model);
            Assert.Equal(searchString, model[0].Title);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithCorrectCourse()
        {
            int courseId = 2;

            var result = await _controller.Details(courseId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Course>(viewResult.ViewData.Model);
            Assert.Equal(courseId, model.CourseId);
            Assert.NotNull(model.Instructor); 
            Assert.Equal("Тестовый Преподаватель 2", model.Instructor.FullName);
        }

        [Fact]
        public void Create_GET_ReturnsViewResult_WithViewModel()
        {
            var result = _controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CourseViewModel>(viewResult.ViewData.Model);

            Assert.NotNull(model.InstructorOptions); 
            Assert.Equal(_instructors.Count, model.InstructorOptions.Count());
        }
    }
}