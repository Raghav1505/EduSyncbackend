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

namespace EduSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AssessmentsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Assessments
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> GetAssessments()
        {
            var assessments = await _context.Assessments
                .AsNoTracking()
                .ToListAsync();

            var dtos = _mapper.Map<List<AssessmentDto>>(assessments);
            return Ok(dtos);
        }

        // GET: api/Assessments/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<AssessmentDto>> GetAssessment(Guid id)
        {
            var assessment = await _context.Assessments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
                return NotFound();

            var dto = _mapper.Map<AssessmentDto>(assessment);
            return Ok(dto);
        }

        [HttpGet("by-course/{courseId}")]
        [AllowAnonymous]  // Allows students (or even unauthenticated users) to access
        public async Task<ActionResult<IEnumerable<AssessmentDto>>> GetAssessmentsByCourse(Guid courseId)
        {
            var assessments = await _context.Assessments
                .Where(a => a.CourseId == courseId)
                .ToListAsync();

            var dtos = _mapper.Map<List<AssessmentDto>>(assessments);
            return Ok(dtos);
        }

        // PUT: api/Assessments/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> PutAssessment(Guid id, [FromBody] UpdateAssessmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
                return NotFound();

            _mapper.Map(dto, assessment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AssessmentExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Assessments
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<AssessmentDto>> PostAssessment([FromBody] CreateAssessmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assessment = _mapper.Map<Assessment>(dto);
            assessment.AssessmentId = Guid.NewGuid();

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<AssessmentDto>(assessment);
            return CreatedAtAction(nameof(GetAssessment), new { id = assessment.AssessmentId }, resultDto);
        }

        // DELETE: api/Assessments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteAssessment(Guid id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
                return NotFound();

            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssessmentExists(Guid id)
            => _context.Assessments.Any(e => e.AssessmentId == id);
    }
}