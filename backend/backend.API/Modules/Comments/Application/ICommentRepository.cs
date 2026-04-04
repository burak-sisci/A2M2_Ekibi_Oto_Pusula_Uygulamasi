using backend.API.Modules.Comments.Domain;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Comments.Application;

public interface ICommentRepository
{
    Task<PagedResult<Comment>> GetByCarIdAsync(string carId, PaginationParameters pagination);

    Task<Comment> GetByCommentIdAsync(string id);
    Task CreateAsync(Comment comment);

    Task UpdateAsync(Comment comment);

    Task<bool> DeleteAsync(string id);

    Task DeleteAllByUserIdAsync(string userId);
    
    Task DeleteAllByCarIdAsync(string carId);


}