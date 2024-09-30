using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class DisplayGameMessage : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The player's Game Object")]
    public GameObject player = null;

    [SerializeField]
    [Tooltip("The game message title")]
    private string gameMessageTitle = string.Empty;

    [SerializeField]
    [Tooltip("Whether the message should be displayed")]
    private bool shouldBeShown = true;

    [SerializeField]
    [Tooltip("The UI Document to display the game message")]
    private UIDocument userInterfaceDocument = null;

    [SerializeField]
    [Tooltip("The Quest to initiate, if needed")]
    private string questToInititate = string.Empty;

    [SerializeField]
    [Tooltip("The Task to initiate, if needed")]
    private string taskToInititate = string.Empty;

    [SerializeField]
    [Tooltip("The Significant Event to raise, if needed")]
    private string significantEvent = string.Empty;

    // Used to hide and display the Security Keypad
    private VisualElement gameMessageRootVisualElement = null;

    // Displays the type of Game Message
    private UnityEngine.UIElements.Label gameMessageTitleLabel = null;

    // The Keypad Readout label that displays the keys that have been pressed
    private UnityEngine.UIElements.Label gameMessageLabel = null;

    private UnityEngine.UIElements.Button btnDismiss = null;

    private GameMessage theGameMessage = null;

    // Start is called before the first frame update
    void Start()
    {
        if(player is null)
        {
            GameLog.ErrorMessage(this, "Player is NULL. Did you forget to set on in the Editor?");
            return;
        }

        // Remember the Root Visual Element of the User Interface for the Game Message
        gameMessageRootVisualElement = userInterfaceDocument.rootVisualElement;
        if (gameMessageRootVisualElement is null)
        {
            GameLog.ErrorMessage(this, "No Root Visual Element found in the Game Message user interface document.");
            return;
        }

        // Get the Root Frame of the Game Message Content
        VisualElement gameContentAppRootFrame = gameMessageRootVisualElement.Q<VisualElement>("RootFrame");
        if (gameContentAppRootFrame is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Visual Element called 'RootFrame' in the Game Message user interface document.");
            return;
        }

        // The Game Message Title label
        gameMessageTitleLabel = gameContentAppRootFrame.Q<Label>("label-MessageTitle");
        if(gameMessageTitleLabel is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Label called 'label-MessageTitle' in the Game Message user interface document.");
            return;
        }
        else
        {
        }

        // The Game Message label
        gameMessageLabel = gameContentAppRootFrame.Q<Label>("label-Message");
        if(gameMessageLabel is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Label called 'label-Message' in the Game Message user interface document.");
            return;
        }
        else
        {
            theGameMessage = GameMessages.Instance.AllGameMessages[gameMessageTitle];
        }

        // The button to dismiss the message
        btnDismiss = gameContentAppRootFrame.Q<Button>("button-Dismiss");
        if (btnDismiss is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Button  called 'button-Dismiss' in the Game Message user interface document.");
            return;
        }
        else
        {
            btnDismiss.RegisterCallback<ClickEvent>(ev => HandleDismissClick());
        }

        // Hide the Game Message User Interface Document
        userInterfaceDocument.rootVisualElement.visible = false;

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

            if (!string.IsNullOrWhiteSpace(questToInititate))
            {
                // Assign the quest to be made active
                QuestManager.CurrentQuestName = questToInititate;

                if (!string.IsNullOrWhiteSpace(taskToInititate))
                {
                    // Assign the task to be made active
                    QuestManager.CurrentTaskName = taskToInititate;
                }
            }

            // Handle the Significant Event, if needed
            QuestManager.HandleSignificantEvent(significantEvent);

            // Update the Game Message
            theGameMessage.WhenShown = DateTime.Now;
            theGameMessage.HasBeenShown = true;
            
            // Assign the Game Message properties to the UI Labels
            gameMessageLabel.text = theGameMessage.MessageText;
            gameMessageTitleLabel.text = gameMessageTitle;

            // Show the Game Message user interface
            userInterfaceDocument.rootVisualElement.visible = true;
        }
    }

    /// <summary>
    /// Respond to the DISMISS button being clicked, and hide the message
    /// </summary>
    private void HandleDismissClick()
    {
        // Hide the message user interface
        userInterfaceDocument.rootVisualElement.visible = false;

        UnityEngine.Cursor.visible = false;

        // Enable the player so they can repond to mouse movements and keyboard presses
        player.SetActive(true);
    }
}