using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class WelcomeScreen : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The UI Document for the Welcome Screen")]
    private UIDocument userInterfaceDocument = null;

    [SerializeField]
    [Tooltip("The scene that should be loaded")]
    private string scene = null;

    // Used to hide and display the Security Keypad
    private VisualElement welcomeScreenRootVisualElement = null;

    private UnityEngine.UIElements.Button btnNewGame = null;

    private UnityEngine.UIElements.Button btnExitGame = null;

    private Label lblLoading = null;
 
    // Start is called before the first frame update
    void Start()
    {
        // Remember the Root Visual Element of the WelcomeScreen
        welcomeScreenRootVisualElement = userInterfaceDocument.rootVisualElement;
        if (welcomeScreenRootVisualElement is null)
        {
            GameLog.ErrorMessage(this, "No Root Visual Element found in the Welcome Screen user interface document.");
            return;
        }

        // Get the Root Frame of the Welcome Screen
        VisualElement WelcomScreenAppRootFrame = welcomeScreenRootVisualElement.Q<VisualElement>("RootFrame");
        if (WelcomScreenAppRootFrame is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Visual Element called 'RootFrame' in the Welcome Screen user interface document.");
            return;
        }

        // The button to start the game
        btnNewGame = WelcomScreenAppRootFrame.Q<Button>("btnNewGame");
        if (btnNewGame is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Button  called 'btnStartGame' in the Welcome Screen user interface document.");
            return;
        }
        else
        {
            btnNewGame.RegisterCallback<ClickEvent>(ev => HandleNewGameClick());
        }

        // The button to start the game
        btnExitGame = WelcomScreenAppRootFrame.Q<Button>("btnExitGame");
        if (btnExitGame is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Button  called 'btnExitGame' in the Welcome Screen user interface document.");
            return;
        }
        else
        {
            btnExitGame.RegisterCallback<ClickEvent>(ev => HandleExitGameClick());
        }

        // The button to start the game
        lblLoading = WelcomScreenAppRootFrame.Q<Label>("lblLoading");
        if (lblLoading is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Button  called 'btnExitGame' in the Welcome Screen user interface document.");
            return;
        }
        else
        {
            lblLoading.visible = false;
        }
    }

    /// <summary>
    /// Respond to the START GAME button being clicked
    /// </summary>
    private void HandleNewGameClick()
    {
        // Use a coroutine to load the Scene in the background
        lblLoading.visible = true;
        StartCoroutine(LoadScene());
    }

    /// <summary>
    /// Respond to the START GAME button being clicked
    /// </summary>
    private void HandleExitGameClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    IEnumerator LoadScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
