using System.Collections.Generic;
using Newtonsoft.Json;

namespace Constants
{
    [JsonObject]
    public class Player
    {
        public string Nickname { get; set; }
        public string PrefabName { get; set; }
        public UnitStats Stats { get; set; }
        public Inventory Inventory { get; set; }
        public List<Item> Units { get; set; }
        public AudioSettings AudioSettings { get; set; }
    }
}