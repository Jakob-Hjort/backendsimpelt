using Microsoft.EntityFrameworkCore;
using GetEFWorking.Models;

namespace GetEFWorking.Data;

public class QueueContext : DbContext
{
	public DbSet<User> Users => Set<User>();
	public DbSet<Teacher> Teachers => Set<Teacher>();
	public DbSet<Student> Students => Set<Student>();
	public DbSet<Queue> Queues => Set<Queue>();
	public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// TPT
		modelBuilder.Entity<User>().UseTptMappingStrategy();

		// Mange-til-mange: Teacher <-> Queue
		modelBuilder.Entity<Teacher>()
			.HasMany(t => t.Queues)
			.WithMany(q => q.Teachers)
			.UsingEntity(j => j.ToTable("TeacherQueues"));

		// 1-til-mange: Queue -> QueueEntries
		modelBuilder.Entity<QueueEntry>()
			.HasOne(e => e.Queue)
			.WithMany(q => q.QueueEntries)
			.HasForeignKey(e => e.QueueId);

		// 1-til-mange: Student -> QueueEntries
		modelBuilder.Entity<QueueEntry>()
			.HasOne(e => e.Student)
			.WithMany(s => s.QueueEntries)
			.HasForeignKey(e => e.StudentId);

		// valgfri: Teacher -> QueueEntries (assigned)
		modelBuilder.Entity<QueueEntry>()
			.HasOne(e => e.Teacher)
			.WithMany(t => t.QueueEntries)
			.HasForeignKey(e => e.TeacherId)
			.OnDelete(DeleteBehavior.SetNull);

		// forhindrer dobbelt-tilmelding til samme kø
		modelBuilder.Entity<QueueEntry>()
			.HasIndex(e => new { e.QueueId, e.StudentId })
			.IsUnique();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var folder = Environment.SpecialFolder.LocalApplicationData;
		var path = Environment.GetFolderPath(folder);
		string dbPath = Path.Join(path, "queues.db");
		optionsBuilder.UseSqlite($"Data Source={dbPath}");
		base.OnConfiguring(optionsBuilder);
		Console.WriteLine(dbPath);
	}
}
