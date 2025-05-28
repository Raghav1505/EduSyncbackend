namespace EduSync.Models
{
    /// <summary>
    /// Represents a single question in an assessment.
    /// </summary>
    public class Question
    {
        public string QuestionText { get; set; } = default!;
        public string[] Options { get; set; } = default!;
        public string CorrectAnswer { get; set; } = default!;
    }

    /// <summary>
    /// Represents a student’s answer to one question.
    /// </summary>
    public class StudentAnswer
    {
        /// <summary>
        /// The zero-based index of the question in the quiz.
        /// </summary>
        public int QuestionIndex { get; set; }

        /// <summary>
        /// The option text the student selected.
        /// </summary>
        public string SelectedOption { get; set; } = default!;
    }
}
