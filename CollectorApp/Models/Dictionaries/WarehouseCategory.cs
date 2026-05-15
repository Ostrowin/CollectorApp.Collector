namespace CollectorApp.Models.Dictionaries;

public record WarehouseCategory(
    int Id,
    string Code,
    string Name,
    int WarehouseId
);