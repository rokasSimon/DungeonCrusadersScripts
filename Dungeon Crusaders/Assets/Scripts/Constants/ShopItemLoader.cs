using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Constants
{
    public static class ShopItemLoader
    {
        public static Shop LoadShopData(ShopTypes shopType, string shopName, ShopManager shopManager)
        {
            return new Shop(shopType, shopName, GenerateRandomItems(shopType, shopManager));
        }

        private static List<Item> GenerateRandomItems(ShopTypes shopType, ShopManager shopManager)
        {
            var items = shopManager.LoadItems(shopType) ?? new List<Item>();

            if (items.Any())
            {
                return items;
            }
            
            var name = shopType switch
            {
                ShopTypes.PotionShop => "Potion #{0}",
                ShopTypes.UnitRecruitment => "Unit #{0}",
                ShopTypes.WeaponShop => "Weapon #{0}",
                _ => throw new ArgumentOutOfRangeException(nameof(shopType), shopType, null)
            };

            for (var i = 0; i < Random.Range(1, 50); i++)
            {
                items.Add(new Item
                {
                    Name = string.Format(name, i),
                    Price = Random.Range(1, 100),
                    Quantity = Random.Range(0, 100)
                });
            }

            return items;
        }
    }

    public class Shop
    {
        public ShopTypes ShopType { get; }
        public string Name { get; }
        public List<Item> Items { get; }

        public Shop(ShopTypes shopType, string shopName, List<Item> items)
        {
            ShopType = shopType;
            Name = shopName;
            Items = items;
        }
    }
}