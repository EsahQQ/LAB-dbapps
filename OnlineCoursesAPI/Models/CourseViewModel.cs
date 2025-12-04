using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesAPI.Models
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Название курса обязательно")]
        [StringLength(200)]
        [Display(Name = "Название курса")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Выберите сложность")]
        [Display(Name = "Сложность")]
        public string Difficulty { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Выберите преподавателя")]
        [Display(Name = "Преподаватель")]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Выберите статус")]
        [Display(Name = "Статус")]
        public string Status { get; set; }

        public IEnumerable<SelectListItem>? InstructorOptions { get; set; }
    }
}