using backend.API.Shared.Security;

namespace backend.API.Modules.Auth.Application;

public class LoginUserCommand
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommand(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResult> ExecuteAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Geçersiz e-posta veya şifre.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Geçersiz e-posta veya şifre.");

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email);
        return new LoginResult(user.Id, user.Email, token);
    }
}

public record LoginRequest(string Email, string Password);
public record LoginResult(string UserId, string Email, string Token);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);