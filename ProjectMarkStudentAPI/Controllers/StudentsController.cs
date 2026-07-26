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
    public class StudentsController : ControllerBase
    {
        private readonly ProjectStudentMarkContext _context;
        private readonly IMapper _mapper;

        public StudentsController(ProjectStudentMarkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Students (OData enabled)
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            var students = await _context.Students.AsNoTracking().ToListAsync(); // Issue #13: AsNoTracking for read-only
            return Ok(_mapper.Map<IEnumerable<StudentDTO>>(students));
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<StudentDTO>(student));
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, StudentDTO studentDTO)
        {
            // Issue #3: Use StudentId (renamed from Id for {Entity}Id consistency)
            if (id != studentDTO.StudentId)
            {
                return BadRequest();
            }

            // [ApiController] already validates ModelState automatically — manual check removed (Issue #10)

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _mapper.Map(studentDTO, student);
            student.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<StudentDTO>> PostStudent(StudentDTO studentDTO)
        {
            // [ApiController] already validates ModelState automatically — manual check removed (Issue #10)

            var student = _mapper.Map<Student>(studentDTO);
            student.CreatedAt = DateTime.Now;
            student.UpdatedAt = DateTime.Now;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Issue #3: Populate back the StudentId field
            studentDTO.StudentId = student.Id;

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, studentDTO);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportStudents()
        {
            var students = await _context.Students.AsNoTracking().ToListAsync();
            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("Students");
            ws.Cell(1, 1).Value = "Id";
            ws.Cell(1, 2).Value = "RollNumber";
            ws.Cell(1, 3).Value = "Email";
            ws.Cell(1, 4).Value = "FirstName";
            ws.Cell(1, 5).Value = "LastName";
            ws.Cell(1, 6).Value = "Dob";
            ws.Cell(1, 7).Value = "Address";
            ws.Cell(1, 8).Value = "PhoneNumber";
            ws.Cell(1, 9).Value = "Gender";

            var row = 2;
            foreach (var s in students)
            {
                ws.Cell(row, 1).Value = s.Id;
                ws.Cell(row, 2).Value = s.RollNumber;
                ws.Cell(row, 3).Value = s.Email;
                ws.Cell(row, 4).Value = s.FirstName;
                ws.Cell(row, 5).Value = s.LastName;
                ws.Cell(row, 6).Value = s.Dob?.ToString("yyyy-MM-dd") ?? "";
                ws.Cell(row, 7).Value = s.Address;
                ws.Cell(row, 8).Value = s.PhoneNumber;
                ws.Cell(row, 9).Value = s.Gender;
                row++;
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachSinhVien.xlsx");
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            var importedCount = 0;
            using (var stream = file.OpenReadStream())
            {
                using var wb = new ClosedXML.Excel.XLWorkbook(stream);
                var ws = wb.Worksheets.First();
                var rows = ws.RowsUsed().Skip(1); // skip header
                
                foreach (var row in rows)
                {
                    int.TryParse(row.Cell(1).GetString(), out var id);

                    var roll = row.Cell(2).GetString().Trim();
                    var email = row.Cell(3).GetString().Trim();
                    var fn = row.Cell(4).GetString().Trim();
                    var ln = row.Cell(5).GetString().Trim();
                    var dobStr = row.Cell(6).GetString().Trim();
                    DateOnly? dob = null;
                    if (DateOnly.TryParse(dobStr, out var d)) dob = d;
                    var addr = row.Cell(7).GetString().Trim();
                    var phone = row.Cell(8).GetString().Trim();
                    var gender = row.Cell(9).GetString().Trim();

                    Student s = null;
                    if (id > 0)
                    {
                        s = await _context.Students.FindAsync(id);
                    }
                    
                    bool isNew = false;
                    if (s == null)
                    {
                        s = new Student();
                        s.CreatedAt = DateTime.Now;
                        isNew = true;
                    }

                    s.RollNumber = roll;
                    s.Email = email;
                    s.FirstName = fn;
                    s.LastName = ln;
                    s.Dob = dob;
                    s.Address = addr;
                    s.PhoneNumber = phone;
                    s.Gender = gender;
                    s.UpdatedAt = DateTime.Now;

                    if (isNew)
                    {
                        _context.Students.Add(s);
                    }

                    importedCount++;
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"Đã import {importedCount} sinh viên thành công." });
        }
    }
}
