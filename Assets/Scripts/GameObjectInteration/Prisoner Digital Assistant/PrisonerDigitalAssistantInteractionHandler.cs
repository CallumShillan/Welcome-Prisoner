using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrisonerDigitalAssistantInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField]
    [Tooltip("Significant Game Event associated with this action")]
    private string significantEvent = string.Empty;

    [SerializeField]
    [Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    [SerializeField]
    [Tooltip("The new layer for the PDA model")]
    private string newLayer = string.Empty;

    [SerializeField, Tooltip("Should a Game Message be shown after the interaction?")]
    private InteractionMessage postInteractionMessage = null;

    Image actionIcon = null;
    PlayerInteraction playerInteraction = null;
    TextMeshProUGUI actionHintTextMesh = null;

    void Awake()
    {
        actionIcon = Globals.Instance.PlayerInteraction.ActionIcon;
        playerInteraction = Globals.Instance.PlayerInteraction;
        actionHintTextMesh = playerInteraction.ActionHintTextMesh;

        if (postInteractionMessage.ShowGameMessageAfterInteraction)
        {
            if (string.IsNullOrWhiteSpace(postInteractionMessage.GameMessageTitle) || postInteractionMessage.SpeakerIconTexture == null)
            {
                GameLog.ErrorMessage(this, "Post Interaction Message: If 'ShowGameMessageAfterInteraction' is true, both 'GameMessageTitle' and 'SpeakerIconTexture' must be set.");
            }
        }
    }

    public bool AdvertiseInteraction()
    {
        actionIcon.enabled = true;
        actionHintTextMesh.enabled = true;
        actionHintTextMesh.text = GameUtils.ActionNameHint("Open", this.name, actionHintMessage);

        return (false);
    }

    public bool PerformInteraction()
    {
        UnityEngine.Cursor.visible = true;

        // Register that significant event
        QuestManager.HandleSignificantEvent(significantEvent);

        // No need to continue advertising the interaction
        actionIcon.enabled = false;
        actionHintTextMesh.enabled = false;

        // Disable the PDA Model's mesh renderer
        this.gameObject.layer = LayerMask.NameToLayer(newLayer);
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;

        // Show the game message
        GameUtils.DisplayInteractionMessage(postInteractionMessage);

        return (false); // As we DO NOT need further interactions to display the PDA
    }

    public InteractionStatus ContinueInteraction()
    {
        return (InteractionStatus.ShowPdaHomeScreen);
    }
}
