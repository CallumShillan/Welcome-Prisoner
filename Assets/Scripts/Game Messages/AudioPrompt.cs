using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Invector.vCharacterController;

public class AudioPrompt : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Whether the audio should be played")]
    private bool shouldBePlayed = true;

    [SerializeField]
    [Tooltip("The name of the audio to play")]
    private string audioToPlay = string.Empty;

    void Awake()
    {
        if (string.IsNullOrWhiteSpace(audioToPlay))
        {
            GameLog.ErrorMessage(this, "The name of the audio to play is not set. Did you forget to set it in the Editor?");
            return;
        }
    }

    /// <summary>
    /// When the player trips the trigger and if it should be played, play the audio
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (shouldBePlayed)
        {
            shouldBePlayed = false;

            // Play the clip
            float clipLength = AudioClips.PlayClipAtPoint(audioToPlay, this.transform.position);
        }
    }
}

