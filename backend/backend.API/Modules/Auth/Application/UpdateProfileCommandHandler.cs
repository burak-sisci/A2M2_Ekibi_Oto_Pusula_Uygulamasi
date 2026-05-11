using MediatR;

namespace backend.API.Modules.Auth.Application;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public UpdateProfileCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.KullaniciId);
        if (user is null) return false;

        if (!string.IsNullOrWhiteSpace(request.Telefon))
            user.Phone = request.Telefon;

        user.UpdatedAt = DateTime.UtcNow;
        return await _userRepository.UpdateAsync(user.Id, user);
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.HedefKullaniciId != request.IstekciKullaniciId)
            throw new UnauthorizedAccessException("Başka bir kullanıcının profilini güncelleyemezsiniz.");

        var user = await _userRepository.GetByIdAsync(request.HedefKullaniciId);
        if (user is null) return false;

        if (!string.IsNullOrWhiteSpace(request.Ad))
            user.Ad = request.Ad;

        if (!string.IsNullOrWhiteSpace(request.Telefon))
            user.Phone = request.Telefon;

        if (!string.IsNullOrWhiteSpace(request.YeniSifre))
            user.PasswordHash = _passwordHasher.Hash(request.YeniSifre);

        user.UpdatedAt = DateTime.UtcNow;
        return await _userRepository.UpdateAsync(user.Id, user);
    }
}
