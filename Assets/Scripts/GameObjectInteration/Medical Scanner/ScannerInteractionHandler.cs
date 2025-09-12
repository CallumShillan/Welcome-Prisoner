using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
public class ScannerInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField, Tooltip("The Significant Event associated with this interaction")]
    [SignificantEventDropdown("GetSignificantEvents")]
    private string significantEvent = string.Empty;

    [SerializeField]
    [Tooltip("The animator for this object")]
    private Animator objectAnimator;

    [SerializeField]
    [Tooltip("The animation to perform scan biometrics")]
    private string biometricScanAnimation = string.Empty;

    [SerializeField]
    [Tooltip("The audio to play whilst scanning biometrics")]
    private AudioClip audioClip = null;

    private Image actionIcon = null;

    private TextMeshProUGUI actionHintTextMesh;

    [SerializeField]
    [Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    public void Awake()
    {
        actionIcon = Globals.Instance.PlayerInteraction.ActionIcon;
        actionHintTextMesh = Globals.Instance.PlayerInteraction.ActionHintTextMesh;
    }


    public bool AdvertiseInteraction()
    {
        // Show the icon that shows how to invoke the action, and display the hint message
        actionIcon.enabled = true;
        actionHintTextMesh.enabled = true;

        // Set the hint message
        actionHintTextMesh.text = GameUtils.ActionNameHint("Lie on the", this.name, "{ACTION} {NAME}");

        // Indicate we need furhter interactions (to indicate we have finished when the animation has stopped playing)
        return (true);
    }

    public bool PerformInteraction()
    {
        GameLog.Message(LogType.Log, this, "Scanning biometrics");

        // No need to continue advertising the interation
        actionIcon.enabled = false;
        actionHintTextMesh.enabled = false;

        // Handle the Significant Event, if needed
        if (false == string.IsNullOrEmpty(significantEvent))
        {
            QuestManager.HandleSignificantEvent(significantEvent);
        }

        // Play the defined animation
        objectAnimator.Play(biometricScanAnimation, 0, 0.0f);

        if (audioClip)
        {
            AudioSource.PlayClipAtPoint(audioClip, this.transform.position);
        }

        return (true); // As we don't need further interactions
    }

    public InteractionStatus ContinueInteraction()
    {
        // Get the animator's state information
        AnimatorStateInfo stateInfo = objectAnimator.GetCurrentAnimatorStateInfo(0);

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
