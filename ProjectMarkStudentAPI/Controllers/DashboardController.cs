using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using ProjectMarkStudentAPI.Models;

namespace ProjectMarkStudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ProjectStudentMarkContext _context;

        public DashboardController(ProjectStudentMarkContext context)
        {
            _context = context;
        }

        [HttpGet("average-scores")]
        [EnableQuery]
        public async Task<IActionResult> GetAverageScorePerSubject()
        {
            // Fetch all needed data to client first to avoid EF Core translation issues
            var subjects = await _context.Subjects.ToListAsync();

            var marks = await _context.Marks
                .Include(m => m.GradeItem)
                .ToListAsync();

            var result = subjects.Select(s =>
            {
                // Group marks of this subject by student, sum weighted score per student
                var perStudentScores = marks
                    .Where(m => m.GradeItem.SubjectId == s.SubjectId)
                    .GroupBy(m => m.StudentId)
                    .Select(g => g.Sum(m => (double)m.Value * (double)m.GradeItem.Rate / 100.0))
                    .ToList();

                return new
                {
                    SubjectName = s.SubjectName,
                    AverageScore = perStudentScores.Count > 0
                        ? Math.Round(perStudentScores.Average(), 2)
                        : 0.0
                };
            }).ToList();

            return Ok(result);
        }

        [HttpGet("score-distribution")]
        [EnableQuery]
        public async Task<IActionResult> GetScoreDistribution()
        {
            var marks = await _context.Marks.Select(m => m.Value).ToListAsync();
            var buckets = new Dictionary<string, int>();
            
            for (int i = 0; i < 10; i++)
            {
                string key = $"{i}-{i + 1}";
                buckets[key] = marks.Count(v => v >= i && v < i + 1);
            }
            buckets["10"] = marks.Count(v => v == 10);
            
            return Ok(buckets);
        }

        [HttpGet("enrollments")]
        [EnableQuery]
        public async Task<IActionResult> GetEnrollmentPerSubject()
        {
            var result = await _context.Subjects
                .Select(s => new
                {
                    SubjectName = s.SubjectName,
                    StudentCount = _context.StudentCourses
                        .Count(sc => sc.Course.SubjectId == s.SubjectId)
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
