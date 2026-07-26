using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMarkStudentAPI.DTOs;
using ProjectMarkStudentAPI.Models;

namespace ProjectMarkStudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GradeItemsController : ControllerBase
    {
        private readonly ProjectStudentMarkContext _context;
        private readonly IMapper _mapper;

        public GradeItemsController(ProjectStudentMarkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("subject/{subjectId}")]
        public async Task<ActionResult<IEnumerable<GradeItemDTO>>> GetBySubject(int subjectId)
        {
            var items = await _context.GradeItems.AsNoTracking() // Issue #13: AsNoTracking for read-only
                .Where(g => g.SubjectId == subjectId).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<GradeItemDTO>>(items));
        }

        // Issue #8: Added GET /{id} so that CreatedAtAction in PostGradeItem works
        [HttpGet("{id}")]
        public async Task<ActionResult<GradeItemDTO>> GetGradeItem(int id)
        {
            var gradeItem = await _context.GradeItems.AsNoTracking().FirstOrDefaultAsync(g => g.GradeItemId == id);
            if (gradeItem == null) return NotFound();
            return Ok(_mapper.Map<GradeItemDTO>(gradeItem));
        }

        // Issue #6: Returns 201 Created with Location header instead of 200 OK
        [HttpPost]
        public async Task<ActionResult<GradeItemDTO>> PostGradeItem(GradeItemDTO dto)
        {
            var gradeItem = _mapper.Map<GradeItem>(dto);
            _context.GradeItems.Add(gradeItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGradeItem), new { id = gradeItem.GradeItemId }, _mapper.Map<GradeItemDTO>(gradeItem));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGradeItem(int id, GradeItemDTO dto)
        {
            if (id != dto.GradeItemId) return BadRequest();

            var gradeItem = await _context.GradeItems.FindAsync(id);
            if (gradeItem == null) return NotFound();

            _mapper.Map(dto, gradeItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGradeItem(int id)
        {
            var gradeItem = await _context.GradeItems.FindAsync(id);
            if (gradeItem == null) return NotFound();

            _context.GradeItems.Remove(gradeItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
