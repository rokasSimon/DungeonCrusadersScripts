using System.Collections.Generic;
using Constants;
using DefaultNamespace;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private MessageDialog messageDialog;
    [SerializeField] private Button buttonSwitch;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject inventoryContentTransform;

    private List<GameObject> _spawnedItems = new();
    private Player _player;
    private TextMeshProUGUI _switchButtonTMP;
    private string _buttonString;
    private const string ItemsInventoryName = "Items";
    private const string UnitsInventoryName = "Units";

    private void Awake()
    {
        _player = Repository.LoadPlayerData();
        _switchButtonTMP = buttonSwitch.GetComponentInChildren<TextMeshProUGUI>();
        _buttonString = _switchButtonTMP.text;
        buttonSwitch.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(_buttonString, UnitsInventoryName);
        LoadInventory();
    }

    public void SwitchViews()
    {
        _spawnedItems = inventoryContentTransform.GetAllChildrenInGameObject();
        string buttonText;
        if (_switchButtonTMP.text.Contains(UnitsInventoryName))
        {
            buttonText = string.Format(_buttonString, ItemsInventoryName);
            DeleteSpawnedObjects();
            LoadUnits();
        }
        else
        {
            buttonText = string.Format(_buttonString, UnitsInventoryName);
            DeleteSpawnedObjects();
            LoadInventory();
        }

        SwitchButtonText(buttonText);
    }

    public void ExitInventory()
    {
        inventoryPanel.SetActive(false);
        DeleteSpawnedObjects();
    }
    
    private void DeleteSpawnedObjects()
    {
        _spawnedItems.ForEach(Destroy);
    }

    private void LoadUnits()
    {
        foreach (var unit in _player.Units)
        {
            var itemObj = Instantiate(itemPrefab, inventoryContentTransform.transform, false);
            SetupItems(itemObj, unit);
        }
    }

    private void LoadInventory()
    {
        foreach (var item in _player.Inventory.Items)
        {
            var itemObj = Instantiate(itemPrefab, inventoryContentTransform.transform, false);
            SetupItems(itemObj, item);
        }
    }

    private void SetupItems<T>(GameObject itemUIObject, T item)
    {
        var itemScript = itemUIObject.GetComponent<ItemScript>();
        itemScript.PlayerData = _player;
        itemScript.SetParameters(messageDialog);
        if (item.GetType() == typeof(Item))
        {
            itemScript.SetItem((Item)(item as object), false);
        }

        if (item.GetType() == typeof(Unit))
        {
            itemScript.SetItem((Unit)(item as object));
        }
    }

    private void SwitchButtonText(string text)
    {
        _switchButtonTMP.text = text;
    }
}