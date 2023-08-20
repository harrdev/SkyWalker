﻿using Newtonsoft.Json;

namespace WalkerAcademy.Models
{
    public class Droid
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}