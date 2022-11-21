using CorePinCash.Model;
using Microsoft.EntityFrameworkCore;

namespace CorePinCash.Data;

public class ApplicationDbContext : DbContext
{
	public DbSet<PinCash> PinCash { get; set; }

	public DbSet<PinItem> PinItem { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
		Database.EnsureCreated();
	}
}
