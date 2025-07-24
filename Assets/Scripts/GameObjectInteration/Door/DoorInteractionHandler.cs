using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteractionHandler : MonoBehaviour, IActionInterface
{
    private const string NamePlaceholder = "{NAME}";
    private const string ActionPlaceholder = "{ACTION}";

    [SerializeField, Tooltip("The significant event to raise")]
    private string significantEvent = string.Empty;

    [SerializeField, Tooltip("The animation to open the door")]
    private string openAnimation = string.Empty;

    [SerializeField, Tooltip("The animation to close the door")]
    private string closeAnimation = string.Empty;

    [SerializeField, Tooltip("The Audio Clip for the locked door rattle")]
    private AudioClip lockedDoorRattleSound = null;

    [SerializeField, Tooltip("The Audio Clip for the open door sound")]
    private AudioClip doorOpenAndCloseSound = null;

    [SerializeField, Tooltip("The icon displayed for this action")]
    private Image actionIcon = null;

    [SerializeField, Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh = null;

    [SerializeField, Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    [SerializeField, Tooltip("A tooltip for when the door is locked")]
    private string actionLockedMessage = string.Empty;

    [SerializeField, Tooltip("Whether the door is locked")]
    private bool doorIsLocked = false;

    [SerializeField, Tooltip("Whether the door is open or closed")]
    private bool doorIsOpen = false;

    private Animator objectAnimator;

    public bool IsDoorLocked
    {
        get => doorIsLocked;
        set => doorIsLocked = value;
    }

    public bool IsDoorOpen
    {
        get => doorIsOpen;
        set => doorIsOpen = value;
    }

    /// <summary>
    /// Initializes the component by retrieving required references and validating their assignments.
    /// </summary>
    /// <remarks>This method attempts to retrieve the parent <see cref="Animator"/> component and validates
    /// that all required references, such as the action icon and action hint text mesh, are properly assigned. If any
    /// required reference is missing, an error message is logged.</remarks>
    private void Awake()
    {
        objectAnimator = GetComponentInParent<Animator>();
        if (objectAnimator == null)
        {
            GameLog.ErrorMessage(this, $"Unable to get the animator for door '{name}'. Did you forget to set one in the editor?");
        }
        else
        {
            // There is an animator, so check the animations
            if (string.IsNullOrWhiteSpace(openAnimation))
            {
                GameLog.ErrorMessage(this, "Open animation is not assigned or is empty.");
            }
            if (string.IsNullOrWhiteSpace(closeAnimation))
            {
                GameLog.ErrorMessage(this, "Close animation is not assigned or is empty.");
            }
        }

        if (string.IsNullOrWhiteSpace(significantEvent))
        {
            GameLog.ErrorMessage(this, "Significant event is not assigned or is empty.");
        }
        if (lockedDoorRattleSound == null)
        {
            GameLog.ErrorMessage(this, "Locked door rattle sound is not assigned.");
        }
        if (doorOpenAndCloseSound == null)
        {
            GameLog.ErrorMessage(this, "Door open and close sound is not assigned.");
        }
        if (actionIcon == null)
        {
            GameLog.ErrorMessage(this, "Action icon is not assigned.");
        }
        if (actionHintTextMesh == null)
        {
            GameLog.ErrorMessage(this, "Action hint text mesh is not assigned.");
        }
        if (string.IsNullOrWhiteSpace(actionHintMessage))
        {
            GameLog.ErrorMessage(this, "Action hint message is not assigned or is empty.");
        }
        if (string.IsNullOrWhiteSpace(actionLockedMessage))
        {
            GameLog.ErrorMessage(this, "Action locked message is not assigned or is empty.");
        }
        // doorIsLocked and doorIsOpen are bools, so no null/empty check needed
    }

    private void Start()
    {
        if (doorIsOpen)
        {
            GameLog.Message(LogType.Log, this, "Initial state needs the door open");
            objectAnimator?.Play(openAnimation, 0, 0.0f);
        }
    }

    public bool AdvertiseInteraction()
    {
        if (actionIcon != null)
        {
            actionIcon.enabled = true;
        }
        if (actionHintTextMesh != null)
        {
            actionHintTextMesh.enabled = true;
            actionHintTextMesh.text = actionHintMessage
            .Replace(NamePlaceholder, name)
            .Replace(ActionPlaceholder, doorIsOpen ? "Close" : "Open");
        }
        return false;
    }

    public bool PerformInteraction()
    {
        // Handle the significant event, if needed
        if (!string.IsNullOrEmpty(significantEvent))
        {
            QuestManager.HandleSignificantEvent(significantEvent);
        }

        if (doorIsLocked)
        {
            GameLog.Message(LogType.Log, this, "Door is locked so playing 'locked' audio");
            PlayAudioClip(lockedDoorRattleSound);
            return false;
        }

        PlayAudioClip(doorOpenAndCloseSound);

        if (objectAnimator == null)
        {
            GameLog.Message(LogType.Error, this, "Animator not found, cannot animate door.");
            return false;
        }

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

        return false; // No further interactions needed
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
}
