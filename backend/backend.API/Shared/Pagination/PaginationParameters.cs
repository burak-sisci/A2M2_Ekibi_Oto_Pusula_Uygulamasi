namespace backend.API.Shared.Paginition;

public class PaginationParameters
{
    private const int MaxLimit=100;
    private int _limit = 20;
    private int _offset = 0;

    public int Limit
    {
        get => _limit;
        set => _limit = value > MaxLimit ? MaxLimit : value < 1 ? 1 : value;
    }

    public int Offset
    {
        get => _offset;
        set => _offset = value < 0 ? 0 : value;
    }
}