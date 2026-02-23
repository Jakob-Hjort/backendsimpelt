namespace GetEFWorking.Models;


public class Teacher : User
{
	public int TeacherId { get; set; }
	public List<Queue> Queues { get; set; } = [];
}
