using System;

namespace OnlineCoursesAPI.Models
{
    public class TestResultUpdateDto
    {
        public long TestResultId { get; set; }
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public int Score { get; set; }
        public DateOnly CompletionDate { get; set; }
    }
}