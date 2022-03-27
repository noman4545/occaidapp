using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OCC_Aid_App.Models;

namespace OCC_Aid_App.DatabaseContext
{
    public class AppDatabaseContext : IdentityDbContext<User>
	{
		#region Members
		public DbSet<IOSCode> IOSCodes { get; set; }
		public DbSet<ACID> ACIDs { get; set; }
		public DbSet<Log> Logs { get; set; }
		public DbSet<Zone> Zones { get; set; }
		public DbSet<Block> Blocks { get; set; }
		public DbSet<TMCSEmergency> TMCSEmergencies { get; set; }
		public DbSet<SMS> SMSs { get; set; }
		public DbSet<ArchievedSMS> ArchievedSMSs { get; set; }
		public DbSet<V1_Zone> V1_Zones { get; set; }
		public DbSet<V1_Block> V1_Blocks { get; set; }
		public DbSet<V1_ZoneBlock> V1_ZoneBlocks { get; set; }
		#endregion

		public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<User>(entity =>
			{
				entity.ToTable(name: "Users");
				entity.Property(e => e.Id).HasColumnName("UserId");
			});
		}
	}
}
