namespace simpeltkoapi.Models;

public class Queue
{
    public int Id {get; set;}

    public string QueueName {get; set;} = "";

    public List<Teacher> Teachers {get; set;} = [];
 
}