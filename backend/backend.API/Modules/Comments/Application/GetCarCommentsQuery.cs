using backend.API.Modules.Comments.Domain;
using backend.API.Shared.Events;
using backend.API.Shared.Messaging;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Comments.Application;

public class GetCarCommentsQuery
{
    private readonly ICommentRepository _commentRepository;

    public GetCarCommentsQuery(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public Task<PagedResult<Comment>> ExecuteAsync(string carId, PaginationParameters sayfalama)
        => _commentRepository.GetByCarIdAsync(carId, sayfalama);
}

public class GetCommentQuery
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentQuery(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<Comment?> ExecuteAsync(string id)
        => await _commentRepository.GetByCommentIdAsync(id);
}

public class AddCommentCommand
{
    private readonly ICommentRepository _commentRepository;
    private readonly IRabbitMqPublisher _publisher;

    public AddCommentCommand(ICommentRepository commentRepository, IRabbitMqPublisher publisher)
    {
        _commentRepository = commentRepository;
        _publisher         = publisher;
    }

    public async Task<Comment> ExecuteAsync(AddCommentRequest istek, string userId)
    {
        var yorum = new Comment
        {
            UserId  = userId,
            CarId   = istek.CarId,
            Content = istek.Content
        };

        await _commentRepository.CreateAsync(yorum);

        // RabbitMQ: Mehmet Uludağ — yorum oluşturma event'i
        await _publisher.PublishAsync(
            new CommentCreatedEvent(yorum.Id!, yorum.CarId!, userId, DateTime.UtcNow),
            RabbitMqQueues.YorumOlusturuldu);

        return yorum;
    }
}

public class UpdateCommentCommand
{
    private readonly ICommentRepository _commentRepository;

    public UpdateCommentCommand(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<Comment> ExecuteAsync(UpdateCommentRequest istek, string userId)
    {
        var mevcut = await _commentRepository.GetByCommentIdAsync(istek.Id)
            ?? throw new KeyNotFoundException($"Yorum bulunamadı: {istek.Id}");

        if (mevcut.UserId != userId)
            throw new UnauthorizedAccessException("Bu yorumu düzenleme yetkiniz yok.");

        mevcut.Content   = istek.Content;
        mevcut.UpdatedAt = DateTime.UtcNow;

        await _commentRepository.UpdateAsync(mevcut);
        return mevcut;
    }
}

public class DeleteCommentCommand
{
    private readonly ICommentRepository _commentRepository;

    public DeleteCommentCommand(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<bool> ExecuteAsync(DeleteCommentRequest istek)
    {
        var yorum = await _commentRepository.GetByCommentIdAsync(istek.Id)
            ?? throw new KeyNotFoundException($"Yorum bulunamadı: {istek.Id}");

        if (yorum.UserId != istek.UserId)
            throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz yok.");

        return await _commentRepository.DeleteAsync(istek.Id);
    }
}

public class LikeCommentCommand
{
    private readonly ICommentRepository _commentRepository;

    public LikeCommentCommand(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<LikeSonuc> ExecuteAsync(string commentId, string userId)
    {
        var yorum = await _commentRepository.GetByCommentIdAsync(commentId)
            ?? throw new KeyNotFoundException("Yorum bulunamadı.");

        if (yorum.LikedByUsers.Contains(userId))
            throw new InvalidOperationException("Bu yorum zaten beğenilmiş.");

        await _commentRepository.LikeAsync(commentId, userId);
        var guncellenmis = await _commentRepository.GetByCommentIdAsync(commentId);

        return new LikeSonuc(commentId, guncellenmis!.LikeCount, true);
    }
}

public class UnlikeCommentCommand
{
    private readonly ICommentRepository _commentRepository;

    public UnlikeCommentCommand(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<LikeSonuc> ExecuteAsync(string commentId, string userId)
    {
        var yorum = await _commentRepository.GetByCommentIdAsync(commentId)
            ?? throw new KeyNotFoundException("Yorum bulunamadı.");

        if (!yorum.LikedByUsers.Contains(userId))
            throw new InvalidOperationException("Bu yorum zaten beğenilmemiş.");

        await _commentRepository.UnlikeAsync(commentId, userId);
        var guncellenmis = await _commentRepository.GetByCommentIdAsync(commentId);

        return new LikeSonuc(commentId, guncellenmis!.LikeCount, false);
    }
}

public record AddCommentRequest(string CarId, string Content);
public record UpdateCommentRequest(string Content, string Id);
public record DeleteCommentRequest(string Id, string UserId);
public record LikeSonuc(string YorumId, int BegeniSayisi, bool BegendimMi);
