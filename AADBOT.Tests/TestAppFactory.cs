using System.Linq;
using AADBOT_KarloHruskovec.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AADBOT.Tests
{
	public class TestAppFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Testing");

			builder.ConfigureServices(services =>
			{
				var toRemove = services
					.Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>))
					.ToList();
				foreach (var d in toRemove) services.Remove(d);

				//InMemory
				services.AddDbContext<ApplicationDbContext>(opts =>
					opts.UseInMemoryDatabase("testdb"));
			});
		}
	}
}
