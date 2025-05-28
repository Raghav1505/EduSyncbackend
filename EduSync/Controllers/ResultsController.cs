using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduSync.Data;
using EduSync.Models;
using EduSync.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Security.Claims;

namespace EduSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ResultsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Results
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetResults()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID missing in token");

            var userId = Guid.Parse(userIdClaim);

            var results = await _context.Results
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var dtos = _mapper.Map<List<ResultDto>>(results);
            return Ok(dtos);
        }

        // GET: api/Results/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Student,Instructor")]
        public async Task<ActionResult<ResultDto>> GetResult(Guid id)
        {
            var result = await _context.Results
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null) return NotFound();

            var dto = _mapper.Map<ResultDto>(result);
            return Ok(dto);
        }

        // GET: api/Results/my
        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<StudentResultDto>>> GetMyDetailedResults()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID missing in token");

            var userId = Guid.Parse(userIdClaim);

            var results = await _context.Results
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var detailed = new List<StudentResultDto>(results.Count);

            foreach (var r in results)
            {
                var assessment = await _context.Assessments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AssessmentId == r.AssessmentId);
                if (assessment == null) continue;

                var questions = JsonConvert.DeserializeObject<Question[]>(
                    assessment.Questions ?? "[]")!;
                var answers = JsonConvert.DeserializeObject<StudentAnswer[]>(
                    r.Answers ?? "[]")!;

                var qDetails = new List<QuestionDetailDto>(questions.Length);
                for (int i = 0; i < questions.Length; i++)
                {
                    var q = questions[i];
                    var a = answers.FirstOrDefault(sa => sa.QuestionIndex == i);
                    qDetails.Add(new QuestionDetailDto
                    {
                        QuestionText = q.QuestionText,
                        Options = q.Options,
                        CorrectAnswer = q.CorrectAnswer,
                        SelectedAnswer = a?.SelectedOption ?? string.Empty
                    });
                }

                detailed.Add(new StudentResultDto
                {
                    ResultId = r.ResultId,
                    AssessmentId = r.AssessmentId!.Value,
                    Score = r.Score!.Value,
                    AttemptDate = r.AttemptDate!.Value,
                    Questions = qDetails
                });
            }

            return Ok(detailed);
        }

        // PUT: api/Results/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> PutResult(Guid id, [FromBody] UpdateResultDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _context.Results.FindAsync(id);
            if (result == null) return NotFound();

            _mapper.Map(dto, result);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!ResultExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Results/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteResult(Guid id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) return NotFound();

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResultExists(Guid id)
            => _context.Results.Any(e => e.ResultId == id);
    }
}
