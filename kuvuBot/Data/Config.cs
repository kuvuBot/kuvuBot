using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kuvuBot.Data
{
    class Config
    {
        [JsonProperty("token")] public string Token { get; set; }
    }
}
