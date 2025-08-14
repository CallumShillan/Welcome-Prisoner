using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SecurityFeedManager : MonoBehaviour
{
    [SerializeField, Tooltip("Tag for security cameras in the scene.")]
    private string cameraTag = string.Empty;

    [SerializeField, Tooltip("Tag for security monitors in the scene.")]   
    private string monitorTag = string.Empty;

    [SerializeField, Tooltip("How long to dwell on a given security camera"), Range(1,3600)]
    private int dwellTime = 60;

    private RenderTexture[] cameraFeeds;
    private Renderer[] monitorRenderers;

    private void Awake()
    {
        if(!ValidTag(cameraTag, nameof(cameraTag)) || !ValidTag(monitorTag, nameof(monitorTag)))
        {
            Debug.LogError($"[{name}] Invalid tags provided. Please check the tags in the Inspector.", this);
            return;
        }

        // Find all cameras by tag and extract their RenderTextures
        var cameraObjects = GameObject.FindGameObjectsWithTag(cameraTag);
        cameraFeeds = new RenderTexture[cameraObjects.Length];
        for (int i = 0; i < cameraObjects.Length; i++)
        {
            var cam = cameraObjects[i].GetComponent<Camera>();
            cameraFeeds[i] = cam.targetTexture;
        }

        // Find all monitors by tag and get their Renderer components
        var monitorObjects = GameObject.FindGameObjectsWithTag(monitorTag);
        monitorRenderers = new Renderer[monitorObjects.Length];
        for (int i = 0; i < monitorObjects.Length; i++)
        {
            monitorRenderers[i] = monitorObjects[i].GetComponent<Renderer>();
        }
    }

    private void Start()
    {
        StartCoroutine(UpdateFeedsRoutine());
    }

    IEnumerator UpdateFeedsRoutine()
    {
        while (true)
        {
            AssignRandomFeeds();
            yield return new WaitForSeconds(dwellTime);
        }
    }

    void AssignRandomFeeds()
    {
        // Create a shuffled list of indices
        List<int> shuffledIndices = new List<int>();
        for (int i = 0; i < cameraFeeds.Length; i++)
            shuffledIndices.Add(i);

        // Fisher-Yates shuffle
        for (int i = shuffledIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffledIndices[i], shuffledIndices[j]) = (shuffledIndices[j], shuffledIndices[i]);
        }

        // Assign first 9 shuffled feeds to monitors
        for (int i = 0; i < monitorRenderers.Length; i++)
        {
            monitorRenderers[i].material.mainTexture = cameraFeeds[shuffledIndices[i]];
        }
    }

    private bool ValidTag(string tag, string tagName)
    {
        bool returnValue = false;
        if (string.IsNullOrWhiteSpace(tag))
        {
            Debug.LogWarning($"[{name}] {tagName} is null or empty. This may cause runtime issues.", this);
        }
        else if (!TagExists(tag))
        {
            Debug.LogError($"[{name}] {tagName} \"{tag}\" is not defined in the project’s tag manager.", this);
        }
        else
        {
            returnValue = true;
        }
        return returnValue;
    }

    private bool TagExists(string tag)
    {
#if UNITY_EDITOR
        
        // In the editor, we can check if the tag exists in the tag manager.
        foreach (var definedTag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            if (definedTag == tag)
            {
                return true;
            }
        }
        return false;
#else
        // In a build, we cannot check tags directly, so we assume they are valid.
        // This is a limitation of Unity's runtime environment.
        return true;
#endif
    }
}
