namespace backend.API.Shared.Paginition;

public class PagedResult<T>
{
    public IEnumerable<T> Data {get;set;}=[];

    public long TotalCount {get;set;}

    public int Limit {get;set;}

    public int Offset {get;set;}

    public int TotalPages=>Limit>0 ? (int)Math.Ceiling((double)TotalCount/Limit) : 0;

    public static PagedResult<T> Create(IEnumerable<T> data, long totalCount, int limit, int offset)
        => new() { Data = data, TotalCount = totalCount, Limit = limit, Offset = offset };
}