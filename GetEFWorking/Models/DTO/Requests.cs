namespace GetEFWorking.Models.Dto;

public record CreateQueueRequest(string QueueName);

public record CreateTeacherRequest(string Name, string Email);

public record CreateStudentRequest(string Name, string Email);

public record AddTeacherToQueueRequest(Guid TeacherId);

public record CreateQueueEntryRequest(Guid StudentId, Guid? TeacherId);