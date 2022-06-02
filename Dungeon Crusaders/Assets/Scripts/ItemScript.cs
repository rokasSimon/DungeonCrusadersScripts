using System.ComponentModel;
using System.Text;
using Constants;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ItemScript : MonoBehaviour
    {
        public Player PlayerData;

        [SerializeField] public TextMeshProUGUI displayPlayerCoinsTMP;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemQuantity;
        [SerializeField] private Button itemButton;
        [SerializeField] private MessageDialog messageDialog;

        public Item Item;

        private void Awake()
        {
            if (PlayerData != null)
                PlayerData.Inventory.PropertyChanged += InventoryOnPropertyChanged;
        }

        private void InventoryOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(PlayerData.Inventory.Coins)))
            {
                displayPlayerCoinsTMP.text = PlayerData.Inventory.Coins.ToString();
            }
        }

        public void SetItem(Item item, bool shop = true)
        {
            Item = item;

            itemName.text = item.Name;
            itemQuantity.text = item.Quantity.ToString();
            if (shop)
            {
                SetupItemButtonForShop(item);
                return;
            }

            SetupItemButtonForInventory(item);
        }

        public void SetItem(Unit unit)
        {
            itemName.text = unit.DisplayName;
            itemQuantity.text = "1";
        }
        
        private void SetupItemButtonForInventory(Item item)
        {
            itemButton.GetComponentInChildren<TextMeshProUGUI>().text = "View item";
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() =>
            {
                messageDialog.dialog.title.text = item.Name;
                messageDialog.dialog.message.text = new StringBuilder()
                    .AppendLine($"Information about {item.Name}")
                    .AppendLine("- Add things")
                    .ToString();
                messageDialog.dialog.cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Exit";
                messageDialog.dialog.cancelButton.onClick.AddListener(() =>
                {
                    messageDialog.gameObject.SetActive(false);
                });
                messageDialog.dialog.continueButton.gameObject.SetActive(false);

                messageDialog.gameObject.SetActive(true);
            });
        }

        private void SetupItemButtonForShop(Item item)
        {
            itemButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy for: {item.Price.ToString()}";
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() =>
            {
                messageDialog.dialog
                    .SetTitle("Are you sure?")
                    .SetMessage($"Are you sure you want to buy {item.Name} for {item.Price.ToString()} gold?")
                    .RemoveAllListenersFromButtons();

                messageDialog.dialog.cancelButton.onClick.AddListener(() =>
                {
                    messageDialog.gameObject.SetActive(false);
                });
                messageDialog.dialog.continueButton.onClick.AddListener(() =>
                {
                    BuyItem(item);
                    messageDialog.gameObject.SetActive(false);
                });

                messageDialog.gameObject.SetActive(true);
            });
        }

        private void BuyItem(Item item)
        {
            var itemClone = item.Clone();
            PlayerData.Inventory.Coins -= item.Price;
            if (item.Name.Contains("Unit"))
            {
                PlayerData.Units.Add(item);
            }
            PlayerData.Inventory.AddItem(itemClone);
            item.Quantity--;

            Repository.SavePlayerData(PlayerData);
            SetItem(item);
        }

        public void SetParameters(MessageDialog msgDialog)
        {
            messageDialog = msgDialog;
        }
    }
}