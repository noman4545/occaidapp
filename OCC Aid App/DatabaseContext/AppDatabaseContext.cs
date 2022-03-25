using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OCC_Aid_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.DatabaseContext
{
	public class AppDatabaseContext : IdentityDbContext<User>
	{
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

		public DbSet<IOSCode> IOSCodes { get; set; }
		public DbSet<ACID> ACIDs { get; set; }
		public DbSet<Log> Logs { get; set; }
		public DbSet<Zone> Zones { get; set; }
		public DbSet<Block> Blocks { get; set; }
		public DbSet<TMCSEmergency> TMCSEmergencies { get; set; }
		public DbSet<SMS> SMSs { get; set; }
		public DbSet<ArchievedSMS> ArchievedSMSs { get; set; }
	}
}
