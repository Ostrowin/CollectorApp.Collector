namespace CollectorApp.Models.Api;

public record LoginResponse(string Token, DateTime Expiration);