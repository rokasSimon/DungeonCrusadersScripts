using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using DefaultNamespace;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;

    private List<Item> _potionShopItems;
    private List<Item> _weaponsShopItems;
    private List<Item> _unitShopItems;
    private List<GameObject> _itemObjects;

    public void ExitMenu()
    {
        shopUI.SetActive(false);
        _itemObjects.ForEach(Destroy);
    }
    
    public void SaveItems(List<GameObject> items, ShopTypes shopType)
    {
        _itemObjects = items;
        switch (shopType)
        {
            case ShopTypes.PotionShop:
                _potionShopItems = ConvertObjectsToItems(items);
                break;
            case ShopTypes.WeaponShop:
                _weaponsShopItems = ConvertObjectsToItems(items);
                break;
            case ShopTypes.UnitRecruitment:
                _unitShopItems = ConvertObjectsToItems(items);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public List<Item> LoadItems(ShopTypes shopType)
    {
        return shopType switch
        {
            ShopTypes.PotionShop => _potionShopItems,
            ShopTypes.WeaponShop => _weaponsShopItems,
            ShopTypes.UnitRecruitment => _unitShopItems,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static List<Item> ConvertObjectsToItems(IEnumerable<GameObject> gameObjects)
    {
        return gameObjects.Select(item => item.GetComponent<ItemScript>())
            .Select(itemScript => itemScript.Item)
            .ToList();
    }
}