using A2M2.API.DTOs;
using A2M2.API.Models;
using MongoDB.Driver;

namespace A2M2.API.Services;

/// <summary>
/// Kimlik doğrulama servisi — Register, Login
/// </summary>
public class AuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly JwtService _jwtService;
    private readonly ListService _listService;

    public AuthService(IMongoDatabase database, JwtService jwtService, ListService listService)
    {
        _users = database.GetCollection<User>("users");
        _jwtService = jwtService;
        _listService = listService;

        // Email unique index
        var emailIndex = Builders<User>.IndexKeys.Ascending(u => u.Email);
        _users.Indexes.CreateOne(new CreateIndexModel<User>(emailIndex, new CreateIndexOptions { Unique = true }));

        // Phone unique index
        var phoneIndex = Builders<User>.IndexKeys.Ascending(u => u.Phone);
        _users.Indexes.CreateOne(new CreateIndexModel<User>(phoneIndex, new CreateIndexOptions { Unique = true }));
    }

    /// <summary>
    /// Yeni kullanıcı kaydı — kayıt sonrası otomatik "Favoriler" listesi oluşturur
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Email kontrolü
        var existing = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        if (existing != null)
            throw new InvalidOperationException("Bu e-posta adresi zaten kayıtlı");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = request.Phone,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
        };

        await _users.InsertOneAsync(user);

        // Varsayılan "Favoriler" listesi oluştur
        await _listService.CreateDefaultListAsync(user.Id);

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name);

        return new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Token = token,
        };
    }

    /// <summary>
    /// Kullanıcı girişi
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Geçersiz e-posta veya şifre");

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name);

        return new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Token = token,
        };
    }

    /// <summary>
    /// Kullanıcı bilgilerini getir
    /// </summary>
    public async Task<User?> GetByIdAsync(string userId)
    {
        return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
    }
}
