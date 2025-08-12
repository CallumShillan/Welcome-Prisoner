using UnityEngine;
using System.Collections;

public class SecurityCameraController : MonoBehaviour
{
    [Header("Animation Settings")]

    [Tooltip("Animator component controlling the sweep animation.")]
    [SerializeField] private Animator animator;

    [Tooltip("Trigger name used to start the sweep animation.")]
    [SerializeField] private string sweepTrigger = "Sweep Camera";

    [Header("Sweep Timing")]

    [Tooltip("Minimum delay before the camera starts sweeping.")]
    [SerializeField] private float minStartDelay = 0f;

    [Tooltip("Maximum delay before the camera starts sweeping.")]
    [SerializeField] private float maxStartDelay = 2f;

    [Header("Optional Speed Variation")]

    [Tooltip("Enable to randomize animation speed for subtle desync.")]
    [SerializeField] private bool randomizeSpeed = true;

    [Tooltip("Minimum animation speed multiplier.")]
    [SerializeField] private float minSpeed = 0.9f;

    [Tooltip("Maximum animation speed multiplier.")]
    [SerializeField] private float maxSpeed = 1.1f;

    public void Start()
    {
        StartCoroutine(DelayedSweep());
    }

    private IEnumerator DelayedSweep()
    {
        float delay = Random.Range(minStartDelay, maxStartDelay);
        yield return new WaitForSeconds(delay);

        if (randomizeSpeed)
            animator.speed = Random.Range(minSpeed, maxSpeed);

        animator.Play(sweepTrigger);
    }
}
