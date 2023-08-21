using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WalkerAcademy.Models.WebApiModel;

namespace WalkerAcademy.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public VehicleController(IHttpClientFactory httpClient, IMemoryCache cache)
        {
            _httpClientFactory = httpClient;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "VehicleList";
            if (!_cache.TryGetValue<List<Vehicle>>(cacheKey, out var allVehicles))
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://starwars-databank-server.vercel.app");

                allVehicles = new List<Vehicle>();
                string? nextPage = "/api/v1/vehicles?page=1";

                while (!string.IsNullOrEmpty(nextPage))
                {
                    var response = await httpClient.GetAsync(nextPage);

                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var vehiclePage = JsonConvert.DeserializeObject<ApiResponse<Vehicle>>(content);

                    if (vehiclePage?.Data == null || !vehiclePage.Data.Any())
                    {
                        break;
                    }

                    allVehicles.AddRange(vehiclePage.Data);

                    nextPage = vehiclePage.Info.Next;
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for 1 hour
                };
                _cache.Set(cacheKey, allVehicles, cacheOptions);
            }
            return View(allVehicles);
        }
    }
}
