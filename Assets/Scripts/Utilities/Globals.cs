using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField, Tooltip("Door animations and sounds")]
    private DoorAudioVisuals doorAudioVisuals = new DoorAudioVisuals();
    public DoorAudioVisuals DoorAudioVisuals { get => doorAudioVisuals; }

    [SerializeField, Tooltip("Player interaction UI and messages")]
    private PlayerInteraction playerInteraction = new PlayerInteraction();
    public PlayerInteraction PlayerInteraction => playerInteraction;

    [Header("Fields with types that cannot be serialized in container classes")]
    [SerializeField, Tooltip("The icon displayed for an action")]
    private UnityEngine.UI.Image cursorIcon = null;
    public UnityEngine.UI.Image CursorIcon => cursorIcon;

    [SerializeField, Tooltip("The icon displayed for an action")]
    private UnityEngine.UI.Image actionIcon = null;
    public UnityEngine.UI.Image ActionIcon => actionIcon;

    [SerializeField, Tooltip("Used for playing the spoken audio of messages")]
    private AudioSource voiceMessageAutioSource = null;
    public AudioSource VoiceMessageAudioSource => voiceMessageAutioSource;

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

            // --- ActionIcon checks ---
            if (ActionIcon == null)
            {
                GameLog.ErrorMessage(this, "Globals: ActionIcon is not set.");
            }
            // --- VoiceMessageAudioSource checks ---
            if (VoiceMessageAudioSource == null)
            {
                GameLog.ErrorMessage(this, "Globals: VoiceMessageAudioSource is not set.");
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
