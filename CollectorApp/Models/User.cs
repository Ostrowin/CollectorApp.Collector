namespace CollectorApp.Models;

public record User(string Email, string Token, DateTime TokenExpiration);