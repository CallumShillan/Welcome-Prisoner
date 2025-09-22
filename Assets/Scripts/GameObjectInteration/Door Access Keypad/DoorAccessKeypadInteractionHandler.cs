using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static SecurityKeypad;

/// <summary>
/// Handles interactions with a security keypad to lock or unlock a door.
/// </summary>
/// <remarks>This class manages the interaction flow between the user and a security keypad, including displaying
/// the keypad UI, validating the entered code, and toggling the lock state of the associated door. It also supports
/// advertising the interaction and providing hints to the user.</remarks>
public class DoorAccessKeypadInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField, Tooltip("The Game Object of the door that will be locked/unlocked")]
    private GameObject doorGameObject = null;

    [SerializeField, Tooltip("The Significant Event for unlocking this door")]
    private string significantEvent = string.Empty;

    [SerializeField, Tooltip("The UI Document for the Security Keypad")]
    private UIDocument securityKeypadUserInterfaceDocument = null;

    [SerializeField, Tooltip("The keypad code to unlock / lock the door")]
    private string doorCode = string.Empty;


    [SerializeField, Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    // The Interaction Handler for the door that is locked/unlocked by this Security Keypad (used to determine if the door is open/closed and locked/unlocked)
    private DoorInteractionHandler controlledDoorInteractionHandler = null;

    private UnityEngine.UI.Image actionIcon = null;
    private TextMeshProUGUI actionHintTextMesh = null;

    // The Security Keypad
    private SecurityKeypad theSecurityKeypad = null;

    // Whether the interaction of the Security Keypad is completed
    private bool correctKeyCodeEntered = false;

    /// <summary>
    /// Initializes the component by retrieving required references and validating their assignments.
    /// </summary>
    private void Awake()
    {
        bool somethingNeedsToBeFixed = false;

        // Guard statements for all serialized fields
        if (doorGameObject == null)
        {
            GameLog.ErrorMessage(this, "Door Game Object is not assigned. Did you forget to add one in the Inspector?");
            somethingNeedsToBeFixed = true;
        }

        if (!doorGameObject.TryGetComponent<DoorInteractionHandler>(out controlledDoorInteractionHandler))
        {
            GameLog.ErrorMessage(this, "Unable to get the Door Interaction Handler of the door that this security lock is controlling. Did you forget to add one in the Game Object Inspector?");
            somethingNeedsToBeFixed = true;
        }

        if (securityKeypadUserInterfaceDocument == null)
        {
            GameLog.ErrorMessage(this, "UI Document for the Security Keypad is not assigned. Did you forget to add one in the Inspector?");
            somethingNeedsToBeFixed = true;
        }

        if (!securityKeypadUserInterfaceDocument.TryGetComponent<SecurityKeypad>(out theSecurityKeypad))
        {
            GameLog.ErrorMessage(this, "There is no SecurityKeypad component in the UI Document. Did you forget to add one in the Inspector?");
            somethingNeedsToBeFixed = true;
        }

        if (string.IsNullOrWhiteSpace(significantEvent))
        {
            GameLog.ErrorMessage(this, "Significant event is not assigned or is empty.");
            somethingNeedsToBeFixed = true;
        }

        if (string.IsNullOrWhiteSpace(doorCode))
        {
            GameLog.ErrorMessage(this, "Door code is not assigned or is empty.");
            somethingNeedsToBeFixed = true;
        }

        if (actionIcon == null)
        {
            GameLog.ErrorMessage(this, "Action icon is not assigned.");
            somethingNeedsToBeFixed = true;
        }

        if (actionHintTextMesh == null)
        {
            GameLog.ErrorMessage(this, "Action hint text mesh is not assigned.");
            somethingNeedsToBeFixed = true;
        }

        if (string.IsNullOrWhiteSpace(actionHintMessage))
        {
            GameLog.ErrorMessage(this, "Action hint message is not assigned or is empty.");
            somethingNeedsToBeFixed = true;
        }

        if (somethingNeedsToBeFixed)
        {
            GameLog.ErrorMessage(this, "Please fix the above issues in the Inspector.");
        }
    }

    public void Start()
    {
        // Get the action icon and hint text mesh from the Player's HUD
        PlayerInteraction playerInteraction = Globals.Instance.PlayerInteraction;
        actionIcon = playerInteraction.ActionIcon;
        actionHintTextMesh = playerInteraction.ActionHintTextMesh;
    }

    /// <summary>
    /// Displays interaction hints to the user by enabling associated UI elements.
    /// </summary>
    /// <remarks>This method enables the action icon and hint text mesh, if they are not null,  and updates
    /// the hint text with the appropriate action message.</remarks>
    /// <returns><see langword="true"/> to indicate we need to advertise further action.</returns>
    public bool AdvertiseInteraction()
    {
        actionHintTextMesh.text = SetActionMessage();
        return true;
    }

    /// <summary>
    /// Perform the interaction with the Security Keypad by displaying the UI and setting everything to an initial status so they're ready for use
    /// </summary>
    /// <returns>TRUE as we need further interactions with the user</returns>
    public bool PerformInteraction()
    {
        UnityEngine.Cursor.visible = true;
        correctKeyCodeEntered = false;

        if (theSecurityKeypad != null)
        {
            theSecurityKeypad.Show(new HandleCheckKeycode(CheckKeycode));
        }

        return true; // As we need further interactions
    }

    /// <summary>
    /// Continue the interaction unless (a) the ESCAPE key has been pressed or (b) the correct keypad code has been entered 
    /// </summary>
    /// <returns>Whether the interaction is continuing or not</returns>
    public InteractionStatus ContinueInteraction()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || correctKeyCodeEntered) && theSecurityKeypad != null)
        {
            theSecurityKeypad.Hide();
            return InteractionStatus.Completed;
        }

        return InteractionStatus.Continuing; // Indicate we have not finished
    }

    /// <summary>
    /// A call back used from the Security Keypad component to check the keycode that has been entered
    /// </summary>
    /// <param name="enteredKeyCode"></param>
    public void CheckKeycode(string enteredKeyCode)
    {
        if (string.Equals(enteredKeyCode, doorCode, StringComparison.InvariantCultureIgnoreCase))
        {
            if (controlledDoorInteractionHandler != null)
            {
                // Correct keycode status, so toggle the Door Locked status and hide the Security Keypad UI
                controlledDoorInteractionHandler.IsDoorLocked = !controlledDoorInteractionHandler.IsDoorLocked;
            }
            correctKeyCodeEntered = true;
            QuestManager.HandleSignificantEvent(significantEvent);
        }
        else
        {
            if (theSecurityKeypad != null)
            {
                theSecurityKeypad.Reset();
            }
        }
    }

    /// <summary>
    /// Set an action message to indicate whether the named door will be locked or unlocked
    /// </summary>
    /// <returns></returns>
    private string SetActionMessage()
    {
        if (doorGameObject == null || controlledDoorInteractionHandler == null)
        {
            return "Door or handler not assigned.";
        }

        string returnActionMessage = actionHintMessage.Replace("{NAME}", doorGameObject.name);

        if (controlledDoorInteractionHandler.IsDoorLocked)
        {
            returnActionMessage = returnActionMessage.Replace("{ACTION}", "Unlock");
        }
        else
        {
            returnActionMessage = returnActionMessage.Replace("{ACTION}", "Lock");
        }
        return returnActionMessage;
    }
}
