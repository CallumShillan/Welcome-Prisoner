using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteractionHandler : MonoBehaviour, IActionInterface
{
    private const string NamePlaceholder = "{NAME}";
    private const string ActionPlaceholder = "{ACTION}";

    [SerializeField, Tooltip("The significant event to raise")]
    private string significantEvent = string.Empty;

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
            return;
        }

        if (string.IsNullOrWhiteSpace(significantEvent))
        {
            GameLog.WarningMessage(this, "Significant event is not assigned or is empty.");
        }
    }

    private void Start()
    {
        if (doorIsOpen)
        {
            GameLog.Message(LogType.Log, this, "Initial state needs the door open");
            if (Globals.Instance != null && Globals.Instance.DoorAudioVisuals != null)
            {
                objectAnimator?.Play(Globals.Instance.DoorAudioVisuals.OpenAnimation, 0, 0.0f);
            }
        }
    }

    public bool AdvertiseInteraction()
    {
        if (Globals.Instance.ActionIcon != null)
        {
            Globals.Instance.ActionIcon.enabled = true;
        }
        if (Globals.Instance.DoorAudioVisuals.ActionHintMessage != null)
        {
            Globals.Instance.PlayerInteraction.ActionHintTextMesh.enabled = true;
            Globals.Instance.PlayerInteraction.ActionHintTextMesh.text = Globals.Instance.DoorAudioVisuals.ActionHintMessage
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
            PlayAudioClip(Globals.Instance?.DoorAudioVisuals?.LockedDoorRattleSound);
            return false;
        }

        PlayAudioClip(Globals.Instance?.DoorAudioVisuals?.DoorOpenAndCloseSound);

        if (objectAnimator == null)
        {
            GameLog.Message(LogType.Error, this, "Animator not found, cannot animate door.");
            return false;
        }

        if (Globals.Instance == null || Globals.Instance.DoorAudioVisuals == null)
        {
            GameLog.Message(LogType.Error, this, "Globals.Instance or DoorAudioVisuals is not assigned.");
            return false;
        }

        if (doorIsOpen)
        {
            GameLog.Message(LogType.Log, this, "Closing the door");
            objectAnimator.Play(Globals.Instance.DoorAudioVisuals.CloseAnimation, 0, 0.0f);
            doorIsOpen = false;
        }
        else
        {
            GameLog.Message(LogType.Log, this, "Opening the door");
            objectAnimator.Play(Globals.Instance.DoorAudioVisuals.OpenAnimation, 0, 0.0f);
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