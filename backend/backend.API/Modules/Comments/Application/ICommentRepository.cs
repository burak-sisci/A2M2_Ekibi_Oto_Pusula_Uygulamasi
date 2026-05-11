using backend.API.Modules.Comments.Domain;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Comments.Application;

public interface ICommentRepository
{
    Task<PagedResult<Comment>> GetByCarIdAsync(string carId, PaginationParameters sayfalama);
    Task<PagedResult<Comment>> GetByUserIdAsync(string userId, PaginationParameters sayfalama);
    Task<Comment?> GetByCommentIdAsync(string id);
    Task CreateAsync(Comment yorum);
    Task UpdateAsync(Comment yorum);
    Task<bool> DeleteAsync(string id);
    Task DeleteAllByUserIdAsync(string userId);
    Task DeleteAllByCarIdAsync(string carId);
    Task<bool> LikeAsync(string commentId, string userId);
    Task<bool> UnlikeAsync(string commentId, string userId);
}
