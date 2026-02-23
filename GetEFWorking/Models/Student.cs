namespace GetEFWorking.Models;

public class Student : User
{

	public List<QueueEntry> QueueEntries {get; set;} = [];


}
