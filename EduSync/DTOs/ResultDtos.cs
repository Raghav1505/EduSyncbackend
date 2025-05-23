// DTOs/ResultDtos.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace EduSync.DTOs
{
    /// <summary>
    /// Returned to clients when fetching results.
    /// </summary>
    public class ResultDto
    {
        public Guid ResultId { get; set; }
        public Guid? AssessmentId { get; set; }
        public Guid? UserId { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; }
    }

    /// <summary>
    /// Used when creating a new result (POST).
    /// </summary>
    public class CreateResultDto
    {
        [Required]
        public Guid AssessmentId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Score { get; set; }

        [Required]
        public DateTime AttemptDate { get; set; }
    }

    /// <summary>
    /// Used when updating an existing result (PATCH/PUT).
    /// All properties are optional so you can update one or more fields.
    /// </summary>
    public class UpdateResultDto
    {
        public Guid? AssessmentId { get; set; }

        public Guid? UserId { get; set; }

        [Range(0, int.MaxValue)]
        public int? Score { get; set; }

        public DateTime? AttemptDate { get; set; }
    }
}
