using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WalkerAcademy.Models.WebApiModel;

namespace WalkerAcademy.Controllers
{
    public class LocationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public LocationController(IHttpClientFactory httpClient, IMemoryCache cache)
        {
            _httpClientFactory = httpClient;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "LocationsList";
            if (!_cache.TryGetValue<List<Location>>(cacheKey, out var allLocations))
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://starwars-databank-server.vercel.app");

                allLocations = new List<Location>();
                string? nextPage = "/api/v1/locations?page=1";

                while (!string.IsNullOrEmpty(nextPage))
                {
                    var response = await httpClient.GetAsync(nextPage);

                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var locationPage = JsonConvert.DeserializeObject<ApiResponse<Location>>(content);

                    if (locationPage?.Data == null || !locationPage.Data.Any())
                    {
                        break;
                    }

                    allLocations.AddRange(locationPage.Data);

                    nextPage = locationPage.Info.Next;
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for 1 hour
                };
                _cache.Set(cacheKey, allLocations, cacheOptions);
            }
            return View(allLocations);
        }

    }
}
