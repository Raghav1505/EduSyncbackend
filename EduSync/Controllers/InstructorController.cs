using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduSync.Data;
using EduSync.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Instructor")]
    public class InstructorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public InstructorController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/instructor/{id}/courses
        [HttpGet("{id}/courses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesForInstructor(Guid id)
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.InstructorId == id)
                .ToListAsync();

            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        // GET: api/instructor/{id}/results
        [HttpGet("{id}/results")]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetResultsForInstructor(Guid id)
        {
            var results = await (
                from r in _context.Results.AsNoTracking()
                join a in _context.Assessments.AsNoTracking()
                    on r.AssessmentId equals a.AssessmentId
                where a.Course!.InstructorId == id
                select r
            ).ToListAsync();

            return Ok(_mapper.Map<List<ResultDto>>(results));
        }
    }
}

