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
            var results = await _context.Results
                .AsNoTracking()
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

        // POST: api/Results
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ResultDto>> PostResult([FromBody] CreateResultDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = _mapper.Map<Result>(dto);
            result.ResultId = Guid.NewGuid();

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<ResultDto>(result);
            return CreatedAtAction(nameof(GetResult), new { id = result.ResultId }, resultDto);
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
