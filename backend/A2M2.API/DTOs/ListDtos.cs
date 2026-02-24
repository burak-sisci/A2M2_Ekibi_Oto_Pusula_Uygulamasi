namespace A2M2.API.DTOs;

// ───────────── List DTOs (Modül C) ─────────────

public class CreateListRequest
{
    public string Name { get; set; } = null!;
}

public class UpdateListRequest
{
    public string Name { get; set; } = null!;
}

public class AddCarToListRequest
{
    public string CarId { get; set; } = null!;
}
