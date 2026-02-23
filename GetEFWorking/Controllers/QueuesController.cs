using GetEFWorking.Data;
using GetEFWorking.Models;
using GetEFWorking.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetEFWorking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueuesController : ControllerBase
{
    private readonly QueueContext _db;
    public QueuesController(QueueContext db) => _db = db;

    // GET: api/queues
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var queues = await _db.Queues
            .Include(q => q.Teachers)
            .Include(q => q.QueueEntries).ThenInclude(e => e.Student)
            .OrderBy(q => q.QueueName)
            .ToListAsync();

        var result = queues.Select(q => new
        {
            q.Id,
            q.QueueName,
            Teachers = q.Teachers.Select(t => new { t.Id, t.Name, t.Email }),
            Entries = q.QueueEntries
                .OrderBy(e => e.CreatedAt)
                .Select(e => new
                {
                    e.Id,
                    e.CreatedAt,
                    Student = new { e.Student.Id, e.Student.Name, e.Student.Email },
                    e.TeacherId
                })
        });

        return Ok(result);
    }

    // GET: api/queues/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var q = await _db.Queues
            .Include(x => x.Teachers)
            .Include(x => x.QueueEntries).ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (q is null) return NotFound();

        return Ok(new
        {
            q.Id,
            q.QueueName,
            Teachers = q.Teachers.Select(t => new { t.Id, t.Name, t.Email }),
            Entries = q.QueueEntries
                .OrderBy(e => e.CreatedAt)
                .Select(e => new
                {
                    e.Id,
                    e.CreatedAt,
                    Student = new { e.Student.Id, e.Student.Name, e.Student.Email },
                    e.TeacherId
                })
        });
    }

    // POST: api/queues
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateQueueRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.QueueName))
            return BadRequest("QueueName is required");

        var queue = new Queue
        {
            QueueName = req.QueueName.Trim()
        };

        _db.Queues.Add(queue);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = queue.Id }, new { queue.Id, queue.QueueName });
    }

    // DELETE: api/queues/5
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var q = await _db.Queues.FirstOrDefaultAsync(x => x.Id == id);
        if (q is null) return NotFound();

        _db.Queues.Remove(q);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}