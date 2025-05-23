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
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CoursesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Courses
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .ToListAsync();

            var dtos = _mapper.Map<List<CourseDto>>(courses);
            return Ok(dtos);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            var dto = _mapper.Map<CourseDto>(course);
            return Ok(dto);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> PutCourse(Guid id, [FromBody] UpdateCourseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _mapper.Map(dto, course);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CourseExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Courses
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<CourseDto>> PostCourse([FromBody] CreateCourseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var course = _mapper.Map<Course>(dto);
            course.CourseId = Guid.NewGuid();

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<CourseDto>(course);
            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, resultDto);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(Guid id)
            => _context.Courses.Any(e => e.CourseId == id);
    }

}
