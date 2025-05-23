
using System;
using System.ComponentModel.DataAnnotations;

namespace EduSync.DTOs
{
    // Returned to the client
    public class AssessmentDto
    {
        public Guid AssessmentId { get; set; }
        public Guid? CourseId { get; set; }
        public string Title { get; set; } = default!;
        public string Questions { get; set; } = default!;
        public int MaxScore { get; set; }
    }

    // Used when creating a new assessment
    public class CreateAssessmentDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = default!;

        [Required]
        public string Questions { get; set; } = default!;

        [Required, Range(1, int.MaxValue)]
        public int MaxScore { get; set; }
    }

    // Used when updating an assessment (PATCH/PUT)
    public class UpdateAssessmentDto
    {
        [StringLength(100)]
        public string? Title { get; set; }

        public string? Questions { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxScore { get; set; }

        public Guid? CourseId { get; set; }
    }
}
