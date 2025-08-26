using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequest
{
	[Required, EmailAddress] public string Email { get; set; } = string.Empty;
	[Required] public string RefreshToken { get; set; } = string.Empty;
}