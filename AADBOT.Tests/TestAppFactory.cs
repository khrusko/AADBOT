using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AADBOT.Tests
{
	public class TestAppFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
		{
			builder.UseEnvironment("Testing");
		}
	}
}
