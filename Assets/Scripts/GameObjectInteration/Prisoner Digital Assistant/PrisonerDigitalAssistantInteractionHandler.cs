using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrisonerDigitalAssistantInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField]
    [Tooltip("Significant Game Event to raise")]
    private string significantEvent = string.Empty;

    [SerializeField]
    [Tooltip("The icon displayed for this action")]
    private Image actionIcon = null;

    [SerializeField]
    [Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh;

    [SerializeField]
    [Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    public bool AdvertiseInteraction()
    {
        actionIcon.enabled = true;
        actionHintTextMesh.enabled = true;
        actionHintTextMesh.text = actionHintMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Open");

        return (false);
    }

    public bool PerformInteraction()
    {
        UnityEngine.Cursor.visible = true;

        // Register that significant event
        QuestManager.HandleSignificantEvent(significantEvent);

        // No need to continue advertising the interation
        actionIcon.enabled = false;
        actionHintTextMesh.enabled = false;

        //Disable the Mesh Colliders on the PDA Model's mesh
        foreach (MeshCollider childMeshCollider in this.gameObject.GetComponentsInChildren<MeshCollider>())
        {
            childMeshCollider.enabled = false;
        }

        return (true); // As we DO need further interactions to allow the PDA to move into position and open/close
    }

    public InteractionStatus ContinueInteraction()
    {
        return (InteractionStatus.ShowPdaHomeScreen);
    }
}
