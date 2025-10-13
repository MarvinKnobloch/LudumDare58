using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    private Controls controls;

    private GameObject currentOpenMenu;
    [NonSerialized] public bool gameIsPaused;

    [SerializeField] private GameObject menu;

    [SerializeField] private GameObject confirmController;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI confirmText;

    [SerializeField] private BuildTowerButton towerPreview;

    private float normalFixedDeltaTime;

    [Space] public SceneType sceneType;
    public enum SceneType
    {
        Menu,
        Ingame,
    }

    private void Awake()
    {
        controls = new Controls();
        normalFixedDeltaTime = Time.fixedDeltaTime;
    }
    private void Start()
    {
        if(IngameController.Instance != null)  IngameController.Instance.DeactivateCursor();
    }

    void Update()
    {
        if (controls.Menu.MenuEsc.WasPerformedThisFrame())
        {
            HandleMenu();
        }
    }
    private void OnEnable()
    {
        controls.Enable();
        controls.Menu.MenuEsc.performed += MenuHotkey;
    }
    private void OnDisable()
    {
        controls.Menu.MenuEsc.performed -= MenuHotkey;
        controls.Disable();
    }
    private void MenuHotkey(InputAction.CallbackContext context)
    {
        HandleMenu();
    }
    public void HandleMenu()
    {
        if (sceneType == SceneType.Menu)
        {
            if (confirmController.activeSelf == true) confirmController.SetActive(false);
            else if (menu.activeSelf == true) return;
            else CloseSelectedMenu();
        }
        else if (sceneType == SceneType.Ingame)
        {
            if (IngameController.Instance.gameOver == true) return;
            if (towerPreview.TowerPreviewActive() == true) return;
            if (IngameController.Instance.playerUI.dialogBox.activeSelf == true) return;
            if (confirmController.activeSelf == true) confirmController.SetActive(false);
            else if (menu.activeSelf == false)
            {
                if (gameIsPaused == false)
                {
                    PauseGame();
                    menu.SetActive(true);

                }
                else CloseSelectedMenu();
            }
            else
            {
                menu.SetActive(false);
                EndPause();
            }
        }
    }

    public void OpenSelection(GameObject currentMenu)
    {
        {
            currentOpenMenu = currentMenu;
            currentMenu.SetActive(true);

            menu.SetActive(false);

            PlayMenuSound();
        }
    }

    public void ResumeGame()
    {
        menu.SetActive(false);
        EndPause();
    }
    public void SetRestartGameConfirm()
    {
        OpenConfirmController(RestartGame, "Restart Game");
    }

    public void SetBackToMainMenuConfirm()
    {
        OpenConfirmController(BackToMainMenu, "Back to main menu?");
    }
    public void NewGame()
    {
        PlayerPrefs.SetInt("Tutorial", 1);

        PlayMenuSound();
        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;
        SceneManager.LoadScene(1);
    }
    public void RestartGame()
    {
        GameManager.Instance.showIntro = false;
        GameManager.Instance.showTutorial = false;
        EndPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void BackToMainMenu()
    {
        PlayMenuSound();
        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;
        SceneManager.LoadScene(0);
    }
    public void CloseSelectedMenu()
    {
        if (currentOpenMenu != null)
        {
            currentOpenMenu.SetActive(false);
            currentOpenMenu = null; // Clear previous menu after returning
            menu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No previous menu to return to. Going back to inGameMenu.");
            menu.SetActive(true);
        }
        PlayMenuSound();
    }

    private void PauseGame()
    {
        IngameController.Instance.ActivateCursor();

        gameIsPaused = true;
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;

        PlayMenuSound();
    }
    public void EndPause()
    {
        IngameController.Instance.DeactivateCursor();

        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;

        PlayMenuSound();
    }
    private void OpenConfirmController(UnityAction buttonEvent, string text)
    {

        confirmText.text = text;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => buttonEvent());
        confirmController.SetActive(true);

        PlayMenuSound();
    }
    public void CloseConfirmSelection()
    {
        confirmController.SetActive(false);

        PlayMenuSound();
    }
    public void TimeScaleToZero()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;
    }
    public void ResetTimeScale()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;
    }

    private void PlayMenuSound()
    {
        //AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilitySounds[(int)AudioManager.UtilitySounds.MenuSelect]);
    }
}
