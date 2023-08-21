using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WalkerAcademy.Models.WebApiModel;

namespace WalkerAcademy.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public OrganizationController(IHttpClientFactory httpClient, IMemoryCache cache)
        {
            _httpClientFactory = httpClient;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "OrganizationsList";
            if (!_cache.TryGetValue<List<Organization>>(cacheKey, out var allOrganizations))
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://starwars-databank-server.vercel.app");

                allOrganizations = new List<Organization>();
                string? nextPage = "/api/v1/organizations?page=1";

                while (!string.IsNullOrEmpty(nextPage))
                {
                    var response = await httpClient.GetAsync(nextPage);

                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var organizationPage = JsonConvert.DeserializeObject<ApiResponse<Organization>>(content);

                    if (organizationPage?.Data == null || !organizationPage.Data.Any())
                    {
                        break;
                    }

                    allOrganizations.AddRange(organizationPage.Data);

                    nextPage = organizationPage.Info.Next;
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for 1 hour
                };
                _cache.Set(cacheKey, allOrganizations, cacheOptions);
            }
            return View(allOrganizations);
        }
    }
}
