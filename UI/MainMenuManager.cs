using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Constants.Repository;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainCameraGameObject;
    public GameObject secondaryCameraGameObject;
    public GameObject mainMenuObject;
    public GameObject selectionMenuObject;
    public GameObject continueButtonObject;
    public GameObject startNewButtonObject;
    public GameObject optionsButtonObject;
    public GameObject exitButtonObject;

    private bool _playerDataExist;
    private bool _playerOptionsExist;

    private Button _continueButton;
    private Button _startNewButton;
    private Button _optionsButton;
    private Button _exitButton;

    private void Awake()
    {
        Setup();
        RegisterButtons();
    }

    private void RegisterButtons()
    {
        _continueButton.onClick.AddListener(ContinueProgress);
        _startNewButton.onClick.AddListener(StartNewProgress);
        _optionsButton.onClick.AddListener(OpenOptions);
        _exitButton.onClick.AddListener(ExitTheGame);
    }

    private static void ExitTheGame() => Application.Quit();

    private static void OpenOptions() => Debug.Log("Options clicked");

    private void StartNewProgress()
    {
        mainCameraGameObject.SetActive(false);
        secondaryCameraGameObject.SetActive(true);
        mainMenuObject.SetActive(false);
        selectionMenuObject.SetActive(true);
    }

    private static void ContinueProgress() => SceneManager.LoadScene("Scenes/TownScene", LoadSceneMode.Single);

    private void Setup()
    {
        _playerDataExist = PlayerDataExists();
        _playerOptionsExist = PlayerOptionsExists();
        continueButtonObject.SetActive(_playerDataExist);

        _continueButton = continueButtonObject.GetComponent<Button>();
        _startNewButton = startNewButtonObject.GetComponent<Button>();
        _optionsButton = optionsButtonObject.GetComponent<Button>();
        _exitButton = exitButtonObject.GetComponent<Button>();
    }
}