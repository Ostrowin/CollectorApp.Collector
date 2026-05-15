using System.Net.Http.Json;
using CollectorApp.Helpers;
using CollectorApp.Models.Dictionaries;
using CollectorApp.Services.Interfaces;

namespace CollectorApp.Services.Implementations;

public class DictionaryService : IDictionaryService
{
    private readonly HttpClient _httpClient;
    private readonly string _cacheDirectory;

    private readonly string WarehousesFile = "cache_warehouses.json";
    private readonly string WarehousesCategoryFile = "cache_warehouses_categories.json";
    private readonly string ContractorsFile = "cache_contractors.json";

    private List<Warehouse> _warehouses = [];
    private List<WarehouseCategory> _warehouseCategories = [];
    private List<Contractor> _contractors = [];
    public bool isLoaded {  get; private set; }

    public DictionaryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _cacheDirectory = FileSystem.Current.AppDataDirectory;
    }

    public async Task RefreshAllAsync()
    {
        var apiSuccess = await TryLoadFromApiAsync();

        if (!apiSuccess)
        {
            await LoadFromCacheAsync();
        }
        isLoaded = true;
    }

    //public async Task<bool> TryLoadFromApiAsync()
    //{
    //    try
    //    {
    //        var warehouses = await _httpClient.GetFromJsonAsync<List<Warehouse>>("/api/Warehouses");
    //        var warehouseCategories = await _httpClient.GetFromJsonAsync<List<WarehouseCategory>>("/api/WarehouseCategories");
    //        var contractors = await _httpClient.GetFromJsonAsync<List<Contractor>>("/api/Contractors");

    //        if (warehouses is null || warehouseCategories is null || contractors is null)
    //        {
    //            return false;
    //        }

    //        _warehouses = warehouses;
    //        _warehouseCategories = warehouseCategories;
    //        _contractors = contractors;

    //        await SaveToCacheAsync();

    //        return true;
    //    }
    //    catch(Exception ex)
    //    {
    //        System.Diagnostics.Debug.WriteLine($"Dictionary API error: {ex.Message}");
    //        return false;
    //    }
    //}

    private async Task<bool> TryLoadFromApiAsync()
    {
#if DEBUG
        // MOCK – usuń gdy endpointy API będą gotowe
        await Task.Delay(1500); // symulacja opóźnienia sieciowego

        _warehouses =
        [
            new Warehouse(1, "MAG-01", "Magazyn Główny", "Magazyn centralny"),
        new Warehouse(2, "MAG-02", "Magazyn Zewnętrzny", "Magazyn przy bramie"),
        new Warehouse(3, "MAG-03", "Magazyn Zwrotów", "Zwroty i reklamacje"),
    ];

        _warehouseCategories =
        [
            new WarehouseCategory(1, "KAT-01", "Elektronika", 1),
        new WarehouseCategory(2, "KAT-02", "AGD", 1),
        new WarehouseCategory(3, "KAT-03", "Spożywcze", 2),
        new WarehouseCategory(4, "KAT-04", "Zwroty klientów", 3),
    ];

        _contractors =
        [
            new Contractor(1, "KON-01", "ABC Sp. z o.o.", "1234567890", "ul. Przykładowa 1, Warszawa"),
        new Contractor(2, "KON-02", "XYZ S.A.", "0987654321", "ul. Testowa 2, Kraków"),
        new Contractor(3, "KON-03", "Jan Kowalski", "1122334455", "ul. Fikcyjna 3, Gdańsk"),
    ];

        await SaveToCacheAsync();
        return true;
#else
    try
    {
        // prawdziwe API – docelowy kod
        var warehouses = await _httpClient
            .GetFromJsonAsync<List<Warehouse>>("/api/Warehouses");
        // ...
        return true;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Dictionary API error: {ex.Message}");
        return false;
    }
#endif
    }

    public async Task LoadFromCacheAsync()
    {
        var warehouses = await LocalStorageHelper.LoadAsync<List<Warehouse>>(WarehousesFile, _cacheDirectory);
        var warehouseCategories = await LocalStorageHelper.LoadAsync<List<WarehouseCategory>>(WarehousesCategoryFile, _cacheDirectory);
        var contractors = await LocalStorageHelper.LoadAsync<List<Contractor>>(ContractorsFile, _cacheDirectory);
        
        if (warehouses is null || warehouseCategories is null || contractors is null || contractors is null)
            throw new InvalidOperationException("No local cache available for dictionaries. Internet Connection required.");

        _warehouses = warehouses;
        _warehouseCategories = warehouseCategories;
        _contractors = contractors;
    }

    public async Task SaveToCacheAsync() {
        await LocalStorageHelper.SaveAsync(WarehousesFile, _warehouses, _cacheDirectory);
        await LocalStorageHelper.SaveAsync(WarehousesCategoryFile, _warehouseCategories, _cacheDirectory);
        await LocalStorageHelper.SaveAsync(ContractorsFile, _contractors, _cacheDirectory);
    }

    public Task<List<Warehouse>> GetWarehousesAsync() => Task.FromResult(_warehouses);
    public Task<List<WarehouseCategory>> GetWarehouseCategoriesAsync() => Task.FromResult(_warehouseCategories);
    public Task<List<Contractor>> GetContractorsAsync() => Task.FromResult(_contractors);
}
