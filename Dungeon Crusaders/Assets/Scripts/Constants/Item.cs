using Newtonsoft.Json;

namespace Constants
{
    [JsonObject]
    public class Item
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }

        public Item Clone()
        {
            return (Item) MemberwiseClone();
        }
    }
}