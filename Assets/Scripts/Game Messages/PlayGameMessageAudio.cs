using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Invector.vCharacterController;

public class PlayGameMessageAudio : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Whether the message should be played")]
    private bool shouldBePlayed = true;

    [SerializeField]
    [Tooltip("The player's Game Object")]
    private GameObject player = null;

    [SerializeField]
    [Tooltip("The speaker's face icon sprite")]
    private Sprite speakerSprite;

    [SerializeField]
    [Tooltip("The game message title")]
    private string gameMessageTitle = string.Empty;

    [SerializeField]
    [Tooltip("The Text Mesh to indicate player movemenet is paused")]
    private TextMeshProUGUI textMeshPlayerMovementPaused = null;

    [SerializeField]
    [Tooltip("The UI Document for the Voice Message")]
    private UIDocument userInterfaceDocument = null;

    // Used to hide and display the Voice Message UI
    private VisualElement voiceMessageRootVisualElement = null;

    // Used to hide and display the Voice Message UI
    private VisualElement speakerImage = null;

    // Displays the name of the speaker
    private UnityEngine.UIElements.Label speakerName = null;

    // The words that are spoken
    private UnityEngine.UIElements.Label spokenWords = null;

    private GameMessage theGameMessage = null;

    private vThirdPersonController thirdPersonController = null;
    private vThirdPersonInput thirdPersonInput = null;

    void Awake()
    {
        if (player is null)
        {
            GameLog.ErrorMessage(this, "Player is NULL. Did you forget to set one in the Editor?");
            return;
        }

        thirdPersonController = player.GetComponentInChildren<vThirdPersonController>();
        if (thirdPersonController is null)
        {
            GameLog.ErrorMessage(this, "Unable to find Invector Character Controller on the Player GameObject. Did you forget to set one in the Editor?");
            return;
        }

        thirdPersonInput = player.GetComponentInChildren<vThirdPersonInput>();
        if (thirdPersonInput is null)
        {
            GameLog.ErrorMessage(this, "Unable to find Invector Character Input on the Player GameObject. Did you forget to set one in the Editor?");
            return;
        }

        if (speakerSprite is null)
        {
            GameLog.ErrorMessage(this, "Speaker Sprite is NULL. Did you forget to set one in the Editor?");
            return;
        }

        if(string.IsNullOrWhiteSpace(gameMessageTitle))
        {
            GameLog.ErrorMessage(this, "Game Message Title is not set. Did you forget to set it in the Editor?");
            return;
        }

        if (textMeshPlayerMovementPaused is null)
        {
            GameLog.ErrorMessage(this, "Text Mesh Player Movement Paused is NULL. Did you forget to set it in the Editor?");
            return;
        }
        textMeshPlayerMovementPaused.enabled = false;

        if(userInterfaceDocument is null)
        {
            GameLog.ErrorMessage(this, "User interface Document is NULL. Did you forget to set one in the Editor?");
            return;
        }
        // Hide the Voice Message User interface
        userInterfaceDocument.rootVisualElement.visible = false;

        // Remember the Root Visual Element of the User Interface for the Game Message
        voiceMessageRootVisualElement = userInterfaceDocument.rootVisualElement;
        if (voiceMessageRootVisualElement is null)
        {
            GameLog.ErrorMessage(this, "No Root Visual Element found in the Voice Message user interface document.");
            return;
        }

        // Get the Root Frame of the Game Message Content
        VisualElement voiceMessageRootFrame = voiceMessageRootVisualElement.Q<VisualElement>("RootFrame");
        if (voiceMessageRootFrame is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Visual Element called 'RootFrame' in the Voice Message user interface document.");
            return;
        }

        speakerImage = voiceMessageRootVisualElement.Q<VisualElement>("visualelement-SpeakerImage");
        if (speakerImage is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Visual Element called 'visualelement-SpeakerImage' in the Voice Message user interface document.");
            return;
        }

        // The Speaker Name label
        speakerName = voiceMessageRootFrame.Q<Label>("label-SpeakerNameAndMessageTitle");
        if (speakerName is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Label called 'label-SpeakerNameAndMessageTitle' in the Voice Message user interface document.");
            return;
        }

        // The Spoken Words label
        spokenWords = voiceMessageRootFrame.Q<Label>("label-SpokenWords");
        if (spokenWords is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Label called 'label-SpokenWords' in the Voice Message user interface document.");
            return;
        }
    }

    /// <summary>
    /// When the player trips the trigger and if it should be shown, display the message
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (shouldBePlayed)
        {
            shouldBePlayed = false;

            // Update the Game Message
            theGameMessage = GameMessages.Instance.AllGameMessages[gameMessageTitle];
            theGameMessage.WhenShown = DateTime.Now;
            theGameMessage.HasBeenShown = true;

            // Set the UI document fields
            speakerImage.style.backgroundImage = new StyleBackground(speakerSprite); 
            speakerName.text = $"{speakerSprite.name}: {gameMessageTitle}";
            spokenWords.text = theGameMessage.MessageText;

            // Play the clip
            float clipLength = AudioClips.PlayClipAtPoint(gameMessageTitle, player.transform.position);

            // Lock player movement for the duration of the clip
            StartCoroutine(LockPlayerMovement(clipLength));

        }
    }
    private IEnumerator LockPlayerMovement(float secondsToWait)
    {
        // Display the cursor, so the user can scroll through the message, if they wish
        UnityEngine.Cursor.visible = true;

        // Don't allow player to move
        textMeshPlayerMovementPaused.enabled = true;

        thirdPersonController.lockMovement = true;
        thirdPersonController.lockRotation = true;

        // Display the Voice Message UI
        userInterfaceDocument.rootVisualElement.visible = true;

        // Wait for the duration of the voice message
        GameLog.NormalMessage($"Disabled player movement at {Time.time}");
        yield return new WaitForSeconds(secondsToWait); // Wait for a number of seconds
        GameLog.NormalMessage($"Enabling player movement at {Time.time}");

        // Hide the Voice Message UI
        userInterfaceDocument.rootVisualElement.visible = false;

        // Allow player to move
        textMeshPlayerMovementPaused.enabled = false;
        thirdPersonController.lockMovement = false;
        thirdPersonController.lockRotation = false;

        // Hide the cursor
        UnityEngine.Cursor.visible = false;

    }
}
