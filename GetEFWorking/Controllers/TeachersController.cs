using GetEFWorking.Data;
using GetEFWorking.Models;
using GetEFWorking.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetEFWorking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly QueueContext _db;
    public TeachersController(QueueContext db) => _db = db;

    // GET: api/teachers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var teachers = await _db.Teachers
            .Include(t => t.Queues)
            .OrderBy(t => t.Name)
            .ToListAsync();

        return Ok(teachers.Select(t => new
        {
            t.Id,
            t.Name,
            t.Email,
            Queues = t.Queues.Select(q => new { q.Id, q.QueueName })
        }));
    }

    // GET: api/teachers/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<object>> GetById(Guid id)
    {
        var teacher = await _db.Teachers
            .Include(t => t.Queues)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (teacher is null) return NotFound();

        return Ok(new
        {
            teacher.Id,
            teacher.Name,
            teacher.Email,
            Queues = teacher.Queues.Select(q => new { q.Id, q.QueueName })
        });
    }

    // POST: api/teachers
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateTeacherRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name)) return BadRequest("Name is required");
        if (string.IsNullOrWhiteSpace(req.Email)) return BadRequest("Email is required");

        var teacher = new Teacher
        {
            Name = req.Name.Trim(),
            Email = req.Email.Trim().ToLowerInvariant(),
            PasswordHash = ""
        };

        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, new { teacher.Id, teacher.Name, teacher.Email });
    }

    // POST: api/teachers/add-to-queue/{queueId}
    // Many-to-many: Teacher <-> Queue
    [HttpPost("add-to-queue/{queueId:int}")]
    public async Task<ActionResult> AddToQueue(int queueId, [FromBody] AddTeacherToQueueRequest req)
    {
        var queue = await _db.Queues
            .Include(q => q.Teachers)
            .FirstOrDefaultAsync(q => q.Id == queueId);

        if (queue is null) return NotFound("Queue not found");

        var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Id == req.TeacherId);
        if (teacher is null) return NotFound("Teacher not found");

        if (queue.Teachers.Any(t => t.Id == teacher.Id))
            return BadRequest("Teacher already in this queue");

        queue.Teachers.Add(teacher);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/teachers/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Id == id);
        if (teacher is null) return NotFound();

        _db.Teachers.Remove(teacher);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}