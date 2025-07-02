using UnityEngine;

/// <summary>
/// Plays an audio clip when the player enters the trigger, only once per activation.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlayAudio : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If true, the audio will play when triggered.")]
    private bool shouldBePlayed = true;

    [SerializeField]
    [Tooltip("The AudioSource component to use for playback.")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("The AudioClip to play.")]
    private AudioClip audioClip;

    private void Awake()
    {
        // Auto-assign AudioSource if not set in the Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                GameLog.ErrorMessage(this, "The audio source is not set and could not be found on the GameObject.");
                return;
            }
        }
        if (audioClip == null)
        {
            GameLog.ErrorMessage(this, "The audio clip is not set. Did you forget to set it in the Editor?");
            return;
        }
    }

    /// <summary>
    /// When the player trips the trigger and if it should be played, play the audio.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider's GameObject is tagged as "Player"
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (!shouldBePlayed)
        {
            return;
        }

        shouldBePlayed = false;

        // Set the current audio source in Globals if available
        if (Globals.Instance != null)
        {
            Globals.Instance.CurrentAudioSource = audioSource;
        }

        audioSource.clip = audioClip;
        audioSource.Play();
    }
}

