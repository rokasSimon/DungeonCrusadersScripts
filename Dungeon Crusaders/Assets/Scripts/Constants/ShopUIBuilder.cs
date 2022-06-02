using System;
using System.Text.RegularExpressions;
using DefaultNamespace;
using Extensions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Constants
{
    public class ShopUIBuilder
    {
        private readonly ShopTypes _shopType;
        private readonly string _shopName;
        private readonly TextMeshProUGUI _shopNameTMP;
        private readonly TextMeshProUGUI _playerCoinsTMP;
        
        private int _playerCoins;

        public ShopUIBuilder(string shopName, TextMeshProUGUI shopNameTMP, TextMeshProUGUI playerCoinsTMP)
        {
            switch (shopName)
            {
                case nameof(ShopTypes.PotionShop):
                    _shopName = shopName.CamelCaseToNormal();
                    _shopType = ShopTypes.PotionShop;
                    break;
                case nameof(ShopTypes.UnitRecruitment):
                    _shopName = shopName.CamelCaseToNormal();
                    _shopType = ShopTypes.UnitRecruitment;
                    break;
                case nameof(ShopTypes.WeaponShop):
                    _shopName = shopName.CamelCaseToNormal();
                    _shopType = ShopTypes.WeaponShop;
                    break;
                default:
                    throw new NotSupportedException($"The shop with the name '{shopName}' is not supported");
            }

            _shopNameTMP = shopNameTMP;
            _playerCoinsTMP = playerCoinsTMP;
        }

        public void BuildShopUI(ShopManager shopManager, GameObject itemPrefab, MessageDialog messageDialog, GameObject parent)
        {
            _shopNameTMP.text = _shopName;
            var playerData = Repository.LoadPlayerData();
            var loadShopData = ShopItemLoader.LoadShopData(_shopType, _shopName, shopManager);

            _playerCoins = playerData.Inventory.Coins;
            _playerCoinsTMP.text = _playerCoins.ToString();

            foreach (var item in loadShopData.Items)
            {
                var itemUIObject = Object.Instantiate(itemPrefab, parent.transform, false);
                SetupItems(messageDialog, itemUIObject, playerData, item);
            }
        }

        private void SetupItems(MessageDialog messageDialog, GameObject itemUIObject, Player playerData, Item item)
        {
            var itemScript = itemUIObject.GetComponent<ItemScript>();
            itemScript.PlayerData = playerData;
            itemScript.displayPlayerCoinsTMP = _playerCoinsTMP;
            itemScript.SetParameters(messageDialog);
            itemScript.SetItem(item);
        }
    }

    public enum ShopTypes
    {
        PotionShop,
        WeaponShop,
        UnitRecruitment
    }
}