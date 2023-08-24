using Newtonsoft.Json;
using WalkerAcademy.Models.Entities;

namespace WalkerAcademy.Models.WebApiModel
{
    public class Droid
    {
        [JsonProperty("_id")]
        public string? Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        public DroidEntity ToEntity(string userId)
        {
            return new DroidEntity
            {
                Name = this.Name,
                Description = this.Description,
                Image = this.Image,
                UserId = userId
            };
        }
    }
}
