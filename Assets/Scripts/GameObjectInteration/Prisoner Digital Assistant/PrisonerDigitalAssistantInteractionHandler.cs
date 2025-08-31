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

    [SerializeField, Tooltip("Should a quest+task be started?")]
    private bool initiateQuest = false;

    [SerializeField]
    [Tooltip("The follow-on Quest to initiate, if needed")]
    [QuestDropdown("GetQuestNames")]
    private string questToInitiate = string.Empty;

    [SerializeField]
    [Tooltip("The follow-on Task to initiate, if needed")]
    [TaskDropdown("GetTaskNames")]
    private string taskToInitiate = string.Empty;

    [Header("After interaction message settings")]
    // The texture used to represent the speaker's icon in the game message.
    [SerializeField, Tooltip("The speaker's face icon texture")]
    private Texture2D speakerIconTexture;

    // The title of the game message that will be displayed.
    [SerializeField, Tooltip("The game message title")]
    [GameMessageFile]
    [MessageAudioPreview]
    private string gameMessageTitle = string.Empty;

    Image actionIcon = null;
    PlayerInteraction playerInteraction = null;
    TextMeshProUGUI actionHintTextMesh = null;

    void Awake()
    {
        actionIcon = Globals.Instance.ActionIcon;
        playerInteraction = Globals.Instance.PlayerInteraction;
        actionHintTextMesh = playerInteraction.ActionHintTextMesh;

        if (initiateQuest)
        {
            if (string.IsNullOrWhiteSpace(questToInitiate) || string.IsNullOrWhiteSpace(taskToInitiate))
            {
                GameLog.ErrorMessage(this, "If 'initiateQuest' is true, both 'questToInitiate' and 'taskToInitiate' must be set.");
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

        // No need to continue advertising the interation
        actionIcon.enabled = false;
        actionHintTextMesh.enabled = false;

        //Disable the Mesh Colliders on the PDA Model's mesh
        foreach (MeshCollider childMeshCollider in this.gameObject.GetComponentsInChildren<MeshCollider>())
        {
            childMeshCollider.enabled = false;
        }

        if (initiateQuest)
        {
            GameUtils.InitiateQuestAndTask(questToInitiate, taskToInitiate);
        }

        Globals.Instance.AfterUseGameMessageTitle = gameMessageTitle;
        Globals.Instance.AfterUseGameMessageSpeakerIconTexture = speakerIconTexture;

        return (true); // As we DO need further interactions to display the PDA
    }

    public InteractionStatus ContinueInteraction()
    {
        return (InteractionStatus.ShowPdaHomeScreen);
    }
}
