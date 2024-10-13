using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Invector.vCharacterController;

public class PlayAudio : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Whether the audio should be played")]
    private bool shouldBePlayed = true;

    [SerializeField]
    [Tooltip("Whether the audio should be played")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("Whether the audio should be played")]
    private AudioClip audioClip;

    void Awake()
    {
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

            // Remember the active AudioSource in case it needs to be stopped by some other event/action
            Globals.Instance.CurrentAudioSource = audioSource;

            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}

