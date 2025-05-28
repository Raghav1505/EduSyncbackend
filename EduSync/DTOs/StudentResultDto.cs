using System;
using System.Collections.Generic;

namespace EduSync.DTOs
{
    public class QuestionDetailDto
    {
        public string QuestionText { get; set; } = default!;
        public IEnumerable<string> Options { get; set; } = default!;
        public string CorrectAnswer { get; set; } = default!;
        public string SelectedAnswer { get; set; } = default!;
    }

    public class StudentResultDto
    {
        public Guid ResultId { get; set; }
        public Guid AssessmentId { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; }

        // Side-by-side view of each question
        public List<QuestionDetailDto> Questions { get; set; } = new();
    }
}
