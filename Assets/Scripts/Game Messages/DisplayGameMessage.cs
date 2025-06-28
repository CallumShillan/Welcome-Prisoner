using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;
using Invector.vCharacterController;

public class DisplayGameMessage : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The player's Game Object")]
    public GameObject player = null;

    [SerializeField]
    [Tooltip("The speaker's face icon sprite")]
    private Sprite speakerSprite;

    [SerializeField]
    [Tooltip("The game message title")]
    private string gameMessageTitle = string.Empty;

    [SerializeField]
    [Tooltip("Whether the message should be displayed")]
    private bool shouldBeShown = true;

    [SerializeField]
    [Tooltip("The UI Document to display the game message")]
    private UIDocument gameMessageDocument = null;

    // Used to hide and display the Game Message UI
    private VisualElement voiceMessageRootVisualElement = null;

    // Used to hide and display the Game Message UI
    private VisualElement speakerImage = null;

    // Displays the name of the speaker
    private UnityEngine.UIElements.Label labelSpeakerAndTitle = null;

    // The words that are spoken
    private UnityEngine.UIElements.Label labelMessage = null;

    // The command button that will close the Game Message UI
    private UnityEngine.UIElements.Button btnDismiss = null;

    private GameMessage theGameMessage = null;

    void Awake()
    {
        if (player == null)
        {
            GameLog.ErrorMessage(this, "Player is NULL. Did you forget to set one in the Editor?");
            return;
        }

        if (speakerSprite == null)
        {
            GameLog.ErrorMessage(this, "Speaker Sprite is NULL. Did you forget to set one in the Editor?");
            return;
        }

        if (string.IsNullOrWhiteSpace(gameMessageTitle))
        {
            GameLog.ErrorMessage(this, "Game Message Title is not set. Did you forget to set it in the Editor?");
            return;
        }

        if (gameMessageDocument == null)
        {
            GameLog.ErrorMessage(this, "Game Message Document is NULL. Did you forget to set one in the Editor?");
            return;
        }
        // Hide the Game Message User interface
        gameMessageDocument.rootVisualElement.visible = false;

        // Remember the Root Visual Element of the User Interface for the Game Message
        voiceMessageRootVisualElement = gameMessageDocument.rootVisualElement;
        if (voiceMessageRootVisualElement == null)
        {
            GameLog.ErrorMessage(this, "No Root Visual Element found in the Game Message user interface document.");
            return;
        }

        // Get the Root Frame of the Game Message Content
        VisualElement voiceMessageRootFrame = voiceMessageRootVisualElement.Q<VisualElement>("RootFrame");
        if (voiceMessageRootFrame == null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Visual Element called 'RootFrame' in the Game Message user interface document.");
            return;
        }

        speakerImage = voiceMessageRootVisualElement.Q<VisualElement>("visualelement-SpeakerImage");
        if (speakerImage == null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Visual Element called 'visualelement-SpeakerImage' in the Game Message user interface document.");
            return;
        }

        // The Speaker and Message Title label
        labelSpeakerAndTitle = voiceMessageRootFrame.Q<Label>("label-SpeakerAndTitle");
        if (labelSpeakerAndTitle == null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Label called 'label-SpeakerNameAndMessageTitle' in the Game Message user interface document.");
            return;
        }

        // The Spoken Words label
        labelMessage = voiceMessageRootFrame.Q<Label>("label-Message");
        if (labelMessage == null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Label called 'label-SpokenWords' in the Game Message user interface document.");
            return;
        }

        btnDismiss = voiceMessageRootFrame.Q<Button>("btn-Dismiss");
        if (btnDismiss == null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Button called 'btn-Dismiss' in the Game Message user interface document.");
            return;
        }

        btnDismiss.RegisterCallback<ClickEvent>(ev => { HandleDismissClick(); });
    }


    /// <summary>
    /// When the player trips the trigger and if it should be shown, display the message
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (shouldBeShown)
        {
            UnityEngine.Cursor.visible = true;

            // Disable the player so they don't repond to mouse movements and keyboard presses
            player.SetActive(false);

            shouldBeShown = false;

            // Update the Game Message to indicate it has been shown
            theGameMessage = GameMessages.Instance.AllGameMessages[gameMessageTitle];
            GameMessages.Instance.SetShown(gameMessageTitle);

            // Assign the Game Message properties to the UI Labels
            speakerImage.style.backgroundImage = new StyleBackground(speakerSprite);
            labelMessage.text = theGameMessage.MessageText;
            labelSpeakerAndTitle.text = $"{speakerSprite.name}: {gameMessageTitle}";

            // Show the Game Message user interface
            gameMessageDocument.rootVisualElement.visible = true;
        }
    }

    /// <summary>
    /// Respond to the DISMISS button being clicked, and hide the message
    /// </summary>
    private void HandleDismissClick()
    {
        // Hide the message user interface
        gameMessageDocument.rootVisualElement.visible = false;

        UnityEngine.Cursor.visible = false;

        // Stop any audio that might be playing
        AudioSource currentAudioSource = Globals.Instance.CurrentAudioSource;
        if (currentAudioSource != null)
        {
            if(currentAudioSource.isPlaying)
            {
                currentAudioSource.Stop();
            }
        }

        // Enable the player so they can repond to mouse movements and keyboard presses
        player.SetActive(true);
    }
}