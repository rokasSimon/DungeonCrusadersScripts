using System;
using System.Collections.Generic;
using Constants;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionMenuManager : MonoBehaviour
{
    private bool _selectionMenu;
    private HeroSelection _selection;
    private GameObject _currentHero;
    private GameObject _prevSpawned;
    private UnitStats _stats;

    [SerializeField] private TextMeshProUGUI nickInput;
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject mainCameraGameObject;
    [SerializeField] private GameObject secondaryCameraGameObject;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI attackPowerValue;
    [SerializeField] private TextMeshProUGUI spellPowerValue;
    [SerializeField] private TextMeshProUGUI maxHealthValue;
    [SerializeField] private TextMeshProUGUI maxManaValue;
    [SerializeField] private TextMeshProUGUI critChanceValue;
    [SerializeField] private Transform heroSpawnLocation;
    
    private void Awake()
    {
        _selection = new HeroSelection();

        _currentHero = _selection.LoadNextHero();
        LoadHero();
    }

    public void LoadNextHero()
    {
        _currentHero = _selection.LoadNextHero();
        LoadHero();
    }

    public void Continue()
    {
        SaveNewPlayer();
        SceneManager.LoadScene("Scenes/TownScene", LoadSceneMode.Single);
    }
    
    public void ExitToMenu()
    {
        secondaryCameraGameObject.SetActive(false);
        mainCameraGameObject.SetActive(true);
        selectionMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void SaveNewPlayer()
    {
        var playerData = new Player()
        {
            Nickname = nickInput.text,
            PrefabName = "Heroes/" + _currentHero.name.Replace("(Clone)", ""),
            Inventory = new Inventory()
            {
                Coins = 100,
                Items = new List<Item>()
            },
            Stats = _stats,
            Units = new List<Item>()
        };

        Repository.SavePlayerData(playerData);
    }
    
    private void LoadHero()
    {
        SpawnHero();
        LoadHeroStats();
    }

    private void LoadHeroStats()
    {
        var unit = _currentHero.GetComponent<Unit>();
        _stats = unit.Stats;

        heroName.text = unit.UnitName;
        attackPowerValue.text = _stats.AttackPower().ToString();
        spellPowerValue.text = _stats.SpellPower().ToString();
        maxHealthValue.text = _stats.MaxHealth().ToString();
        maxManaValue.text = _stats.MaxMana().ToString();
        critChanceValue.text = _stats.AttackCritChance().ToString("P");
    }

    private void SpawnHero()
    {
        if (_prevSpawned is not null)
        {
            Destroy(_prevSpawned);
        }

        _currentHero.transform.localScale = Vector3.one;
        _prevSpawned = Instantiate(_currentHero, heroSpawnLocation);
        _currentHero = _prevSpawned;
    }
}