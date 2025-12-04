using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineCoursesAPI.Models; 

public class TestResultViewModel
{
    public long TestResultId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } 
    public int TestId { get; set; }
    public string TestTitle { get; set; }
    public int Score { get; set; }
    public DateOnly CompletionDate { get; set; }
}