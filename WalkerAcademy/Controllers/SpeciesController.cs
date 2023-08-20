using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WalkerAcademy.Models;

namespace WalkerAcademy.Controllers
{
    public class SpeciesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public SpeciesController(IHttpClientFactory httpClient, IMemoryCache cache)
        {
            _httpClientFactory = httpClient;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "SpeciesList";
            if (!_cache.TryGetValue<List<Species>>(cacheKey, out var allSpecies))
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://starwars-databank-server.vercel.app");

                allSpecies = new List<Species>();
                string? nextPage = "/api/v1/species?page=1";

                while (!string.IsNullOrEmpty(nextPage))
                {
                    var response = await httpClient.GetAsync(nextPage);

                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var speciesPage = JsonConvert.DeserializeObject<ApiResponse<Species>>(content);

                    if (speciesPage?.Data == null || !speciesPage.Data.Any())
                    {
                        break;
                    }

                    allSpecies.AddRange(speciesPage.Data);

                    nextPage = speciesPage.Info.Next;
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for 1 hour
                };
                _cache.Set(cacheKey, allSpecies, cacheOptions);
            }
            System.Diagnostics.Debug.WriteLine(allSpecies.Count);
            return View(allSpecies);
        }
    }
}
