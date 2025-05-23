
using System;
using System.ComponentModel.DataAnnotations;

namespace EduSync.DTOs
{
    public class CourseDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Guid? InstructorId { get; set; }
        public string MediaUrl { get; set; } = default!;
    }

    public class CreateCourseDto
    {
        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        [Required]
        public Guid InstructorId { get; set; }

        [Required]
        public string MediaUrl { get; set; } = default!;
    }

    public class UpdateCourseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? InstructorId { get; set; }
        public string? MediaUrl { get; set; }
    }
}
