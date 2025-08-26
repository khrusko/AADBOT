using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace AADBOT.Tests
{
	public class AuthIntegrationTests : IClassFixture<TestAppFactory>
	{
		private readonly HttpClient _c;

		public AuthIntegrationTests(TestAppFactory f)
		{
			_c = f.CreateClient();
		}

		[Fact]
		public async Task Me_WithoutToken_Returns401()
		{
			var r = await _c.GetAsync("/api/auth/me");
			Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
		}

		[Fact]
		public async Task Login_WithInvalidCredentials_Returns401()
		{
			var r = await _c.PostAsJsonAsync("/api/auth/login", new
			{
				email = "nepostoji@example.com",
				password = "krivasifra222"
			});
			Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
		}
	}
}
