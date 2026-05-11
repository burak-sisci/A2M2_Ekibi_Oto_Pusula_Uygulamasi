using backend.API.Modules.Auth.Domain;
using backend.API.Modules.Lists.Application;
using backend.API.Shared.Database;
using backend.API.Shared.Events;
using backend.API.Shared.Messaging;
using backend.API.Shared.Security;

namespace backend.API.Modules.Auth.Application;

public class RegisterUserCommand
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IListRepository _listRepository;
    private readonly MongoTransactionManager _transactionManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly IRabbitMqPublisher _publisher;

    public RegisterUserCommand(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IListRepository listRepository,
        MongoTransactionManager transactionManager,
        JwtTokenGenerator jwtTokenGenerator,
        IRabbitMqPublisher publisher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _listRepository = listRepository;
        _transactionManager = transactionManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _publisher = publisher;
    }

    public async Task<RegisterResult> ExecuteAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Bu e-posta adresi zaten kullanımda.");

        if (await _userRepository.ExistsByPhoneAsync(request.Phone))
            throw new InvalidOperationException("Bu telefon numarası zaten kullanımda.");

        var user = new User
        {
            Ad           = request.Ad,
            Email        = request.Email,
            Phone        = request.Phone,
            PasswordHash = _passwordHasher.Hash(request.Sifre),
            Cinsiyet     = request.Cinsiyet,
            DogumTarihi  = request.DogumTarihi
        };

        await _transactionManager.ExecuteInTransactionAsync(async session =>
        {
            await _userRepository.CreateAsync(user, session);
            await _listRepository.CreateDefaultListAsync(user.Id, session);
        });

        // RabbitMQ: Burak Şişci — kullanıcı kaydı event'i
        await _publisher.PublishAsync(
            new UserRegisteredEvent(user.Id, user.Email, user.Ad, DateTime.UtcNow),
            RabbitMqQueues.KullaniciKayit);

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email);

        return new RegisterResult(user.Id, user.Email, user.Ad, token);
    }
}

public record RegisterRequest(
    string Ad,
    string Email,
    string Phone,
    string Sifre,
    Cinsiyet? Cinsiyet = null,
    DateTime? DogumTarihi = null);

public record RegisterResult(string KullaniciId, string Email, string Ad, string Token);
