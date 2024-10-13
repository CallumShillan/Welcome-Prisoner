using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DataBrickInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField]
    [Tooltip("The Databrick's GameObject Model/Prefab")]
    private GameObject databrickModel = null;

    [SerializeField]
    [Tooltip("The Significant Event, if relevant")]
    private string significantEvent = null;

    [SerializeField]
    [Tooltip("The optional animator for this object")]
    private Animator objectAnimator;

    [SerializeField]
    [Tooltip("The animation to trigger")]
    private string animationToPlay = string.Empty;

    [SerializeField]
    [Tooltip("The icon displayed for this action")]
    private UnityEngine.UI.Image actionIcon = null;

    [SerializeField]
    [Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh;

    [SerializeField]
    [Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    void Awake()
    {
        if (databrickModel is null)
        {
            GameLog.ErrorMessage(this, "The Databrick's GameObject Model/Prefab is NULL. Did you forget to set one in the Editor?");
            return;
        }

        if (objectAnimator is not null && string.IsNullOrWhiteSpace(animationToPlay))
        {
            GameLog.ErrorMessage(this, "The optional Object Animator has been set but the Animation to Play is not set. Did you forget to set it in the Editor?");
            return;
        }

        if (actionIcon is null)
        {
            GameLog.ErrorMessage(this, "The Action Icon Name is not set. Did you forget to set it in the Editor?");
            return;
        }

        if (actionHintTextMesh is null)
        {
            GameLog.ErrorMessage(this, "The Action Hint Text Mesh is not set. Did you forget to set it in the Editor?");
            return;
        }

        if (string.IsNullOrWhiteSpace(actionHintMessage))
        {
            GameLog.ErrorMessage(this, "The Action Hint Message is not set. Did you forget to set it in the Editor?");
            return;
        }

    }

    public bool AdvertiseInteraction()
    {
        actionIcon.enabled = true;
        actionHintTextMesh.enabled = true;
        actionHintTextMesh.text = actionHintMessage.Replace("{NAME}", $"'{this.name}'").Replace("{ACTION}", "Add to PDA: ");

        return (true);
    }

    /// <summary>
    /// Add the Databrick to the list of Game Messages for viewing in the PDA
    /// </summary>
    /// <returns>TRUE as we need further interactions with the user</returns>
    public bool PerformInteraction()
    {

        // Raise the significant event, if needed
        if (false == string.IsNullOrWhiteSpace(significantEvent))
        {
            QuestManager.HandleSignificantEvent(significantEvent);
        }

        // Mark the game message as having been accessed
        GameMessage theGameMessage = GameMessages.Instance.AllGameMessages[this.name];
        if(theGameMessage is not null)
        {
            theGameMessage.WhenShown = DateTime.Now;
            theGameMessage.HasBeenShown = true;
        }

        // Deactivate the DataBrick GameObject so it won't get rendered, scripts won't be run, colliders won't be effective, and so on
        databrickModel.SetActive(false);

        // Play the animation, if needed
        if(objectAnimator is not null)
        {
            if( false == string.IsNullOrEmpty(animationToPlay))
            {
                objectAnimator.Play(animationToPlay, 0, 0.0f);
            }
        }

        return (true); // As we are finished
    }
}
