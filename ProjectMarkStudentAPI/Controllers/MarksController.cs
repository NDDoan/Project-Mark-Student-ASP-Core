using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using ProjectMarkStudentAPI.Constants;
using ProjectMarkStudentAPI.DTOs;
using ProjectMarkStudentAPI.Models;
using System.Security.Claims;

namespace ProjectMarkStudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MarksController : ControllerBase
    {
        private readonly ProjectStudentMarkContext _context;
        private readonly IMapper _mapper; // Issue #5: inject IMapper via constructor, not Service Locator

        public MarksController(ProjectStudentMarkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("course/{courseId}/student/{studentId}")]
        [EnableQuery]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Manager + "," + RoleNames.Teacher)]
        public async Task<ActionResult<IEnumerable<MarkDTO>>> GetStudentMarksForCourse(int courseId, int studentId)
        {
            var marks = await _context.Marks
                .AsNoTracking() // Issue #13: read-only query
                .Include(m => m.GradeItem)
                .Where(m => m.CourseId == courseId && m.StudentId == studentId)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<MarkDTO>>(marks));
        }

        [HttpPost("entry")]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
        public async Task<IActionResult> EnterMarks([FromBody] MarkEntryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            foreach (var kvp in dto.StudentMarks)
            {
                var studentId = kvp.Key;
                var value = kvp.Value;

                var existingMark = await _context.Marks
                    .FirstOrDefaultAsync(m => m.CourseId == dto.CourseId && m.GradeItemId == dto.GradeItemId && m.StudentId == studentId);

                if (existingMark != null)
                {
                    existingMark.Value = value;
                }
                else
                {
                    _context.Marks.Add(new Mark
                    {
                        CourseId = dto.CourseId,
                        GradeItemId = dto.GradeItemId,
                        StudentId = studentId,
                        Value = value
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("teacher/courses")]
        [EnableQuery]
        [Authorize(Roles = RoleNames.Teacher)]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetTeacherCourses()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var courses = await _context.Courses
                .AsNoTracking()
                .Include(c => c.Subject)
                .Include(c => c.Teacher)
                .Where(c => c.TeacherId == user.UserId)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<CourseDTO>>(courses));
        }
    }
}
