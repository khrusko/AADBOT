using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace AADBOT.Tests
{
	public class UiHttpFlowTests : IClassFixture<TestAppFactory>
	{
		private readonly HttpClient _http;

		public UiHttpFlowTests(TestAppFactory f)
		{
			_http = f.CreateClient(); // TestServer client
		}

		[Fact]
		public async Task Me_Page_Unauthenticated_Shows401()
		{
			var resp = await _http.GetAsync("/api/auth/me");
			if (resp.StatusCode == HttpStatusCode.NotFound)
				resp = await _http.GetAsync("/auth/me");

			Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
		}

		[Fact]
		public async Task Login_InvalidCredentials_Shows401()
		{
			var body = new { email = "invalid@example.com", password = "WrongPass!123" };

			var resp = await _http.PostAsJsonAsync("/api/auth/login", body);
			if (resp.StatusCode == HttpStatusCode.NotFound)
				resp = await _http.PostAsJsonAsync("/auth/login", body);

			Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
		}
	}
}
