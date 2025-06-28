using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DisplayGameMessage : MonoBehaviour
{
    // Visual Element Name Constants
    private const string RootFrameName = "RootFrame";
    private const string SpeakerImageName = "SpeakerTexture2D";
    private const string SpeakerAndTitleName = "SpeakerAndTitleLabel";
    private const string MessageTextName = "MessageLabel";
    private const string BtnDismissName = "DismissButton";

    [SerializeField, Tooltip("The player's Game Object")]
    private GameObject player = null;

    [SerializeField, Tooltip("The speaker's face icon texture")]
    private Texture2D speakerImageIcon;

    [SerializeField, Tooltip("The game message title")]
    private string gameMessageTitle = string.Empty;

    [SerializeField, Tooltip("Whether the message should be displayed")]
    private bool shouldBeShown = true;

    [SerializeField, Tooltip("The UI Document to display the game message")]
    private UIDocument gameMessageDocument = null;

    private VisualElement voiceMessageRootVisualElement;
    private VisualElement speakerImage;
    private Label labelSpeakerAndTitle;
    private Label labelMessage;
    private Button btnDismiss;

    private GameMessage theGameMessage;

    // Store the delegate so it can be unregistered
    private EventCallback<ClickEvent> btnDismissClickHandler;

    private void Awake()
    {
        if (!ValidateSerializedFields())
            return;

        HookUpUiElements();

        btnDismissClickHandler = _ => HandleDismissClick();
        btnDismiss.RegisterCallback<ClickEvent>(btnDismissClickHandler);
    }

    // Unregister the button click event to prevent memory leaks when this object is destroyed.
    private void OnDestroy()
    {
        if (btnDismiss != null && btnDismissClickHandler != null)
        {
            btnDismiss.UnregisterCallback<ClickEvent>(btnDismissClickHandler);
        }
    }

    private bool ValidateSerializedFields()
    {
        if (player == null)
        {
            GameLog.ErrorMessage(this, "Player is NULL. Did you forget to set one in the Editor?");
            return false;
        }
        if (speakerImageIcon == null)
        {
            GameLog.ErrorMessage(this, "Speaker Image Icon is NULL. Did you forget to set one in the Editor?");
            return false;
        }
        if (string.IsNullOrWhiteSpace(gameMessageTitle))
        {
            GameLog.ErrorMessage(this, "Game Message Title is not set. Did you forget to set it in the Editor?");
            return false;
        }
        if (gameMessageDocument == null)
        {
            GameLog.ErrorMessage(this, "Game Message Document is NULL. Did you forget to set one in the Editor?");
            return false;
        }
        return true;
    }

    private void HookUpUiElements()
    {
        voiceMessageRootVisualElement = gameMessageDocument.rootVisualElement;
        if (voiceMessageRootVisualElement == null)
        {
            GameLog.ErrorMessage(this, "No Root Visual Element found in the Game Message user interface document.");
            return;
        }

        voiceMessageRootVisualElement.visible = false;

        var voiceMessageRootFrame = voiceMessageRootVisualElement.Q<VisualElement>(RootFrameName);
        if (voiceMessageRootFrame == null)
        {
            GameLog.ErrorMessage(this, $"Unable to find a Visual Element called '{RootFrameName}' in the Game Message user interface document.");
            return;
        }

        speakerImage = voiceMessageRootVisualElement.Q<VisualElement>(SpeakerImageName);
        if (speakerImage == null)
        {
            GameLog.ErrorMessage(this, $"Unable to find a Visual Element called '{SpeakerImageName}' in the Game Message user interface document.");
            return;
        }

        labelSpeakerAndTitle = voiceMessageRootFrame.Q<Label>(SpeakerAndTitleName);
        if (labelSpeakerAndTitle == null)
        {
            GameLog.ErrorMessage(this, $"Unable to find a Label called '{SpeakerAndTitleName}' in the Game Message user interface document.");
            return;
        }

        labelMessage = voiceMessageRootFrame.Q<Label>(MessageTextName);
        if (labelMessage == null)
        {
            GameLog.ErrorMessage(this, $"Unable to find a Label called '{MessageTextName}' in the Game Message user interface document.");
            return;
        }

        btnDismiss = voiceMessageRootFrame.Q<Button>(BtnDismissName);
        if (btnDismiss == null)
        {
            GameLog.ErrorMessage(this, $"Unable to find a Button called '{BtnDismissName}' in the Game Message user interface document.");
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!shouldBeShown) return;

        UnityEngine.Cursor.visible = true;
        player.SetActive(false);
        shouldBeShown = false;

        if (!TryGetGameMessage(gameMessageTitle, out theGameMessage))
        {
            GameLog.ErrorMessage(this, $"Game message with title '{gameMessageTitle}' not found.");
            return;
        }

        theGameMessage.WhenShown = DateTime.Now;
        theGameMessage.HasBeenShown = true;

        speakerImage.style.backgroundImage = new StyleBackground(speakerImageIcon);
        labelMessage.text = theGameMessage.MessageText;
        labelSpeakerAndTitle.text = $"{speakerImageIcon.name}: {gameMessageTitle}";

        gameMessageDocument.rootVisualElement.visible = true;
    }

    private bool TryGetGameMessage(string title, out GameMessage message)
    {
        message = null;
        if (GameMessages.Instance == null || GameMessages.Instance.AllGameMessages == null)
            return false;
        return GameMessages.Instance.AllGameMessages.TryGetValue(title, out message);
    }

    private void HandleDismissClick()
    {
        gameMessageDocument.rootVisualElement.visible = false;
        UnityEngine.Cursor.visible = false;

        var currentAudioSource = Globals.Instance.CurrentAudioSource;
        if (currentAudioSource != null && currentAudioSource.isPlaying)
        {
            currentAudioSource.Stop();
        }

        player.SetActive(true);
    }
}