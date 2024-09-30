using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class TriggerAnimation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Whether the animation should be triggered again")]
    private bool shouldBeShown = true;

    [SerializeField]
    [Tooltip("The animator for this object")]
    private Animator objectAnimator;

    [SerializeField]
    [Tooltip("The animation to trigger")]
    private string animationToPlay = string.Empty;

    void Awake()
    {
        if (objectAnimator is null)
        {
            GameLog.ErrorMessage(this, "Animator is NULL. Did you forget to set one in the Editor?");
            return;
        }

        if (string.IsNullOrWhiteSpace(animationToPlay))
        {
            GameLog.ErrorMessage(this, "Animation To Play is not set. Did you forget to set it in the Editor?");
            return;
        }
    }

    /// <summary>
    /// When the player trips the trigger and if it should be shown, display the message
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (shouldBeShown)
        {
            shouldBeShown = false;

            objectAnimator.Play(animationToPlay, 0, 0.0f);
        }
    }
}