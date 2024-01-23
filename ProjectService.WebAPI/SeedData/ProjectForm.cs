using System;
using Newtonsoft.Json;

namespace ProjectService.WebAPI.SeedData
{
    public class ProjectForm
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }

        [JsonProperty("addedDate")]
        public DateTime AddedDate { get; set; }
    }
}
