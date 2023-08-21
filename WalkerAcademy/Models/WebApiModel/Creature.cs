using Newtonsoft.Json;

namespace WalkerAcademy.Models.WebApiModel
{
    public class Creature
    {
        [JsonProperty("_id")]
        public string? Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
