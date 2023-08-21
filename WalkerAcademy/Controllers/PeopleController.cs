using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WalkerAcademy.Models.WebApiModel;

namespace WalkerAcademy.Controllers
{
    public class PeopleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public PeopleController(IHttpClientFactory httpClient, IMemoryCache cache)
        {
            _httpClientFactory = httpClient;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "PeopleList";
            if (!_cache.TryGetValue<List<Person>>(cacheKey, out var allPeople))
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://starwars-databank-server.vercel.app");

                allPeople = new List<Person>();
                string? nextPage = "/api/v1/characters?page=1";

                while (!string.IsNullOrEmpty(nextPage))
                {
                    var response = await httpClient.GetAsync(nextPage);

                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var peoplePage = JsonConvert.DeserializeObject<ApiResponse<Person>>(content);

                    if (peoplePage?.Data == null || !peoplePage.Data.Any())
                    {
                        break;
                    }

                    allPeople.AddRange(peoplePage.Data);

                    nextPage = peoplePage.Info.Next;
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for 1 hour
                };
                _cache.Set(cacheKey, allPeople, cacheOptions);
            }
            return View(allPeople);
        }

    }
}
