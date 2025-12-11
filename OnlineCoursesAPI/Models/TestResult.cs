using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineCoursesAPI.Models;

public partial class TestResult
{
    [Column("ResultID")]
    public long TestResultId { get; set; }

    public int StudentId { get; set; }

    public int TestId { get; set; }

    public int Score { get; set; }

    /// <summary>
    /// Стандартный JSON-сериализатор (System.Text.Json), поддерживает типы DateOnly и TimeOnly по умолчанию.
    /// Он корректно сериализует их в стандартный формат ISO 8601 (YYYY-MM-DD), который совместим с JavaScript.
    /// Тестирование показало, что текущая реализация обмена данными с DateOnly работает стабильно и не требует написания кастомного JsonConverter
    /// </summary>
    public DateOnly CompletionDate { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
