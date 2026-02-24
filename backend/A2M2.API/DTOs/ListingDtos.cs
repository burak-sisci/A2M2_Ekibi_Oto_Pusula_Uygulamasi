using A2M2.API.Models;

namespace A2M2.API.DTOs;

// ───────────── Listing DTOs ─────────────

public class CreateListingRequest
{
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public int Km { get; set; }
    public string FuelType { get; set; } = null!;
    public string GearType { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public List<string>? Images { get; set; }
    public LocationInfo? Location { get; set; }
    public List<string>? DamageInfo { get; set; }
}

public class UpdateListingRequest
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public int? Km { get; set; }
    public string? FuelType { get; set; }
    public string? GearType { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public List<string>? Images { get; set; }
    public LocationInfo? Location { get; set; }
    public List<string>? DamageInfo { get; set; }
}

public class ListingFilterParams
{
    public string? Brand { get; set; }
    public string? FuelType { get; set; }
    public string? GearType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Location { get; set; }
    public string? SortBy { get; set; } // price, year, km, createdAt
    public string? SortOrder { get; set; } // asc, desc
}

public class PricePredictionRequest
{
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public int Km { get; set; }
    public string FuelType { get; set; } = null!;
    public string GearType { get; set; } = null!;
}
