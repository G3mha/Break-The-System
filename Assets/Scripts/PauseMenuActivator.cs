using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuActivator : MonoBehaviour
{
    public InputActionAsset inputActions; // Attach your input actions asset here
    private InputAction pauseAction;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("PauseMenuActivator").Length > 1) {
            Destroy(gameObject);
            return;
        }
        // Don't destroy this object when loading a new scene
        DontDestroyOnLoad(gameObject);

        // Get references to the action maps
        playerActionMap = inputActions.FindActionMap("Player", true);
        uiActionMap = inputActions.FindActionMap("UI", true);

        // Find the "PauseMenu" action in the UI map
        pauseAction = uiActionMap.FindAction("PauseMenu", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += HandlePauseToggle;
    }

    private void OnDisable()
    {
        if (pauseAction == null) return;
        pauseAction.Disable();
        pauseAction.performed -= HandlePauseToggle;
    }

    private void HandlePauseToggle(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0; // Pause the game
        playerActionMap.Disable(); // Disable Player controls
        uiActionMap.Disable(); // Disable UI controls
        Time.timeScale = 0;
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
        playerActionMap.Enable(); // Enable Player controls
        uiActionMap.Enable(); // Enable UI controls
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("PauseMenu");
    }
}
