using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Claims;
using WalkerAcademy.DBContext;
using WalkerAcademy.Models.Entities;
using WalkerAcademy.Models.WebApiModel;

namespace WalkerAcademy.Controllers
{
    public class DroidController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;

        public DroidController(IHttpClientFactory httpClient, IMemoryCache cache, ApplicationDbContext context)
        {
            _httpClientFactory = httpClient;
            _cache = cache;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "DroidList";
            List<Droid> allDroids;

            if (!_cache.TryGetValue<List<Droid>>(cacheKey, out allDroids))
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

            // Fetch droids created by the user from database
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDroids = _context.Droids
                .Where(d => d.UserId == userId)
                .Select(d => d.ToApiModel())
                .ToList();

            // Merge the two lists
            allDroids.AddRange(userDroids);

            return View(allDroids);
        }

        [HttpPost]
        public IActionResult CreateDroid(Droid droid)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var droidEntity = droid.ToEntity(userId);
                _context.Droids.Add(droidEntity);
                _context.SaveChanges();

                // Add the droid to the cache if it exists
                const string cacheKey = "DroidList";
                if (_cache.TryGetValue<List<Droid>>(cacheKey, out var allDroids))
                {
                    allDroids.Add(droid);

                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Cache for 1 hour
                    };
                    _cache.Set(cacheKey, allDroids, cacheOptions); // Update the cache with the new list
                }

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }



    }
}
