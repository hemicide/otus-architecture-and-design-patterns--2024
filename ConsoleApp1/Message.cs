using Newtonsoft.Json;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;


namespace SpaceBattle
{
    public class Message : IMessage
    {
        [JsonProperty("id")]
        public string GameObjectID { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("properties")]
        public IDictionary<string, object> Properties { get; set; }
    }
}
