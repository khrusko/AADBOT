﻿using AADBOT_KarloHruskovec.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AADBOT_KarloHruskovec.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Photo> Photos { get; set; }
		public DbSet<LogEntry> Logs { get; set; }

	}
}
