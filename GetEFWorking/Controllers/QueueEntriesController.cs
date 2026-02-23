using GetEFWorking.Data;
using GetEFWorking.Models;
using GetEFWorking.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetEFWorking.Controllers;

[ApiController]
[Route("api/queues/{queueId:int}/entries")]
public class QueueEntriesController : ControllerBase
{
    private readonly QueueContext _db;
    public QueueEntriesController(QueueContext db) => _db = db;

    // GET: api/queues/{queueId}/entries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll(int queueId)
    {
        var queueExists = await _db.Queues.AnyAsync(q => q.Id == queueId);
        if (!queueExists) return NotFound("Queue not found");

        var entries = await _db.QueueEntries
            .Where(e => e.QueueId == queueId)
            .Include(e => e.Student)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();

        return Ok(entries.Select(e => new
        {
            e.Id,
            e.CreatedAt,
            e.StudentId,
            Student = new { e.Student.Name, e.Student.Email },
            e.TeacherId
        }));
    }

    // POST: api/queues/{queueId}/entries
    [HttpPost]
    public async Task<ActionResult<object>> Create(int queueId, [FromBody] CreateQueueEntryRequest req)
    {
        var queueExists = await _db.Queues.AnyAsync(q => q.Id == queueId);
        if (!queueExists) return NotFound("Queue not found");

        var studentExists = await _db.Students.AnyAsync(s => s.Id == req.StudentId);
        if (!studentExists) return NotFound("Student not found");

        // Undgå dobbelttilmelding (hvis du har unique index på QueueId+StudentId)
        var already = await _db.QueueEntries.AnyAsync(e => e.QueueId == queueId && e.StudentId == req.StudentId);
        if (already) return BadRequest("Student already signed up for this queue");

        if (req.TeacherId is not null)
        {
            var teacherExists = await _db.Teachers.AnyAsync(t => t.Id == req.TeacherId);
            if (!teacherExists) return NotFound("Teacher not found");
        }

        var entry = new QueueEntry
        {
            QueueId = queueId,
            StudentId = req.StudentId,
            TeacherId = req.TeacherId
        };

        _db.QueueEntries.Add(entry);
        await _db.SaveChangesAsync();

        return Ok(new { entry.Id, entry.QueueId, entry.StudentId, entry.TeacherId, entry.CreatedAt });
    }

    // DELETE: api/queues/{queueId}/entries/{entryId}
    [HttpDelete("{entryId:int}")]
    public async Task<ActionResult> Delete(int queueId, int entryId)
    {
        var entry = await _db.QueueEntries.FirstOrDefaultAsync(e => e.Id == entryId && e.QueueId == queueId);
        if (entry is null) return NotFound();

        _db.QueueEntries.Remove(entry);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}