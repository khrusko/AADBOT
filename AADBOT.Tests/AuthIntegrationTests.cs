using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace AADBOT.Tests
{
	public class AuthIntegrationTests : IClassFixture<TestAppFactory>
	{
		private readonly HttpClient _http;

		public AuthIntegrationTests(TestAppFactory f)
		{
			_http = f.CreateClient();
		}

		[Fact]
		public async Task Me_WithoutToken_Returns401()
		{
			var resp = await _http.GetAsync("/api/auth/me");
			if (resp.StatusCode == HttpStatusCode.NotFound)
				resp = await _http.GetAsync("/auth/me");

			Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
		}

		[Fact]
		public async Task Login_WithInvalidCredentials_Returns401()
		{
			var body = new { email = "nope@example.com", password = "WrongPass!123" };

			var resp = await _http.PostAsJsonAsync("/api/auth/login", body);
			if (resp.StatusCode == HttpStatusCode.NotFound)
				resp = await _http.PostAsJsonAsync("/auth/login", body);

			Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
		}
	}
}
