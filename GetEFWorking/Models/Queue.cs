namespace GetEFWorking.Models;

public class Queue
{
    public int Id { get; set; }

    public string QueueName { get; set; } = "";

    // mange-til-mange: Queue <-> Teacher
    public List<Teacher> Teachers { get; set; } = [];

    // en-til-mange: Queue -> QueueEntry
    public List<QueueEntry> QueueEntries { get; set; } = [];
}
