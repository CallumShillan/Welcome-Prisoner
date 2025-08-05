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
public class TriggerMessageDisplay : MonoBehaviour
{
    // The texture used to represent the speaker's icon in the game message.
    [SerializeField, Tooltip("The speaker's face icon texture")]
    private Texture2D speakerIconTexture;

    // The title of the game message that will be displayed.
    [SerializeField, Tooltip("The game message title")]
    private string gameMessageTitle = string.Empty;

    // Whether the game message should be shown when the player collides with the trigger.
    [SerializeField, Tooltip("Whether the message should be displayed")]
    private bool shouldBeShown = true;

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
    }

    private void OnTriggerEnter(Collider other)
    {
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

        DisplayGameMessage.Instance.ShowGameMessage(speakerIconTexture, gameMessageTitle);
    }
}