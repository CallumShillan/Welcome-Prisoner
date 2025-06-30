using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;
using Invector.vCharacterController;

/// <summary>
/// Manages the display of in-game messages, including speaker icons, message text, and dismiss functionality.
/// </summary>
/// <remarks>This class is responsible for showing and hiding game messages in the UI, as well as handling user
/// interactions such as dismissing the message. It integrates with Unity's UI Toolkit and requires specific UI elements
/// to be defined in the associated <see cref="UIDocument"/>. The class also interacts with the player GameObject and
/// global game state to manage visibility and behavior during message display.  Ensure that all required fields are set
/// in the Unity Editor, including the player GameObject, speaker icon texture, game message title, and the UI document used to display the game message.
/// Missing or improperly configured fields will result in warnings in the Unity console.</remarks>
public class DisplayGameMessage : MonoBehaviour
{
    // These constants are used to find the specific UI elements in the UIDocument.
    private const string RootFrameName = "RootFrame";
    private const string SpeakerIconTextureName = "SpeakerIconTexture";
    private const string SpeakerAndMessageTitleName = "SpeakerAndMessageTitle";
    private const string MessageName = "Message";
    private const string DismissButtonName = "DismissButton";

    // The player GameObject that will be deactivated when the message is shown and reactivated when dismissed.
    [SerializeField, Tooltip("The player's Game Object")]
    private GameObject player = null;

    // The texture used to represent the speaker's icon in the game message.
    [SerializeField, Tooltip("The speaker's face icon texture")]
    private Texture2D speakerIconTexture;

    // The title of the game message that will be displayed.
    [SerializeField, Tooltip("The game message title")]
    private string gameMessageTitle = string.Empty;

    // Whether the game message should be shown when the player collides with the trigger.
    [SerializeField, Tooltip("Whether the message should be displayed")]
    private bool shouldBeShown = true;

    // The UIDocument that contains the UI elements for displaying the game message.
    [SerializeField, Tooltip("The UI Document to display the game message")]
    private UIDocument gameMessageDocument = null;

    private VisualElement rootVisualElement = null;
    private VisualElement docSpeakerIconTexture = null;
    private Label docSpeakerAndMessageTitle = null;
    private Label docMessage = null;
    private Button docDismissButton = null;
    private GameMessage theGameMessage = null;
    private EventCallback<ClickEvent> dismissButtonCallback;

    #region Unity Invoked Methods

    private void Awake()
    {
        if (!ValidateInspectorFields())
        {
            return;
        }

        HookUpUIElements();
        HideGameMessageUI();
        RegisterButtonCallbacks();
    }

    private void OnDestroy()
    {
        UnregisterButtonCallback();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!shouldBeShown)
        {
            return;
        }

        UnityEngine.Cursor.visible = true;
        if (player != null)
        {
            player.SetActive(false);
        }

        shouldBeShown = false;

        if (!TryGetGameMessage(gameMessageTitle, out theGameMessage))
        {
            return;
        }

        GameMessages.Instance.SetShown(gameMessageTitle);

        docSpeakerIconTexture.style.backgroundImage = new StyleBackground(speakerIconTexture);
        docMessage.text = theGameMessage.MessageText;
        docSpeakerAndMessageTitle.text = $"{speakerIconTexture.name}: {gameMessageTitle}";

        gameMessageDocument.rootVisualElement.visible = true;
    }

    #endregion

    #region Internal Support Methods

    // Determines whether all required fields are set in the Unity Editor.
    private bool ValidateInspectorFields()
    {
        bool returnValue = true;
        if (player == null)
        {
            Debug.Log("Player is NULL. Did you forget to set one in the Editor?");
            returnValue = false;
        }

        if (speakerIconTexture == null)
        {
            Debug.Log("Speaker Icon Texture is NULL. Did you forget to set one in the Editor?");
            returnValue = false;
        }

        if (string.IsNullOrWhiteSpace(gameMessageTitle))
        {
            Debug.Log("Game Message Title is not set. Did you forget to set it in the Editor?");
            returnValue = false;
        }

        if (gameMessageDocument == null)
        {
            Debug.Log("Game Message Document is NULL. Did you forget to set one in the Editor?");
            returnValue = false;
        }

        return returnValue;
    }

    // Hook up the UI elements from the UIDocument to the script's fields.
    private void HookUpUIElements()
    {
        rootVisualElement = gameMessageDocument.rootVisualElement;
        if (rootVisualElement == null)
        {
            Debug.Log("No Root Visual Element found in the Game Message user interface document.");
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
    private void HideGameMessageUI()
    {
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
        // Unregister the callback if it was registered so as not to cause memory leaks
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
        if (gameMessageDocument?.rootVisualElement != null)
        {
            gameMessageDocument.rootVisualElement.visible = false;
        }

        UnityEngine.Cursor.visible = false;

        var currentAudioSource = Globals.Instance?.CurrentAudioSource;
        if (currentAudioSource != null && currentAudioSource.isPlaying)
        {
            currentAudioSource.Stop();
        }

        if (player != null)
        {
            player.SetActive(true);
        }
    }

    #endregion
}