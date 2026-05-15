using CollectorApp.Models.Dictionaries;

namespace CollectorApp.Services.Interfaces;

public interface IDictionaryService
{
    bool isLoaded { get; }
    Task RefreshAllAsync();
    Task<List<Warehouse>> GetWarehousesAsync();
    Task<List<WarehouseCategory>> GetWarehouseCategoriesAsync();
    Task<List<Contractor>> GetContractorsAsync();
}
