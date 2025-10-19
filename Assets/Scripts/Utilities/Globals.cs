using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable]
public class UiStyles
{
    [SerializeField, Tooltip("The USS style class for a quest button")]
    private string questButtonClass = "quest-button";
    public string QuestButtonClass => questButtonClass;

    [SerializeField, Tooltip("The USS style class for base text")]
    private string baseTextClass = "base-text";
    public string BaseTextClasss => baseTextClass;

    [SerializeField, Tooltip("The USS style class for task data label")]
    private string taskLabelClass = "task-label";
    public string TaskLabelClass => taskLabelClass;

    [SerializeField, Tooltip("The quest ListView fixed item height")]
    private int questListviewFixedItemHeight = 60;
    public int QuestListviewFixedItemHeight => questListviewFixedItemHeight;
}

[Serializable]
public class DoorAudioVisuals
{
    [SerializeField, Tooltip("The animation to open the door")]
    private string openAnimation = string.Empty;
    public string OpenAnimation => openAnimation;

    [SerializeField, Tooltip("The animation to close the door")]
    private string closeAnimation = string.Empty;
    public string CloseAnimation => closeAnimation;

    [SerializeField, Tooltip("The Audio Clip for the locked door rattle")]
    private AudioClip lockedDoorRattleSound = null;
    public AudioClip LockedDoorRattleSound => lockedDoorRattleSound;

    [SerializeField, Tooltip("The Audio Clip for the open door sound")]
    private AudioClip doorOpenAndCloseSound = null;
    public AudioClip DoorOpenAndCloseSound => doorOpenAndCloseSound;

    [SerializeField, Tooltip("A tooltip for when the door is locked")]
    private string actionLockedMessage = string.Empty;
    public string ActionLockedMessage => actionLockedMessage;

    [SerializeField, Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;
    public string ActionHintMessage
    {
        get => actionHintMessage;
        set => actionHintMessage = value;
    }
}

[Serializable]
public class PlayerInteraction
{
    [SerializeField, Tooltip("The UI Document to display the game message")]
    private UIDocument gameMessageDocument = null;
    public UIDocument GameMessageDocument { get => gameMessageDocument; }

    [SerializeField, Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh = null;
    public TextMeshProUGUI ActionHintTextMesh => actionHintTextMesh;

    [SerializeField, Tooltip("The text mesh to display the hint")]
    private UnityEngine.UI.Image actionHintBackground = null;
    public UnityEngine.UI.Image ActionHintBackground => actionHintBackground;

    [SerializeField, Tooltip("Whether a world space UI Application has exited")]
    private bool uiApplicationExited = false;
    public bool UiApplicationExited
    {
        get => uiApplicationExited;
        set => uiApplicationExited = value;
    }

    [SerializeField, Tooltip("The icon displayed for an action")]
    private UnityEngine.UI.Image actionIcon = null;
    public UnityEngine.UI.Image ActionIcon => actionIcon;

    [SerializeField]
    [Tooltip("The icon to show an action is unknown")]
    private UnityEngine.UI.Image unknownActionIcon;
    public UnityEngine.UI.Image UnknownActionIcon { get => unknownActionIcon; }

    [SerializeField]
    [Tooltip("An description of the action")]
    private string unknownActionDescription;
    public string UnknownActionDescription { get => unknownActionDescription; }

    [SerializeField]
    [Tooltip("The Text Mesh used to display the action description")]
    private TextMeshProUGUI textMeshActionHint = null;
    public TextMeshProUGUI TextMeshActionHint { get => textMeshActionHint; }

    [SerializeField, Tooltip("The icon to show a known action is available")]
    private UnityEngine.UI.Image interactionIndicatorIcon;
    public UnityEngine.UI.Image InteractionIndicatorIcon { get => interactionIndicatorIcon; }

    [SerializeField]
    [Tooltip("The key used to trigger object interaction")]
    private KeyCode primaryInteractionKey = KeyCode.Mouse0;
    public KeyCode PrimaryInteractionKey { get => primaryInteractionKey; set => primaryInteractionKey = value; }

}


public class Globals : Singleton<Globals>
{
    // The player GameObject that will be deactivated when the message is shown and reactivated when dismissed.
    [SerializeField, Tooltip("The player's Game Object")]
    private GameObject player = null;
    public GameObject Player { get => player; }

    [SerializeField, Tooltip("The main camera")]
    private Camera mainCamera = null;
    public Camera MainCamera { get => mainCamera; }

    private List<string> questTitles = null;
    public List<string> QuestTitles
    {
        get
        {
            if (questTitles == null)
            {
                questTitles = QuestHelper.QuestTitles;
            }
            return questTitles;
        }
    }

    private List<string> completionEvents = null;
    public List<string> CompletionEvents
    {
        get
        {
            if (completionEvents == null || completionEvents.Count == 0)
            {
                completionEvents = QuestHelper.CompletionEvents;
            }
            return completionEvents;
        }
    }

    [SerializeField, Tooltip("Door animations and sounds")]
    private UiStyles uiStyles = new UiStyles();
    public UiStyles UiStyles { get => uiStyles; }

    [SerializeField, Tooltip("Door animations and sounds")]
    private DoorAudioVisuals doorAudioVisuals = new DoorAudioVisuals();
    public DoorAudioVisuals DoorAudioVisuals { get => doorAudioVisuals; }

    [SerializeField, Tooltip("Player interaction UI and messages")]
    private PlayerInteraction playerInteraction = new PlayerInteraction();
    public PlayerInteraction PlayerInteraction => playerInteraction;

    [Header("Fields with types that cannot be serialized in container classes")]
    [SerializeField, Tooltip("The icon displayed for an action")]
    private GameObject cursorIcon = null;
    public GameObject CursorIcon => cursorIcon;

    [SerializeField, Tooltip("Used for playing the spoken audio of messages")]
    private AudioSource voiceMessageAudioSource = null;
    public AudioSource VoiceMessageAudioSource => voiceMessageAudioSource;

    [SerializeField, Tooltip("Used to display game messages")]
    private UIDocument gameMessageUiDocument = null;
    public UIDocument GameMessageUiDocument => gameMessageUiDocument;

    [SerializeField, Tooltip("Used to display task details")]
    private UIDocument taskDetailsUiDocument = null;
    public UIDocument TaskDetailsUiDocument => taskDetailsUiDocument;

    [Header("After interaction message settings")]
    // The texture used to represent the speaker's icon in the game message.
    [SerializeField, Tooltip("The speaker's face icon texture")]
    private Texture2D afterUseSpeakerIconTexture;
    public Texture2D AfterUseGameMessageSpeakerIconTexture
    {
        get => afterUseSpeakerIconTexture;
        set => afterUseSpeakerIconTexture = value;
    }

    // The title of the game message that will be displayed.
    [SerializeField, Tooltip("The game message title")]
    [GameMessageFile]
    [MessageAudioPreview]
    private string afterUseGameMessageTitle = string.Empty;
    public string AfterUseGameMessageTitle
    {
        get => afterUseGameMessageTitle;
        set => afterUseGameMessageTitle = value;
    }



    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> AudioClips => audioClips;

    [SerializeField, Tooltip("The tag for security cameras in the scene.")]
    private string cameraTag = "SecurityCamera";
    public string CameraTag { get => cameraTag; }


    private GameObject[] securityCameras;

    public GameObject[] SecurityCameras
    {
        get
        {
            if (securityCameras == null)
            {
                securityCameras = GameObject.FindGameObjectsWithTag("SecurityCamera");
                if (securityCameras.Length == 0)
                {
                    Debug.LogWarning("No security cameras found with the tag 'SecurityCamera'.");
                }
            }
            return securityCameras;
        }
    }


    public AudioSource CurrentAudioSource { get => currentAudioSource; set => currentAudioSource = value; }
    private AudioSource currentAudioSource;

    protected override void Awake()
    {
        base.Awake();

        try
        {
            if (player == null)
            {
                GameLog.ErrorMessage(this, "Globals: The player GameObject is not set in the Inspector.");
            }

            // --- DoorAudioVisuals checks ---
            if (doorAudioVisuals == null)
            {
                GameLog.ErrorMessage(this, "Globals: DoorAudioVisuals is not set.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(doorAudioVisuals.OpenAnimation))
                {
                    GameLog.ErrorMessage(this, "Globals: DoorAudioVisuals.OpenAnimation is not set or empty.");
                }
                if (string.IsNullOrWhiteSpace(doorAudioVisuals.CloseAnimation))
                {
                    GameLog.ErrorMessage(this, "Globals: DoorAudioVisuals.CloseAnimation is not set or empty.");
                }
                if (doorAudioVisuals.LockedDoorRattleSound == null)
                {
                    GameLog.ErrorMessage(this, "Globals: DoorAudioVisuals.LockedDoorRattleSound is not set.");
                }
                if (doorAudioVisuals.DoorOpenAndCloseSound == null)
                {
                    GameLog.ErrorMessage(this, "Globals: DoorAudioVisuals.DoorOpenAndCloseSound is not set.");
                }
                if (string.IsNullOrWhiteSpace(doorAudioVisuals.ActionLockedMessage))
                {
                    GameLog.ErrorMessage(this, "Globals: DoorAudioVisuals.ActionLockedMessage is not set or empty.");
                }
                if (string.IsNullOrWhiteSpace(doorAudioVisuals.ActionHintMessage))
                {
                    GameLog.ErrorMessage(this, "Globals: DoorAudioVisuals.ActionHintMessage is not set or empty.");
                }
            }

            // --- PlayerInteraction checks ---
            if (playerInteraction == null)
            {
                GameLog.ErrorMessage(this, "Globals: PlayerInteraction is not set.");
            }
            else
            {
                if (playerInteraction.GameMessageDocument == null)
                {
                    GameLog.ErrorMessage(this, "Globals: PlayerInteraction.GameMessageDocument is not set.");
                }
                if (playerInteraction.ActionHintTextMesh == null)
                {
                    GameLog.ErrorMessage(this, "Globals: PlayerInteraction.ActionHintTextMesh is not set.");
                }
            }

            //// --- ActionIcon checks ---
            //if (ActionIcon == null)
            //{
            //    GameLog.ErrorMessage(this, "Globals: ActionIcon is not set.");
            //}
            // --- VoiceMessageAudioSource checks ---
            if (VoiceMessageAudioSource == null)
            {
                GameLog.ErrorMessage(this, "Globals: VoiceMessageAudioSource is not set.");
            }

            // --- GameMessageUiDocument checks ---
            if (GameMessageUiDocument == null)
            {
                GameLog.ErrorMessage(this, "Globals: GameMessageUiDocument is not set.");
            }
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(this, "Globals Awake() exception: {0}", ex.ToString());
        }
        GameLog.NormalMessage(this, "Globals Awake() finished");
    }

    void Start()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");

        foreach (AudioClip clip in clips)
        {
            if (!audioClips.ContainsKey(clip.name))
            {
                audioClips.Add(clip.name, clip);
            }
        }
    }
}
