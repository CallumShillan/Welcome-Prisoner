using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static SecurityKeypad;

public class DoorAccessKeypadInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField]
    [Tooltip("The Game Object of the door that will be locked/unlocked")]
    private GameObject doorGameObject = null;

    [SerializeField]
    [Tooltip("The Significant Event for unlocking this door")]
    private string significantEvent = string.Empty;

    [SerializeField]
    [Tooltip("The UI Document for the Security Keypad")]
    private UIDocument securityKeypadUserInterfaceDocument = null;

    [SerializeField]
    [Tooltip("The keypad code to unlock / lock the door")]
    private string doorCode = null;

    [SerializeField]
    [Tooltip("The icon displayed for this action")]
    private UnityEngine.UI.Image actionIcon = null;

    [SerializeField]
    [Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh;

    [SerializeField]
    [Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    // The Interaction Handler for the door that is locked/unlocked by this Security Keypad (used to determine if the door is open/closed and locked/unlocked)
    private DoorInteractionHandler controlledDoorInteractionHandler = null;

    // The Security Keypad
    private SecurityKeypad theSecurityKeypad = null;

    // Whether the interaction of the Security Keypad is completed
    private bool correctKeyCodeEntered = false;

    // Start is called before the first frame update
    public void Start()
    {
        // Try to get the Door Game Object script from the Door Game Object
        if(doorGameObject is null)
        {
            GameLog.ErrorMessage(this, "Unable to get the Door Game Object. Did you forget to add one in the Game Object Inspector?");
            return;
        }

        // Try to get the Door Interaction Handler
        if (false == doorGameObject.TryGetComponent<DoorInteractionHandler>(out controlledDoorInteractionHandler))
        {
            GameLog.ErrorMessage(this, "Unable to get the Door Interaction Handler of the door that this security lock is controlling. Did you forget to add one in the Game Object Inspector?");
            return;
        }

        if(securityKeypadUserInterfaceDocument is null)
        {
            GameLog.ErrorMessage(this, "There is no UI Document for the User Interface. Did you forget to add one in the Inspector?");
            return;
        }

        if(false == securityKeypadUserInterfaceDocument.TryGetComponent<SecurityKeypad>(out theSecurityKeypad))
        {
            GameLog.ErrorMessage(this, "There is no SecurityKeypad component in the UI Document. Did you forget to add one in the Inspector?");
            return;
        }

    }

    public bool AdvertiseInteraction()
    {
        actionIcon.enabled = true;
        actionHintTextMesh.enabled = true;

        actionHintTextMesh.text = SetActionMessage();

        return (true);
    }

    /// <summary>
    /// Perform the interaction with the Security Keypad by displaying the UI and setting everything to an initial status so they're ready for use
    /// </summary>
    /// <returns>TRUE as we need further interactions with the user</returns>
    public bool PerformInteraction()
    {
        // Check we're in the right Game State to be able to perform this interaction
        //if(GameState.CurrentProgress != requiredGameProgress)
        //{
        //    return (false);
        //}
        UnityEngine.Cursor.visible = true;
        correctKeyCodeEntered = false;
        
        theSecurityKeypad.Show(new HandleCheckKeycode(CheckKeycode));

        return (true); // As we need further interactions
    }

    /// <summary>
    /// Continue the interaction unless (a) the ESCAPE key has been pressed or (b) the correct keypad code has been entered 
    /// </summary>
    /// <returns>Whether the interaction is continuiing or not</returns>
    public InteractionStatus ContinueInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || correctKeyCodeEntered)
        {
            theSecurityKeypad.Hide();
            //GameState.CurrentProgress = progressGameToNextState;
            return (InteractionStatus.Completed);
        }

        return (InteractionStatus.Continuing); // Indicate we have not finished
    }
    
    /// <summary>
    /// A call back used from the Security Keypad component to check the keycode that has been entered
    /// </summary>
    /// <param name="enteredKeyCode"></param>
    public void CheckKeycode(string enteredKeyCode)
    {
        if (string.Equals(enteredKeyCode, doorCode, StringComparison.InvariantCultureIgnoreCase))
        {
            // Correct keycode status, so toggle the Door Locked status and hide the Security Keypad UI
            controlledDoorInteractionHandler.IsDoorLocked = !controlledDoorInteractionHandler.IsDoorLocked;
            correctKeyCodeEntered = true;
            QuestManager.HandleSignificantEvent(significantEvent);
        }
        else
        {
            theSecurityKeypad.Reset();
        }
    }

    /// <summary>
    /// Set an action message to indicate whether the named door will be locked or unlocked
    /// </summary>
    /// <returns></returns>
    private string SetActionMessage()
    {
        string returnActionMessage = actionHintMessage.Replace("{NAME}", doorGameObject.name);

        if (controlledDoorInteractionHandler.IsDoorLocked)
        {
            returnActionMessage = returnActionMessage.Replace("{ACTION}", "Unlock");
        }
        else
        {
            returnActionMessage = returnActionMessage.Replace("{ACTION}", "Lock");
        }
        return (returnActionMessage);
    }
}
