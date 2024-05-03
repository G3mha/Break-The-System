using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuSelector : MonoBehaviour
{
    public TextMeshProUGUI[] menuOptions;
    private int selectedIndex = 2;
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.red;
    public InputActionAsset inputActions;
    private InputActionMap pauseMenuActionMap;
    private InputAction upAction;
    private InputAction downAction;
    private InputAction confirmAction;
    private InputAction cancelAction;
    private InputAction leftAction;
    private InputAction rightAction;
    private InputAction anyKeyAction;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;
    private bool isControlScreenActive = false;
    private int counterControlScreenActive = 0;
    private bool isSoundScreenActive = false;
    private int counterSoundScreenActive = 0;

    private void Awake()
    {
        // Get references to the action map
        pauseMenuActionMap = inputActions.FindActionMap("PauseMenu", true);
        playerActionMap = inputActions.FindActionMap("Player", true);
        uiActionMap = inputActions.FindActionMap("UI", true);

        // Find the "PauseMenu" action in the UI map
        upAction = pauseMenuActionMap.FindAction("Up");
        downAction = pauseMenuActionMap.FindAction("Down");
        confirmAction = pauseMenuActionMap.FindAction("Confirm");
        cancelAction = pauseMenuActionMap.FindAction("Cancel");
        leftAction = pauseMenuActionMap.FindAction("Left");
        rightAction = pauseMenuActionMap.FindAction("Right");
        anyKeyAction = pauseMenuActionMap.FindAction("AnyKey");

        // Set the default color for the menu options
        UpdateMenuSelection();
    }

    private void OnEnable()
    {
        upAction.performed += OnUpPerformed;
        downAction.performed += OnDownPerformed;
        confirmAction.performed += OnConfirmPerformed;
        cancelAction.performed += OnCancelPerformed;
        leftAction.performed += OnLeftPerformed;
        rightAction.performed += OnRightPerformed;
        anyKeyAction.performed += OnAnyButtonPress;
        upAction.Enable();
        downAction.Enable();
        confirmAction.Enable();
        cancelAction.Enable();
        leftAction.Enable();
        rightAction.Enable();
        anyKeyAction.Enable();
    }

    private void OnDisable()
    {
        upAction.performed -= OnUpPerformed;
        downAction.performed -= OnDownPerformed;
        confirmAction.performed -= OnConfirmPerformed;
        cancelAction.performed -= OnCancelPerformed;
        leftAction.performed -= OnLeftPerformed;
        rightAction.performed -= OnRightPerformed;
        anyKeyAction.performed -= OnAnyButtonPress;
        upAction.Disable();
        downAction.Disable();
        confirmAction.Disable();
        cancelAction.Disable();
        leftAction.Disable();
        rightAction.Disable();
        anyKeyAction.Disable();
    }

    private void OnLeftPerformed(InputAction.CallbackContext context)
    {
        if (!isSoundScreenActive) return;
        int selectedSlider = PlayerPrefs.GetInt("AudioConfigSelected");
        string prefKey = selectedSlider == 0 ? "MusicVolume" : "SFXVolume";
        float volume = PlayerPrefs.GetFloat(prefKey);
        volume = Mathf.Max(0, volume - 0.1f);
        PlayerPrefs.SetFloat(prefKey, volume);
        PlayerPrefs.Save();
    }

    private void OnRightPerformed(InputAction.CallbackContext context)
    {
        if (!isSoundScreenActive) return;
        int selectedSlider = PlayerPrefs.GetInt("AudioConfigSelected");
        string prefKey = selectedSlider == 0 ? "MusicVolume" : "SFXVolume";
        float volume = PlayerPrefs.GetFloat(prefKey);
        volume = Mathf.Min(1, volume + 0.1f);
        PlayerPrefs.SetFloat(prefKey, volume);
        PlayerPrefs.Save();
    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (isControlScreenActive) return;
        if (!isSoundScreenActive)
        {
            ResumeGame();
        }
        else if (isSoundScreenActive)
        {
            CloseScreen("SoundScreen");
        }
    }

    private void OnConfirmPerformed(InputAction.CallbackContext context)
    {
        if (isControlScreenActive || isSoundScreenActive) return;
        switch (selectedIndex)
        {
            case 0: // Controls
                ShowControls();
                break;
            case 1: // Sound
                ShowSoundSettings();
                break;
            case 2: // Resume
                ResumeGame();
                break;
            case 3: // Restart Level
                RestartLevel();
                break;
            case 4: // Main Menu
                QuitToMainMenu();
                break;
            default:
                break;
        }
    }

    private void OnUpPerformed(InputAction.CallbackContext context)
    {
        if (isControlScreenActive) return;
        if (!isSoundScreenActive)
        {
            selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : menuOptions.Length - 1;
            UpdateMenuSelection();
        }
        else if (isSoundScreenActive)
        {
            // Toggle slider selection
            int selected = PlayerPrefs.GetInt("AudioConfigSelected");
            PlayerPrefs.SetInt("AudioConfigSelected", (selected + 1) % 2); // Toggle between 0 and 1
            PlayerPrefs.Save();
        }
    }

    private void OnDownPerformed(InputAction.CallbackContext context)
    {
        if (isControlScreenActive) return;
        if (!isSoundScreenActive)
        {
            selectedIndex = (selectedIndex < menuOptions.Length - 1) ? selectedIndex + 1 : 0;
            UpdateMenuSelection();
        }
        else if (isSoundScreenActive)
        {
            // Toggle slider selection
            int selected = PlayerPrefs.GetInt("AudioConfigSelected");
            PlayerPrefs.SetInt("AudioConfigSelected", (selected + 1) % 2); // Toggle between 0 and 1
            PlayerPrefs.Save();
        }
    }

    private void OnAnyButtonPress(InputAction.CallbackContext context)
    {
        if (isSoundScreenActive) return;
        if (isControlScreenActive && counterControlScreenActive == 1)
        {
            CloseScreen("ControlScreen");
        }
        else if (isSoundScreenActive && counterSoundScreenActive == 1)
        {
            CloseScreen("SoundScreen");
        }
        else
        {
            if (isControlScreenActive) counterControlScreenActive++;
            if (isSoundScreenActive) counterSoundScreenActive++;
        }
    }

    private void CloseScreen(string screenName)
    {
        SceneManager.SetActiveScene(SceneManager.GetActiveScene());
        SceneManager.UnloadSceneAsync(screenName);
        if (screenName == "ControlScreen") {
            isControlScreenActive = false;
            counterControlScreenActive = 0;
        } else if (screenName == "SoundScreen") {
            isSoundScreenActive = false;
            counterSoundScreenActive = 0;
        }
    }

    public void MoveSelection(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input.y > 0)
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
        }
        else if (input.y < 0)
        {
            selectedIndex = Mathf.Min(menuOptions.Length - 1, selectedIndex + 1);
        }
        UpdateMenuSelection();
    }

    private void UpdateMenuSelection()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].color = i == selectedIndex ? selectedColor : defaultColor;
        }
    }

    private void ShowControls()
    {
        SceneManager.LoadScene("ControlScreen", LoadSceneMode.Additive);
        isControlScreenActive = true;
    }

    private void ShowSoundSettings()
    {
        SceneManager.LoadScene("SoundScreen", LoadSceneMode.Additive);
        isSoundScreenActive = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
        playerActionMap.Enable(); // Enable Player controls
        uiActionMap.Enable(); // Enable UI controls
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("PauseMenu");
    }

    private void RestartLevel()
    {
        // Ensure the game is not paused
        Time.timeScale = 1;

        // Re-enable player and UI controls
        playerActionMap.Enable();
        uiActionMap.Enable();

        // Directly reload the current scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void OnDestroy() {
        if (upAction != null) {
            upAction.performed -= OnUpPerformed;
            upAction.Disable();    
        }
        
        if (downAction != null) {
            downAction.performed -= OnDownPerformed;
            downAction.Disable();
        } 

        if (confirmAction != null) {
            confirmAction.performed -= OnConfirmPerformed;
            confirmAction.Disable();    
        }

        if (cancelAction != null) {
            cancelAction.performed -= OnCancelPerformed;
            cancelAction.Disable();
        }

        if (leftAction != null) {
            leftAction.performed -= OnLeftPerformed;
            leftAction.Disable();    
        }

        if (rightAction != null) {
            rightAction.performed -= OnRightPerformed;
            rightAction.Disable();    
        }

        if (anyKeyAction != null) {
            anyKeyAction.performed -= OnAnyButtonPress;
            anyKeyAction.Disable();
        }
    }

    private void QuitToMainMenu()
    {
        // Ensure the game is not paused
        Time.timeScale = 1;

        // Re-enable player and UI controls
        playerActionMap.Enable();
        uiActionMap.Enable();
        
        // Directly reload the current scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("StartScreen");
    }
}
