using GetEFWorking.Data;
using GetEFWorking.Models;
using GetEFWorking.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetEFWorking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly QueueContext _db;
    public StudentsController(QueueContext db) => _db = db;

    // GET: api/students
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var students = await _db.Students
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(students.Select(s => new { s.Id, s.Name, s.Email }));
    }

    // GET: api/students/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<object>> GetById(Guid id)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student is null) return NotFound();

        return Ok(new { student.Id, student.Name, student.Email });
    }

    // POST: api/students
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateStudentRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name)) return BadRequest("Name is required");
        if (string.IsNullOrWhiteSpace(req.Email)) return BadRequest("Email is required");

        var student = new Student
        {
            Name = req.Name.Trim(),
            Email = req.Email.Trim().ToLowerInvariant(),
            PasswordHash = ""
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, new { student.Id, student.Name, student.Email });
    }

    // DELETE: api/students/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student is null) return NotFound();

        _db.Students.Remove(student);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}