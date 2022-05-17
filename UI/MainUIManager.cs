using System;
using System.Collections;
using System.Linq;
using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Constants.Repository;

public class MainUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerGoldAmountTMP;
    [SerializeField] private TextMeshProUGUI playerUnitAmountTMP;

    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private AudioSettingsManager audioSettingsManager;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject messageDialogPanel;
    [SerializeField] private Dialog dialog;

    [SerializeField] private GameObject exitToDungeonButton;
    [SerializeField] private GameObject exitToTownButton;

    private bool changingScenes;
    [SerializeField] private SceneFader sceneFader;

    private bool IsDungeonScene => SceneManager.GetActiveScene().name == "DungeonScene";
    private bool IsSettingsPanelActive => settingsPanel.activeSelf;

    private Player _player;

    private void Start()
    {
        _player = LoadPlayerData();
        _player.Inventory.PropertyChanged += (_, _) =>
        {
            DisplayPlayerStats();
        };
        
        exitToTownButton.SetActive(IsDungeonScene);
        exitToDungeonButton.SetActive(!IsDungeonScene);

        DisplayPlayerStats();
        
        StartCoroutine(TransitionScene(() => { }, false));
    }

    private void DisplayPlayerStats()
    {
        playerGoldAmountTMP.text = _player.Inventory.Coins.ToString();
        playerUnitAmountTMP.text = _player.Units.Count().ToString();
    }

    public void ToggleMenuPanel()
    {
        settingsPanel.SetActive(!IsSettingsPanelActive);
    }

    public void ToggleOptionsPanel()
    {
        audioSettingsManager.SavePlayerSettings();
        optionsPanel.SetActive(!optionsPanel.activeSelf);
        ToggleMenuPanel();
    }

    public void SaveSettings()
    {
        SavePlayerData(_player);
    }

    public void ExitToMainMenu()
    {
        SaveSettings();
        dialog.SetTitle("Do you really want to exit to the main menu?")
            .SetMessage(
                "You will lose your progress if you exit to the main menu and will have to start from the start");

        ShowMessageDialog(() =>
        {
            dialog.RemoveAllListenersFromButtons();
            dialog.cancelButton.onClick.AddListener(HideMessageDialog);
            dialog.continueButton.onClick.AddListener(SwitchToSceneByName("Scenes/MainMenuScene"));
        });
    }

    public void ExitToDungeon()
    {
        SaveSettings();
        dialog.SetTitle("Are you ready for adventures?")
            .SetMessage("Are you ready for adventures?");
        
        ShowMessageDialog(() =>
        {
            dialog.RemoveAllListenersFromButtons();
            dialog.cancelButton.onClick.AddListener(HideMessageDialog);
            dialog.continueButton.onClick.AddListener(SwitchToSceneByName("Scenes/DungeonScene"));
        });
    }
    
    public void ExitToLobby()
    {
        dialog.SetTitle("Do you really want to exit to the lobby?")
            .SetMessage(
                "You will lose your progress if you exit to the lobby and will have to start from the start");

        ShowMessageDialog(() =>
        {
            dialog.RemoveAllListenersFromButtons();
            dialog.cancelButton.onClick.AddListener(HideMessageDialog);
            dialog.continueButton.onClick.AddListener(SwitchToSceneByName("Scenes/TownScene"));
        });
    }

    private void HideMessageDialog()
    {
        messageDialogPanel.SetActive(false);
    }

    private void ShowMessageDialog(Action action)
    {
        messageDialogPanel.SetActive(true);

        action();
    }

    private UnityAction SwitchToSceneByName(string sceneName) => () =>
    {
        
        
        if (changingScenes)
        {
            return;
        }

        StartCoroutine(TransitionScene(() =>
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }, true));
    };

    private IEnumerator TransitionScene(Action action, bool fadeOut)
    {
        changingScenes = true;

        if (fadeOut)
        {
            yield return sceneFader.FadeOutScene();
        }
        else
        {
            yield return sceneFader.FadeInScene();
        }

        changingScenes = false;

        action();
    }
}