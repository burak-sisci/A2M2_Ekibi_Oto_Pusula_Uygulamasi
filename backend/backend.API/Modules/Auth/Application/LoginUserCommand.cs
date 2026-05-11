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
        // Email veya telefon ile giriş desteklenir
        var user = request.Identifier.Contains('@')
            ? await _userRepository.GetByEmailAsync(request.Identifier)
            : await _userRepository.GetByPhoneAsync(request.Identifier);

        if (user is null || !_passwordHasher.Verify(request.Sifre, user.PasswordHash))
            throw new UnauthorizedAccessException("Kimlik bilgileri hatalı.");

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email);

        return new LoginResult(user.Id, user.Email, user.Ad, token);
    }
}

public record LoginRequest(string Identifier, string Sifre);
public record LoginResult(string KullaniciId, string Email, string Ad, string Token);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string YeniSifre);
