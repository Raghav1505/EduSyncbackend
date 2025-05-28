using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduSync.Data;
using EduSync.Models;
using EduSync.DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EduSync.Controllers
{
    [ApiController]
    [Route("api/attempt")]
    [Authorize(Roles = "Student")]
    public class AssessmentAttemptController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssessmentAttemptController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/attempt/{assessmentId}
        [HttpPost("{assessmentId}")]
        public async Task<IActionResult> AttemptAssessment(Guid assessmentId, [FromBody] StudentAnswer[] answers)
        {
            // 1. Validate assessment exists
            var assessment = await _context.Assessments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AssessmentId == assessmentId);

            if (assessment == null)
                return NotFound("Assessment not found");

            // 2. Deserialize questions
            var questions = JsonConvert.DeserializeObject<Question[]>(assessment.Questions ?? "[]");
            if (questions == null || questions.Length == 0)
                return BadRequest("Assessment contains no questions");

            // 3. Auto-grade
            int rawScore = 0;
            for (int i = 0; i < questions.Length && i < answers.Length; i++)
            {
                if (answers[i].SelectedOption == questions[i].CorrectAnswer)
                    rawScore++;
            }

            int maxScore = assessment.MaxScore ?? 0;
            int finalScore = (int)Math.Round(
                rawScore * maxScore / (double)questions.Length
            );

            // 4. Get Student ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID missing in token");

            var studentId = Guid.Parse(userIdClaim);

            // 5. Save Result
            var result = new Result
            {
                ResultId = Guid.NewGuid(),
                AssessmentId = assessmentId,
                UserId = studentId,
                Score = finalScore,
                Answers = JsonConvert.SerializeObject(answers),
                AttemptDate = DateTime.UtcNow
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            // 6. Return success
            return Ok(new
            {
                message = "Assessment submitted",
                score = finalScore,
                total = maxScore,
                resultId = result.ResultId
            });
        }
    }
}
