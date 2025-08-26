using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
	[Required, EmailAddress] public string Email { get; set; } = string.Empty;
	[Required, MinLength(8)] public string Password { get; set; } = string.Empty;
	[Required] public string Package { get; set; } = "FREE";
}