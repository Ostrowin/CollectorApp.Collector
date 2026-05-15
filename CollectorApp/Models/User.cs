namespace CollectorApp.Models;

public record User(string Login, string Token, DateTime TokenExpiration);