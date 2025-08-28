namespace AADBOT_KarloHruskovec.Application.Auth;

public sealed record RegisterUserCommand(string Email, string Password, string Package);
