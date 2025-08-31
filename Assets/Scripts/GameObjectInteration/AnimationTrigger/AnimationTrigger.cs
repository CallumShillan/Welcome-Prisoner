using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationTrigger : MonoBehaviour, IActionInterface
{
    private const string NamePlaceholder = "{NAME}";
    private const string ActionPlaceholder = "{ACTION}";

    [SerializeField, Tooltip("The significant event to raise")]
    [SignificantEventDropdown("GetSignificantEvents")]
    private string significantEvent = string.Empty;

    [SerializeField, Tooltip("The name of this action")]
    private string actionName = string.Empty;

    [SerializeField, Tooltip("The action hint to display")]
    private string actionHint = string.Empty;

    [SerializeField, Tooltip("The animation to play")]
    private string animationToPlay = string.Empty;

    [SerializeField, Tooltip("Optional animation to play")]
    private AudioClip audioToPlay = null;

    [SerializeField, Tooltip("Optional game object to initialy hide")]
    private GameObject gameObjectToHide = null;

    [SerializeField, Tooltip("Should a quest+task be started?")]
    private bool initiateQuest = false;

    [SerializeField, Tooltip("The follow-on Quest to initiate, if needed")]
    [QuestDropdown("GetQuestNames")]
    private string questToInitiate = string.Empty;

    [SerializeField, Tooltip("The follow-on Task to initiate, if needed")]
    [TaskDropdown("GetTaskNames")]
    private string taskToInitiate = string.Empty;

    private Animator theAnimator = null;

    /// <summary>
    /// Initializes the component by retrieving required references and validating their assignments.
    /// </summary>
    /// <remarks>This method attempts to retrieve the parent <see cref="Animator"/> component and validates
    /// that all required references, such as the action icon and action hint text mesh, are properly assigned. If any
    /// required reference is missing, an error message is logged.</remarks>
    private void Awake()
    {
        theAnimator = GetComponentInParent<Animator>();
        if (theAnimator == null)
        {
            GameLog.ErrorMessage(this, $"Unable to get the animator for '{this.name}'. Did you forget to set one in the editor?");
            return;
        }

        // Gatekeeper routines for [SerializeField] fields
        if (string.IsNullOrWhiteSpace(significantEvent))
        {
            GameLog.WarningMessage(this, "Significant event is not assigned or is empty.");
        }
        if (string.IsNullOrWhiteSpace(actionName))
        {
            GameLog.WarningMessage(this, "Action name is not assigned or is empty.");
        }
        if (string.IsNullOrWhiteSpace(actionHint))
        {
            GameLog.WarningMessage(this, "Action hint is not assigned or is empty.");
        }
        if (string.IsNullOrWhiteSpace(animationToPlay))
        {
            GameLog.WarningMessage(this, "Animation to play is not assigned or is empty.");
        }
        if (audioToPlay == null)
        {
            GameLog.WarningMessage(this, "Audio clip to play is not assigned.");
        }

        if(gameObjectToHide == null)
        {
            GameLog.WarningMessage(this, "Game object to hide and show is not assigned.");
        }
        else
        {
            gameObjectToHide.SetActive(false);
            GameLog.NormalMessage(this, $"Game object '{gameObjectToHide.name}' has been set not active so as to hide it.");
        }

        if (initiateQuest)
        {
            if (string.IsNullOrWhiteSpace(questToInitiate) || string.IsNullOrWhiteSpace(taskToInitiate))
            {
                GameLog.ErrorMessage(this, "If 'initiateQuest' is true, both 'questToInitiate' and 'taskToInitiate' must be set.");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool AdvertiseInteraction()
    {
        if (Globals.Instance.ActionIcon != null)
        {
            Globals.Instance.ActionIcon.enabled = true;
        }
        if (Globals.Instance.DoorAudioVisuals.ActionHintMessage != null)
        {
            Globals.Instance.PlayerInteraction.ActionHintTextMesh.enabled = true;
            Globals.Instance.PlayerInteraction.ActionHintTextMesh.text = GameUtils.ActionNameHint(actionName, this.name, actionHint);
        }
        return false;
    }

    public bool PerformInteraction()
    {
        if (theAnimator == null)
        {
            GameLog.Message(LogType.Error, this, $"Animator not found, cannot play '{animationToPlay}' animation.");
            return false;
        }

        // Handle the significant event, if needed
        if (!string.IsNullOrEmpty(significantEvent))
        {
            QuestManager.HandleSignificantEvent(significantEvent);
        }

        // Play the animation, if needed
        if (!string.IsNullOrEmpty(animationToPlay))
        {
            theAnimator.Play(animationToPlay, 0, 0.0f);
        }

        // Play audio, if needed
        if (audioToPlay != null)
        {
            AudioSource.PlayClipAtPoint(audioToPlay, this.transform.position);
        }

        GameUtils.InitiateQuestAndTask(questToInitiate, taskToInitiate);

        return true;
    }
    public InteractionStatus ContinueInteraction()
    {
        // Get the animator's state information
        AnimatorStateInfo stateInfo = theAnimator.GetCurrentAnimatorStateInfo(0);

        // From https://docs.unity3d.com/ScriptReference/AnimatorStateInfo-normalizedTime.html
        // The normalized time is a progression ratio
        // The integer part is the number of times the State has looped
        // The fractional part is a percentage (0-1) that represents the progress of the current loop

        // Therefore, if normalized time is greater than or equal to 1, it has played 1x (which is all we want the player to experience)

        if (stateInfo.normalizedTime >= 1.0f)
        {
            return (InteractionStatus.Completed);
        }

        // Indicate we have not finished
        return (InteractionStatus.Continuing);
    }
}