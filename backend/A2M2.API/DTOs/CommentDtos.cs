namespace A2M2.API.DTOs;

// ───────────── Comment DTOs (Modül D) ─────────────

public class CreateCommentRequest
{
    public string Text { get; set; } = null!;
}

public class UpdateCommentRequest
{
    public string Text { get; set; } = null!;
}
