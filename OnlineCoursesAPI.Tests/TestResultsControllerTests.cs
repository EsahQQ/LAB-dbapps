using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;
using OnlineCoursesAPI.Controllers;
using OnlineCoursesAPI.Data;
using OnlineCoursesAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineCoursesAPI.Tests.Controllers
{
    public class TestResultsControllerTests
    {
        private readonly Mock<Db31048Context> _mockContext;
        private readonly TestResultsController _controller;
        private readonly List<TestResult> _testResults;
        private readonly List<Student> _students;
        private readonly List<Test> _tests;

        public TestResultsControllerTests()
        {
            _students = new List<Student> { new Student { StudentId = 1, FullName = "Тест Студент" } };
            _tests = new List<Test> { new Test { TestId = 1, Title = "Тест Тестовый" } };
            _testResults = new List<TestResult>
            {
                new TestResult { TestResultId = 1, StudentId = 1, TestId = 1, Score = 90, Student = _students[0], Test = _tests[0] }
            };

            _mockContext = new Mock<Db31048Context>();
            _mockContext.Setup(c => c.TestResults).ReturnsDbSet(_testResults);
            _mockContext.Setup(c => c.Students).ReturnsDbSet(_students);
            _mockContext.Setup(c => c.Tests).ReturnsDbSet(_tests);

            _controller = new TestResultsController(_mockContext.Object);
        }

        [Fact]
        public async Task GetTestResults_ReturnsListOfViewModels()
        {
            // Act
            var result = await _controller.GetTestResults();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TestResultViewModel>>>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TestResultViewModel>>(actionResult.Value);
            Assert.Single(model); 
            Assert.Equal("Тест Студент", model.First().StudentName);
        }

        [Fact]
        public async Task GetTestResult_ReturnsNotFound_ForInvalidId()
        {
            // Act
            var result = await _controller.GetTestResult(100000);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostTestResult_CreatesNewResult_AndReturnsCreatedAtAction()
        {
            // Arrange
            var newResultDto = new TestResultCreateDto
            {
                StudentId = 1,
                TestId = 1,
                Score = 85,
                CompletionDate = DateOnly.FromDateTime(System.DateTime.Now)
            };

            // Act
            var result = await _controller.PostTestResult(newResultDto);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var model = Assert.IsType<TestResult>(actionResult.Value);
            Assert.Equal(85, model.Score);

            _mockContext.Verify(c => c.TestResults.Add(It.IsAny<TestResult>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task GetTestResult_ReturnsCorrectViewModel_ForValidId() 
        {
            // Arrange
            long validId = 1;

            // Act
            var result = await _controller.GetTestResult(validId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TestResultViewModel>>(result);
            var model = Assert.IsType<TestResultViewModel>(actionResult.Value);

            Assert.Equal(validId, model.TestResultId);
            Assert.Equal("Тест Студент", model.StudentName); 
            Assert.Equal("Тест Тестовый", model.TestTitle); 
        }

        [Fact]
        public async Task PutTestResult_ReturnsNoContent_ForValidUpdate()
        {
            // Arrange
            int existingId = 1;

            var updateDto = new TestResultUpdateDto
            {
                TestResultId = existingId,
                StudentId = 1,
                TestId = 1,
                Score = 99, 
                CompletionDate = DateOnly.FromDateTime(System.DateTime.Now)
            };

            _mockContext.Setup(c => c.TestResults.FindAsync(It.IsAny<object[]>()))
                        .ReturnsAsync((object[] ids) => _testResults.FirstOrDefault(tr => tr.TestResultId == (long)ids[0]));
            // Act
            var result = await _controller.PutTestResult(existingId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task DeleteTestResult_ReturnsNoContent_ForValidId()
        {
            // Arrange
            long existingId = 1;

            _mockContext.Setup(c => c.TestResults.FindAsync(It.IsAny<object[]>()))
                        .ReturnsAsync((object[] ids) => _testResults.FirstOrDefault(tr => tr.TestResultId == (long)ids[0]));

            // Act
            var result = await _controller.DeleteTestResult(existingId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockContext.Verify(c => c.TestResults.Remove(It.IsAny<TestResult>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }
    }
}