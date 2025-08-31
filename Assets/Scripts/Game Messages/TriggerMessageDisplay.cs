using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

/// <summary>
/// Manages the display of in-game messages, including speaker icons, message text, and dismiss functionality.
/// </summary>
/// <remarks>This class is responsible for showing and hiding game messages in the UI, as well as handling user
/// interactions such as dismissing the message. It integrates with Unity's UI Toolkit and requires specific UI elements
/// to be defined in the associated <see cref="UIDocument"/>. The class also interacts with the player GameObject and
/// global game state to manage visibility and behavior during message display.  Ensure that all required fields are set
/// in the Unity Editor, including the player GameObject, speaker icon texture, game message title, and the UI document used to display the game message.
/// Missing or improperly configured fields will result in warnings in the Unity console.</remarks>
public class TriggerMessageDisplay : MonoBehaviour
{
    private enum WhenToShowMessage
    {
        [Description("Always")]
        Always,

        [Description("Event HAS Occurred")]
        EventOccurred,

        [Description("Event NOT Occurred")]
        EventNotOccurred
    }

    // The texture used to represent the speaker's icon in the game message.
    [SerializeField, Tooltip("The speaker's face icon texture")]
    private Texture2D speakerIconTexture;

    // The title of the game message that will be displayed.
    [SerializeField, Tooltip("The game message title")]
    [GameMessageFile]
    [MessageAudioPreview]
    private string gameMessageTitle = string.Empty;

    // Whether the game message should be shown when the player collides with the trigger.
    [SerializeField, Tooltip("Whether the message should be displayed")]
    private bool shouldBeShown = true;

    // Condition for the message to be shown.
    [SerializeField, Tooltip("Condition for message to be shown")]
    private WhenToShowMessage whenShown = WhenToShowMessage.Always;

    // Significant Event to check condition against.
    [SerializeField, Tooltip("Significant Event to check condition against")]
    [SignificantEventDropdown("GetSignificantEvents")]
    private string significantEvent = string.Empty;

    private bool triggerHandled = false;

    private void Awake()
    {
        if (speakerIconTexture == null)
        {
            Debug.Log("Speaker Icon Texture is NULL. Did you forget to set one in the Editor?");
        }

        if (string.IsNullOrWhiteSpace(gameMessageTitle))
        {
            Debug.Log("Game Message Title is not set. Did you forget to set it in the Editor?");
        }

        if(whenShown != WhenToShowMessage.Always && string.IsNullOrEmpty(significantEvent))
        {
            Debug.Log($"When Shown is '{whenShown}' and Significant Event is not set. Did you forget to set it in the Editor?");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(triggerHandled)
        {
            return; // Prevents multiple triggers from the same collider.
        }
        else
        {
            triggerHandled = true; // Set to true to prevent further triggers.
        }

        if (!shouldBeShown)
        {
            return;
        }

        // Only trigger for the player
        if (!other.CompareTag("Player"))
        {
            return;
        }

        shouldBeShown = false; // Prevents the message from being shown again after the first trigger.

        bool shouldDisplayMessage = false;
        switch (whenShown)
        {
            case WhenToShowMessage.Always:
                shouldDisplayMessage = true;
                break;
            case WhenToShowMessage.EventOccurred:
                if (QuestManager.GetSignificantEventState(significantEvent) == QuestManager.SignificantEventState.SignificantEventOccurred)
                {
                    shouldDisplayMessage = true;
                }
                break;
            case WhenToShowMessage.EventNotOccurred:
                if (QuestManager.GetSignificantEventState(significantEvent) == QuestManager.SignificantEventState.SignificantEventNotOccurred)
                {
                    shouldDisplayMessage = true;
                }
                break;
        }

        if (shouldDisplayMessage)
        {
            DisplayGameMessage.Instance.ShowGameMessage(speakerIconTexture, gameMessageTitle);
        }
    }
}