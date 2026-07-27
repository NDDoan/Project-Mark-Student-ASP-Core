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
    public class CoursesController : ControllerBase
    {
        private readonly ProjectStudentMarkContext _context;
        private readonly IMapper _mapper;

        public CoursesController(ProjectStudentMarkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCourses()
        {
            var courses = await _context.Courses.AsNoTracking() // Issue #13
                .Include(c => c.Subject).Include(c => c.Teacher).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CourseDTO>>(courses));
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<ActionResult<CourseDTO>> GetCourse(int id)
        {
            var course = await _context.Courses.AsNoTracking() // Issue #13
                .Include(c => c.Subject).Include(c => c.Teacher).FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null) return NotFound();
            return Ok(_mapper.Map<CourseDTO>(course));
        }

        [HttpPost]
        public async Task<ActionResult<CourseDTO>> PostCourse(CourseDTO courseDto)
        {
            var course = _mapper.Map<Course>(courseDto);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var createdCourse = await _context.Courses.Include(c => c.Subject).Include(c => c.Teacher).FirstOrDefaultAsync(c => c.CourseId == course.CourseId);
            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, _mapper.Map<CourseDTO>(createdCourse));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, CourseDTO courseDto)
        {
            if (id != courseDto.CourseId) return BadRequest();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.Title = courseDto.Title;
            course.SubjectId = courseDto.SubjectId;
            course.TeacherId = courseDto.TeacherId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/students")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudentsInCourse(int id)
        {
            var students = await _context.StudentCourses
                .AsNoTracking() // Issue #13
                .Where(sc => sc.CourseId == id)
                .Select(sc => sc.Student)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<StudentDTO>>(students));
        }

        // Issue #7: Returns 201 Created instead of 200 OK for POST operations
        [HttpPost("{id}/students")]
        public async Task<IActionResult> AssignStudentToCourse(int id, [FromBody] StudentAssignmentDTO dto)
        {
            if (id != dto.CourseId) return BadRequest();

            var exists = await _context.StudentCourses.AnyAsync(sc => sc.CourseId == dto.CourseId && sc.StudentId == dto.StudentId);
            if (exists) return BadRequest("Student already assigned to this course.");

            var sc = new StudentCourse { CourseId = dto.CourseId, StudentId = dto.StudentId };
            _context.StudentCourses.Add(sc);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudentsInCourse), new { id = dto.CourseId }, dto);
        }

        [HttpDelete("{courseId}/students/{studentId}")]
        public async Task<IActionResult> RemoveStudentFromCourse(int courseId, int studentId)
        {
            var sc = await _context.StudentCourses.FirstOrDefaultAsync(s => s.CourseId == courseId && s.StudentId == studentId);
            if (sc == null) return NotFound();

            _context.StudentCourses.Remove(sc);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
