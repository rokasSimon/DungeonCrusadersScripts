using System;
using System.Collections.Generic;
using Constants;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shops
{
    public class PotionShop : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject shopNameObject;
        [SerializeField] private GameObject shopUI;
        [SerializeField] private TextMeshProUGUI shopUIName;
        [SerializeField] private TextMeshProUGUI playerCoins;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private MessageDialog messageDialog;
        [SerializeField] private GameObject itemsPanel;

        private ShopManager _shopManager;

        private void Awake()
        {
            _shopManager = shopUI.GetComponent<ShopManager>();
        }

        private void ShowMenu()
        {
            SaveShopItems();
            shopUI.SetActive(true);
        }
        
        private void SaveShopItems()
        {
            var items = itemsPanel.GetAllChildrenInGameObject();
            _shopManager.SaveItems(items, ShopTypes.PotionShop);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            new ShopUIBuilder(ShopTypes.PotionShop.ToString(), shopUIName, playerCoins).BuildShopUI(
                _shopManager,
                itemPrefab,
                messageDialog,
                itemsPanel);
            ShowMenu();
        }

        private void OnMouseOver()
        {
            shopNameObject.SetActive(true);
        }

        private void OnMouseExit()
        {
            shopNameObject.SetActive(false);
        }
    }
}