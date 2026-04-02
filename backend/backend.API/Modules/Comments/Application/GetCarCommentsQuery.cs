using backend.API.Modules.Comments.Application;
using backend.API.Modules.Comments.Domain;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Comments.Application;

public class GetCarCommentsQuery
{
    private readonly ICommentRepository _commentRepository;

    public GetCarCommentsQuery(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public Task<PagedResult<Comment>> ExecuteAsync(string carId, PaginationParameters pagination)
         => _commentRepository.GetByCarIdAsync(carId, pagination);
}

public class GetCommentQuery
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentQuery(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<Comment> ExecuteAsync(string id)
         => await _commentRepository.GetByCommentIdAsync(id);
}

public class AddCommentCommand
{
    private readonly ICommentRepository _commentRepository;

    public AddCommentCommand(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<Comment> ExecuteAsync(AddCommentRequest request, string userId)
    {
        var comment = new Comment
        {
            UserId = userId,
            CarId = request.CarId,
            Content = request.Content
        };
        await _commentRepository.CreateAsync(comment);
        return comment;
    }

}

public class UpdateCommentCommand
{
    private readonly ICommentRepository _commentRepository;

    public UpdateCommentCommand(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<Comment> ExecuteAsync(UpdateCommentRequest request, string userId)
    {
        var existingComment = await _commentRepository.GetByCommentIdAsync(request.Id);
        if (existingComment == null)
        {
            throw new KeyNotFoundException($"Yorumun ID {request.Id} bulunamadı");
        }

        if (existingComment.UserId != userId)
        {
            throw new UnauthorizedAccessException("Bu yorumu düzenleme yetkiniz yok.");
        }

        existingComment.Content = request.Content;
        await _commentRepository.UpdateAsync(existingComment);
        return existingComment;
    }
}

public class DeleteCommentCommand
{
    private readonly ICommentRepository _commentRepository;

    public DeleteCommentCommand(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<bool> ExecuteAsync(DeleteCommentRequest request)
    {
        var comment = await _commentRepository.GetByCommentIdAsync(request.Id);
        if (comment == null)
        {
            throw new KeyNotFoundException($"Yorumun ID {request.Id} bulunamadı");
        }
        if (comment.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz yok.");
        }
        await _commentRepository.DeleteAsync(request.Id);
        return true;
    }


}

public record AddCommentRequest(string CarId, string Content);
public record UpdateCommentRequest(string Content, string Id);
public record DeleteCommentRequest(string Id,string UserId);