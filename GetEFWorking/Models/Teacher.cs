namespace GetEFWorking.Models;


public class Teacher : User
{

	 // Mange-til-mange: lærer kan være i flere queues
    public List<Queue> Queues { get; set; } = [];

    // Valgfrit: hvis du vil se hvilke entries en lærer har taget
    public List<QueueEntry> QueueEntries { get; set; } = [];
}






   
