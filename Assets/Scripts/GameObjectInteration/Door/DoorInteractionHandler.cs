using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class DoorInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField]
    [Tooltip("The animator for this object")]
    private Animator objectAnimator;

    [SerializeField]
    [Tooltip("The animation to open the door")]
    private string openAnimation = string.Empty;

    [SerializeField]
    [Tooltip("The animation to close the door")]
    private string closeAnimation = string.Empty;

    [SerializeField]
    [Tooltip("The Audio Clip for the locked door rattle")]
    private AudioClip lockedDoorRattleSound = null;

    [SerializeField]
    [Tooltip("The Audio Clip for the open door sound")]
    private AudioClip doorOpenAndCloseSound = null;

    [SerializeField]
    [Tooltip("The icon displayed for this action")]
    private Image actionIcon = null;

    [SerializeField]
    [Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh;

    [SerializeField]
    [Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    [SerializeField]
    [Tooltip("A tooltip for when the door is locked")]
    private string actionLockedMessage = string.Empty;

    [SerializeField]
    [Tooltip("Whether the door is locked")]
    private bool doorIsLocked = false;

    [SerializeField]
    [Tooltip("Whether the door is open or closed")]
    private bool doorIsOpen = false;

    public bool IsDoorLocked { get => doorIsLocked; set => doorIsLocked = value; }
    public bool IsDoorOpen { get => doorIsOpen; set => doorIsOpen = value; }

    public void Start()
    {
        if(doorIsOpen)
        {
            GameLog.Message(LogType.Log, this, "Initial state needs the door open");
            objectAnimator.Play(openAnimation, 0, 0.0f);
        }
    }

    public bool AdvertiseInteraction()
    {
        actionIcon.enabled = true;
        actionHintTextMesh.enabled = true;

        if (doorIsLocked)
        {
            actionHintTextMesh.text = actionLockedMessage.Replace("{NAME}", this.name).Replace("{ACTION}", doorIsOpen ? "Close" : "Open");
        }
        else
        {
            actionHintTextMesh.text = actionHintMessage.Replace("{NAME}", this.name).Replace("{ACTION}", doorIsOpen ? "Close" : "Open");
        }
        return (false);
    }

    public bool PerformInteraction()
    {
        if (doorIsLocked)
        {
            GameLog.Message(LogType.Log, this, "Door is locked so playing 'locked' audio");
            AudioSource.PlayClipAtPoint(lockedDoorRattleSound, this.transform.position);
        }
        else
        {
            AudioSource.PlayClipAtPoint(doorOpenAndCloseSound, this.transform.position);

            // Thanks to https://www.youtube.com/watch?v=tJiO4cvsHAo for basic mechanism of animating doors
            if (doorIsOpen)
            {
                GameLog.Message(LogType.Log, this, "Closing the door");
                objectAnimator.Play(closeAnimation, 0, 0.0f);
                doorIsOpen = false;
            }
            else
            {
                GameLog.Message(LogType.Log, this, "Opening the door");
                objectAnimator.Play(openAnimation, 0, 0.0f);
                doorIsOpen = true;
            }
        }

        return (false); // As we don't need further interactions
    }
}
