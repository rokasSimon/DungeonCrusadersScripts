using System.Collections.Generic;
using System.Linq;
using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitController : MonoBehaviour
{
    [SerializeField] private MessageDialog messageDialog;

    private LevelManager _levelManager;
    private UnitLocationController _grid;
    private List<Vec2> _exitTiles;
    private bool _levelFinished;
    private bool _levelLost;

    private void Awake()
    {
        _levelManager = FindObjectOfType<LevelManager>();
    }

    public void Init(UnitLocationController grid)
    {
        _grid = grid;
        _exitTiles = _grid.FirstOrDefault(TileTypes.Exit)?.Range(1);
        _levelFinished = false;
    }

    public void CheckForLevelFinish()
    {
        _levelFinished = _grid.Units.All(unit => unit.Team == UnitTeam.Player);
        _levelLost = _grid.Units.All(unit => unit.Team == UnitTeam.AI);
    }

    public void CheckForHeroInRange()
    {
        if (_levelFinished)
        {
            foreach (var pos in _exitTiles)
            {
                var unit = _grid.UnitAt(pos);

                if (unit != null && unit.Team == UnitTeam.Player)
                {
                    foreach (var unitTag in unit.UnitTags)
                    {
                        if (unitTag == UnitTag.Hero)
                        {
                            _levelManager.UnitManager.GetComponent<UnitManager>().LevelEnded = true;
                            CreateWinConditionPopUp();
                            return;
                        }
                    }
                }
            }
        }

        if (_levelLost)
        {
            _levelManager.UnitManager.GetComponent<UnitManager>().LevelEnded = true;
            CreateLoseConditionPopUp();
        }
    }

    private void CreateWinConditionPopUp()
    {
        var rewardGold = Random.Range(1, 120);
        var playerData = Repository.LoadPlayerData();
        playerData.Inventory.Coins += rewardGold;
        Repository.SavePlayerData(playerData);
        messageDialog.dialog = messageDialog.dialog
            .SetTitle($"Congratulations! You beat level {_levelManager.currentLevel.ToString()}")
            .SetMessage($"Your reward: {rewardGold} gold!");
        messageDialog.dialog.cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Leave dungeons";
        messageDialog.dialog.cancelButton.GetComponentInChildren<TextMeshProUGUI>().autoSizeTextContainer = true;
        messageDialog.dialog.RemoveAllListenersFromButtons();
        messageDialog.dialog.cancelButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Scenes/TownScene", LoadSceneMode.Single);
            messageDialog.gameObject.SetActive(false);
        });
        messageDialog.dialog.continueButton.onClick.AddListener(() =>
        {
            _levelManager.NewLevel();
            messageDialog.gameObject.SetActive(false);
        });

        messageDialog.gameObject.SetActive(true);
    }

    private void CreateLoseConditionPopUp()
    {
        var goldLoss = Random.Range(1, 60);
        var playerData = Repository.LoadPlayerData();
        playerData.Inventory.Coins -= goldLoss;
        Repository.SavePlayerData(playerData);
        messageDialog.dialog = messageDialog.dialog
            .SetTitle($"Bad luck! You lost at level {_levelManager.currentLevel.ToString()}")
            .SetMessage($"You lost: {goldLoss} gold!");
        messageDialog.dialog.continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Restart adventure";
        messageDialog.dialog.continueButton.GetComponentInChildren<TextMeshProUGUI>().autoSizeTextContainer = true;
        messageDialog.dialog.cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Leave dungeons";
        messageDialog.dialog.cancelButton.GetComponentInChildren<TextMeshProUGUI>().autoSizeTextContainer = true;
        messageDialog.dialog.RemoveAllListenersFromButtons();
        messageDialog.dialog.cancelButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Scenes/TownScene", LoadSceneMode.Single);
            messageDialog.gameObject.SetActive(false);
        });
        messageDialog.dialog.continueButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Scenes/DungeonScene", LoadSceneMode.Single);
            messageDialog.gameObject.SetActive(false);
        });

        messageDialog.gameObject.SetActive(true);
    }
}