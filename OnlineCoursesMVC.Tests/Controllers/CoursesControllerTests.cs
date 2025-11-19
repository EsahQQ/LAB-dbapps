using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public class CoursesControllerTests
    {
        private readonly Mock<Db31048Context> _mockContext;
        private readonly CoursesController _controller;
        private readonly List<Course> _courses;
        private readonly List<Instructor> _instructors;

        public CoursesControllerTests()
        {
 
            _instructors = new List<Instructor>
            {
                new Instructor { InstructorId = 1, FullName = "Тестовый Преподаватель 1" },
                new Instructor { InstructorId = 2, FullName = "Тестовый Преподаватель 2" }
            };

            _courses = new List<Course>
            {
                new Course { CourseId = 1, Title = "Курс 1", InstructorId = 1, Instructor = _instructors[0] },
                new Course { CourseId = 2, Title = "Курс 2", InstructorId = 2, Instructor = _instructors[1] },
                new Course { CourseId = 3, Title = "Курс 3", InstructorId = 1, Instructor = _instructors[0] }
            };

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
            string searchString = "Курс 2"; 

            var result = await _controller.Index(searchString, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<Course>>(viewResult.ViewData.Model);
            Assert.Single(model); 
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