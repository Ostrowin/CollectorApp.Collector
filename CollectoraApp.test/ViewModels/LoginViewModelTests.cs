using CollectorApp.Models;
using CollectorApp.Services.Interfaces;
using CollectorApp.ViewModels;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CollectorApp.Tests.ViewModels;

public class LoginViewModelTests
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly LoginViewModel _sut; // sut = System Under Test

    public LoginViewModelTests()
    {
        _authService = Substitute.For<IAuthService>();
        _navigationService = Substitute.For<INavigationService>();
        _sut = new LoginViewModel(_authService, _navigationService);
    }

    // --- CanLogin ---

    [Fact]
    public void CanLogin_WhenFieldsEmpty_ReturnsFalse()
    {
        // Arrange – domyślnie pola są puste (ustawione w konstruktorze)

        // Act
        var canLogin = _sut.LoginCommand.CanExecute(null);

        // Assert
        Assert.False(canLogin);
    }

    [Fact]
    public void CanLogin_WhenFieldsValid_ReturnsTrue()
    {
        // Arrange
        _sut.Username = "admin";
        _sut.Password = "admin";

        // Act
        var canLogin = _sut.LoginCommand.CanExecute(null);

        // Assert
        Assert.True(canLogin);
    }

    [Theory]
    [InlineData("ab", "admin")]   // username za krótki
    [InlineData("admin", "ab")]   // password za krótki
    [InlineData("ab", "ab")]      // oba za krótkie
    public void CanLogin_WhenFieldsTooShort_ReturnsFalse(string username, string password)
    {
        // Arrange
        _sut.Username = username;
        _sut.Password = password;

        // Act
        var canLogin = _sut.LoginCommand.CanExecute(null);

        // Assert
        Assert.False(canLogin);
    }

    // --- LoginAsync ---

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_NavigatesToScanner()
    {
        // Arrange
        _sut.Username = "admin";
        _sut.Password = "admin";
        _authService
            .LoginAsync("admin", "admin")
            .Returns(new User("admin", "mock-token"));

        // Act
        await _sut.LoginCommand.ExecuteAsync(null);

        // Assert
        await _navigationService.Received(1).GoToAsync("//scanner");
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsInvalid_SetsErrorMessage()
    {
        // Arrange
        _sut.Username = "admin";
        _sut.Password = "wrong";
        _authService
            .LoginAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns((User?)null);

        // Act
        await _sut.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_sut.ErrorMessage));
        await _navigationService.DidNotReceive().GoToAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task LoginAsync_WhenExceptionThrown_SetsErrorMessageAndNotBusy()
    {
        // Arrange
        _sut.Username = "admin";
        _sut.Password = "admin";
        _authService
            .LoginAsync(Arg.Any<string>(), Arg.Any<string>())
            .Throws(new HttpRequestException("Network error"));

        // Act
        await _sut.LoginCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_sut.ErrorMessage));
        Assert.False(_sut.IsBusy);
    }

    [Fact]
    public async Task LoginAsync_WhenExecuting_IsBusyIsTrue()
    {
        // Arrange
        _sut.Username = "admin";
        _sut.Password = "admin";

        var tcs = new TaskCompletionSource<User?>();
        _authService
            .LoginAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(tcs.Task);

        // Act
        var loginTask = _sut.LoginCommand.ExecuteAsync(null);

        // Assert – w trakcie wykonywania
        Assert.True(_sut.IsBusy);

        // Cleanup
        tcs.SetResult(null);
        await loginTask;
    }
}