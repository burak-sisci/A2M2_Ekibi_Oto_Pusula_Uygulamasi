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
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user == null)
            return false; 
        user.Phone = request.Phone ?? user.Phone;

        await _userRepository.UpdateAsync(user.Id, user);

        return true; 
    }
}