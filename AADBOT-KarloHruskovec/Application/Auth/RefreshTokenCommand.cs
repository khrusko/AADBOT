namespace AADBOT_KarloHruskovec.Application.Auth;

public sealed record RefreshTokenCommand(string Email, string RefreshToken);
