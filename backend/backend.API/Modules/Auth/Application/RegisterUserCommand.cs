using backend.API.Modules.Auth.Domain;
using backend.API.Modules.Lists.Application;
using backend.API.Shared.Database;
using backend.API.Shared.Security;

namespace backend.API.Modules.Auth.Application;

public class RegisterUserCommand
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IListRepository _listRepository;
    private readonly MongoTransactionManager _transactionManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public RegisterUserCommand(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IListRepository listRepository,
        MongoTransactionManager transactionManager,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _listRepository = listRepository;
        _transactionManager = transactionManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<RegisterResult> ExecuteAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Bu e-posta adresi zaten kullanımda.");

        if (await _userRepository.ExistsByPhoneAsync(request.Phone))
            throw new InvalidOperationException("Bu telefon numarası zaten kullanımda.");

        var user = new User
        {
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _transactionManager.ExecuteInTransactionAsync(async session =>
        {
            await _userRepository.CreateAsync(user, session);
            await _listRepository.CreateDefaultListAsync(user.Id, session);
        });

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email);

        return new RegisterResult(user.Id, user.Email, token);
    }
}

public record RegisterRequest(string Email, string Phone, string Password);
public record RegisterResult(string UserId, string Email, string Token);
