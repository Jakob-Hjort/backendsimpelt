namespace GetEFWorking.Models;

public class QueueEntry
{
    public int Id { get; set; }

    public int QueueId { get; set; }
    public Queue Queue { get; set; } = null!;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    // valgfri: hvilken lærer håndterer entry
    public Guid? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}