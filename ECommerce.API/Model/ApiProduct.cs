﻿using Newtonsoft.Json;
using System;

namespace ECommerce.API.Model
{
    public class ApiProduct
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("available")]
        public bool Available { get; set; }
    }
}
