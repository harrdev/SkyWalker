using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WalkerAcademy.Models.WebApiModel;

namespace WalkerAcademy.Controllers
{
    public class DroidController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public DroidController(IHttpClientFactory httpClient, IMemoryCache cache)
        {
            _httpClientFactory = httpClient;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "DroidList";
            if (!_cache.TryGetValue<List<Droid>>(cacheKey, out var allDroids))
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://starwars-databank-server.vercel.app");

                allDroids = new List<Droid>();
                string? nextPage = "/api/v1/droids?page=1";

                while (!string.IsNullOrEmpty(nextPage))
                {
                    var response = await httpClient.GetAsync(nextPage);

                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var droidPage = JsonConvert.DeserializeObject<ApiResponse<Droid>>(content);

                    if (droidPage?.Data == null || !droidPage.Data.Any())
                    {
                        break;
                    }

                    allDroids.AddRange(droidPage.Data);

                    nextPage = droidPage.Info.Next;
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for 1 hour
                };
                _cache.Set(cacheKey, allDroids, cacheOptions);
            }
            return View(allDroids);
        }
    }
}
