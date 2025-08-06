using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Provides functionality for displaying game messages to the player, including speaker icons, message text, and
/// dismiss options.
/// </summary>
/// <remarks>This class manages the user interface elements required to show game messages, such as speaker icons,
/// message titles, and message text. It also handles interactions like dismissing the message and restoring the
/// player's state. Use the <see cref="ShowGameMessage(Texture2D, string)"/> method to display a game message.</remarks>
public class DisplayGameMessage : Singleton<DisplayGameMessage>
{
    // These constants are used to find the specific UI elements in the UIDocument.
    private const string RootFrameName = "RootFrame";
    private const string SpeakerIconTextureName = "SpeakerIconTexture";
    private const string SpeakerAndMessageTitleName = "SpeakerAndMessageTitle";
    private const string MessageName = "Message";
    private const string DismissButtonName = "DismissButton";

    private VisualElement rootVisualElement = null;
    private VisualElement docSpeakerIconTexture = null;
    private Label docSpeakerAndMessageTitle = null;
    private Label docMessage = null;
    private Button docDismissButton = null;
    private EventCallback<ClickEvent> dismissButtonCallback;

    void Start()
    {
        var globals = Globals.Instance;
        HookUpUIElements(globals);
        RegisterButtonCallbacks();
        HideGameMessageUI(globals);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = false;
    }

    protected override void OnDestroy()
    {
        UnregisterButtonCallback();
    }

    public void ShowGameMessage(Texture2D speakerIconTexture, string gameMessageTitle)
    {
        var globals = Globals.Instance;

        if (Instance == null)
        {
            Debug.LogError("DisplayGameMessage instance is not initialized.");
            return;
        }

        if (!Instance.TryGetGameMessage(gameMessageTitle, out GameMessage message))
        {
            Debug.LogError($"Game message '{gameMessageTitle}' not found.");
            return;
        }

        GameMessages.Instance.SetShown(gameMessageTitle);

        var player = globals.Player;
        if (player != null)
        {
            player.SetActive(false);
        }

        Instance.docSpeakerIconTexture.style.backgroundImage = new StyleBackground(speakerIconTexture);
        Instance.docMessage.text = message.MessageText;
        Instance.docSpeakerAndMessageTitle.text = $"{speakerIconTexture.name}: {gameMessageTitle}";

        var gameMessageDocument = globals.PlayerInteraction.GameMessageDocument;
        if (gameMessageDocument != null && gameMessageDocument.rootVisualElement != null)
        {
            UnityEngine.Cursor.visible = true;

            gameMessageDocument.rootVisualElement.visible = true;
        }
    }

    #region Internal Support Methods

    // Hook up the UI elements from the UIDocument to the script's fields.
    private void HookUpUIElements(Globals globals)
    {
        var playerInteraction = globals?.PlayerInteraction;
        var gameMessageDocument = playerInteraction?.GameMessageDocument;
        rootVisualElement = gameMessageDocument?.rootVisualElement;
        if (rootVisualElement == null)
        {
            Debug.Log("No Root Visual Element found in the Game Message Document.");
            return;
        }

        var docRootFrame = rootVisualElement.Q<VisualElement>(RootFrameName);
        if (docRootFrame == null)
        {
            Debug.Log($"Unable to find a Visual Element called '{RootFrameName}' in the Game Message user interface document.");
            return;
        }

        docSpeakerIconTexture = rootVisualElement.Q<VisualElement>(SpeakerIconTextureName);
        if (docSpeakerIconTexture == null)
        {
            Debug.Log($"Unable to find a Visual Element called '{SpeakerIconTextureName}' in the Game Message user interface document.");
            return;
        }

        docSpeakerAndMessageTitle = docRootFrame.Q<Label>(SpeakerAndMessageTitleName);
        if (docSpeakerAndMessageTitle == null)
        {
            Debug.Log($"Unable to find a Label called '{SpeakerAndMessageTitleName}' in the Game Message user interface document.");
            return;
        }

        docMessage = docRootFrame.Q<Label>(MessageName);
        if (docMessage == null)
        {
            Debug.Log($"Unable to find a Label called '{MessageName}' in the Game Message user interface document.");
            return;
        }

        docDismissButton = docRootFrame.Q<Button>(DismissButtonName);
        if (docDismissButton == null)
        {
            Debug.Log($"Unable to find a Button called '{DismissButtonName}' in the Game Message user interface document.");
            return;
        }
    }

    // Hide the game message UI by making it invisible.
    private void HideGameMessageUI(Globals globals)
    {
        var gameMessageDocument = globals.PlayerInteraction.GameMessageDocument;
        if (gameMessageDocument?.rootVisualElement != null)
        {
            gameMessageDocument.rootVisualElement.visible = false;
        }
    }

    // Registers the callback for the dismiss button in the game message UI.
    private void RegisterButtonCallbacks()
    {
        if (docDismissButton != null)
        {
            dismissButtonCallback = ev => HandleDismissClick();
            docDismissButton.RegisterCallback(dismissButtonCallback);
        }
    }

    // Unregisters the callback for the dismiss button to prevent memory leaks.
    private void UnregisterButtonCallback()
    {
        if (docDismissButton != null && dismissButtonCallback != null)
        {
            docDismissButton.UnregisterCallback(dismissButtonCallback);
        }
    }

    // Try to get the game message by its title.
    private bool TryGetGameMessage(string title, out GameMessage message)
    {
        message = null;
        var allMessages = GameMessages.Instance?.AllGameMessages;
        if (allMessages == null)
        {
            Debug.Log("GameMessages.Instance or AllGameMessages is null.");
            return false;
        }
        if (!allMessages.TryGetValue(title, out message))
        {
            Debug.Log($"Game message '{title}' not found.");
            return false;
        }
        return true;
    }

    // Handle the dismiss button click event to hide the message and restore the player state.
    private void HandleDismissClick()
    {
        var globals = Globals.Instance;
        var gameMessageDocument = globals.PlayerInteraction.GameMessageDocument;
        if (gameMessageDocument?.rootVisualElement != null)
        {
            gameMessageDocument.rootVisualElement.visible = false;
        }

        UnityEngine.Cursor.visible = false;

        var currentAudioSource = globals.CurrentAudioSource;
        if (currentAudioSource != null && currentAudioSource.isPlaying)
        {
            currentAudioSource.Stop();
        }

        var player = globals.Player;
        if (player != null)
        {
            player.SetActive(true);
        }
    }
    #endregion
}