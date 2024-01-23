using System;
using Newtonsoft.Json;

namespace ProjectService.WebAPI.SeedData
{
    public class UserForm
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("projectId")]
        public int ProjectId { get; set; }

        [JsonProperty("addedDate")]
        public DateTime AddedDate { get; set; }
    }
}
