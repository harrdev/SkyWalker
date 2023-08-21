using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WalkerAcademy.Models.WebApiModel;

namespace WalkerAcademy.Controllers
{
    public class CreatureController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public CreatureController(IHttpClientFactory httpClient, IMemoryCache cache)
        {
            _httpClientFactory = httpClient;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "CreaturesList";
            if (!_cache.TryGetValue<List<Creature>>(cacheKey, out var allCreatures))
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://starwars-databank-server.vercel.app");

                allCreatures = new List<Creature>();
                string? nextPage = "/api/v1/creatures?page=1";

                while (!string.IsNullOrEmpty(nextPage))
                {
                    var response = await httpClient.GetAsync(nextPage);

                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var creaturePage = JsonConvert.DeserializeObject<ApiResponse<Creature>>(content);

                    if (creaturePage?.Data == null || !creaturePage.Data.Any())
                    {
                        break;
                    }

                    allCreatures.AddRange(creaturePage.Data);

                    nextPage = creaturePage.Info.Next;
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for 1 hour
                };
                _cache.Set(cacheKey, allCreatures, cacheOptions);
            }
            return View(allCreatures);
        }
    }
}
