using System.Linq;
using AADBOT_KarloHruskovec.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AADBOT.Tests
{
	// Uses TestServer (default). It also swaps SQL Server for EF InMemory in "Testing".
	public class TestAppFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Testing");

			builder.ConfigureServices(services =>
			{
				// Remove the existing SQL Server registration
				var toRemove = services
					.Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>))
					.ToList();
				foreach (var d in toRemove) services.Remove(d);

				// Add InMemory DB for tests
				services.AddDbContext<ApplicationDbContext>(opts =>
					opts.UseInMemoryDatabase("testdb"));
			});
		}
	}
}
