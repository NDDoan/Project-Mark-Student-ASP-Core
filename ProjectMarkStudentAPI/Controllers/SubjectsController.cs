using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using ProjectMarkStudentAPI.DTOs;
using ProjectMarkStudentAPI.Models;

namespace ProjectMarkStudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ProjectStudentMarkContext _context;
        private readonly IMapper _mapper;

        public SubjectsController(ProjectStudentMarkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<SubjectDTO>>> GetSubjects()
        {
            var subjects = await _context.Subjects.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<SubjectDTO>>(subjects));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDTO>> GetSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<SubjectDTO>(subject));
        }

        [HttpPost]
        public async Task<ActionResult<SubjectDTO>> PostSubject(SubjectDTO subjectDto)
        {
            if (await _context.Subjects.AnyAsync(s => s.SubjectName == subjectDto.SubjectName))
            {
                return BadRequest("Subject name already exists.");
            }

            var subject = _mapper.Map<Subject>(subjectDto);
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubject), new { id = subject.SubjectId }, _mapper.Map<SubjectDTO>(subject));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(int id, SubjectDTO subjectDto)
        {
            if (id != subjectDto.SubjectId) return BadRequest();

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            _mapper.Map(subjectDto, subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
