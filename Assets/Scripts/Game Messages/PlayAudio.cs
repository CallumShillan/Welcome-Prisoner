using UnityEngine;

/// <summary>
/// Plays an audio clip when the player enters the trigger, only once per activation.
/// </summary>
public class PlayAudio : MonoBehaviour
{
    [SerializeField, Tooltip("If true, the audio will play only once.")]
    private bool singleShotPlay = true;

    [SerializeField, Tooltip("If true, the audio will play when triggered.")]
    private bool shouldBePlayed = true;

    [SerializeField, Tooltip("If true, the audio will loop.")]
    private bool shouldLoop = false;

    [SerializeField, Tooltip("The AudioSource component to use for playback.")]
    private AudioSource audioSource;

    [SerializeField, Tooltip("The audio clip to play")]
    private AudioClip audioClip;

    private void Awake()
    {
        // Auto-assign AudioSource if not set in the Inspector
        if (audioSource == null)
        {
            GameLog.ErrorMessage(this, "The audio source is not set. Did you forget to set it in the Editor?");
            return;
        }
        if (audioClip == null)
        {
            GameLog.ErrorMessage(this, "The audio clip is not set. Did you forget to set it in the Editor?");
            return;
        }

        GameLog.NormalMessage(
            this,
            $"PlayAudio Awake() finished. singleShotPlay: {singleShotPlay}," +
            $" shouldBePlayed: {shouldBePlayed}," +
            $" shouldLoop: {shouldLoop}," +
            $" AudioSource: {audioSource.name}," +
            $" AudioClip: {audioClip.name}"
        );
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

        if(singleShotPlay && audioSource.isPlaying)
        {
            // If single shot play is enabled and audio is already playing, do not play again
            return;
        }

        if(singleShotPlay)
        {
            shouldBePlayed = false;
        }

        // Set the current audio source in Globals if available
        if (Globals.Instance != null)
        {
            Globals.Instance.CurrentAudioSource = audioSource;
        }

        GameLog.NormalMessage(this, $"Playing audio clip: {audioClip.name} and shouldLoop = {shouldLoop}");

        audioSource.clip = audioClip;
        audioSource.loop = shouldLoop;
        audioSource.Play();
    }
}

