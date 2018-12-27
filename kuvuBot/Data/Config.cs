﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace kuvuBot.Data
{
    class MySQLConfig
    {
        [JsonProperty("ip")] public string Ip { get; set; }
        [JsonProperty("port")] public string Port { get; set; }
        [JsonProperty("user")] public string User { get; set; }
        [JsonProperty("password")] public string Password { get; set; }
        [JsonProperty("database")] public string Database { get; set; }

    }
    public partial class Apis
    {
        [JsonProperty("cat")] public string Cat { get; set; }
    }

    class Config
    {
        [JsonProperty("token")] public string Token { get; set; }
        [JsonProperty("apis")] public Apis Apis { get; set; }
        [JsonProperty("defualtPrefix")] public string DefualtPrefix { get; set; }
        [JsonProperty("mySql")] public MySQLConfig MySQL { get; set; }
    }
}
