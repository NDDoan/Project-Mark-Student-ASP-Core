using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // Issue #11: Correct weighted average calculation
        // Old: Average(Value * Rate) — wrong when GradeItems have different Rates
        // New: For each subject, group marks by student, sum(Value * Rate) per student, then average those totals
        [HttpGet("average-scores")]
        public async Task<IActionResult> GetAverageScorePerSubject()
        {
            var result = await _context.Subjects
                .Select(s => new
                {
                    SubjectName = s.SubjectName,
                    // Step 1: collect all marks for this subject (with rate)
                    // Step 2: group by student, sum weighted score per student
                    // Step 3: average the per-student totals
                    AverageScore = _context.Marks
                        .Where(m => m.GradeItem.SubjectId == s.SubjectId)
                        .GroupBy(m => m.StudentId)
                        .Select(g => g.Sum(m => (decimal?)m.Value * (decimal?)m.GradeItem.Rate) ?? 0m)
                        .DefaultIfEmpty(0m)
                        .Average()
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("score-distribution")]
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
