using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class SecurityFeedManager : MonoBehaviour
{
    [SerializeField, Tooltip("How long to dwell on a given security camera"), Range(1, 3600)]
    private int dwellTime = 60;

    [SerializeField, Tooltip("Tag for security cameras in the scene.")]
    private string cameraTag = string.Empty;

    [SerializeField, Tooltip("Tag for security monitors in the scene.")]
    private string monitorTag = string.Empty;

    [Header("Material Locations")]
    [SerializeField, Tooltip("Camera Feeds")]
    [AssetsFolderLocation("Camera Feeds")]
    private string cameraFeedMaterialsLocation = string.Empty;

    [SerializeField, Tooltip("Where are the camera name materials?")]
    [AssetsFolderLocation("Camera Names")]
    private string cameraNameMaterialsLocation = string.Empty;

    private GameObject[] securityCameras;
    private GameObject[] securityMonitors;

    private void Awake()
    {
        if(!ValidTag(cameraTag, nameof(cameraTag)) || !ValidTag(monitorTag, nameof(monitorTag)))
        {
            Debug.LogError($"[{name}] Invalid tags provided. Please check the tags in the Inspector.", this);
            return;
        }

        // Find all cameras by tag
        securityCameras = GameObject.FindGameObjectsWithTag(cameraTag);

        // Find all monitors by tag
        securityMonitors = GameObject.FindGameObjectsWithTag(monitorTag);
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
        // Step 1: Shuffle camera list
        List<GameObject> shuffledCameras = new List<GameObject>(securityCameras);
        for (int i = 0; i < shuffledCameras.Count; i++)
        {
            int swapIndex = Random.Range(i, shuffledCameras.Count);

            // Swap the current camera with a random camera further down the list
            if (swapIndex == i) continue; // No need to swap with itself
            (shuffledCameras[i], shuffledCameras[swapIndex]) = (shuffledCameras[swapIndex], shuffledCameras[i]);
        }


        // Step 2: Assign cameras to monitors
        for (int i = 0; i < securityMonitors.Length; i++)
        {
            GameObject camera = shuffledCameras[i];
            GameObject monitor = securityMonitors[i];

            CameraInfo cameraInfo = camera.GetComponent<CameraInfo>();


            ApplyMaterialToMesh(monitor, "CameraIdentifier", cameraInfo.CameraNameMaterial);
            ApplyMaterialToMesh(monitor, "Screen", cameraInfo.CameraFeedMaterial);
        }
    }

    private void ApplyMaterialToMesh(GameObject item, string meshName, Material material)
    {
        Transform meshTransform = item.transform.Find(meshName);
        if (meshTransform != null)
        {
            if (material != null)
            {
                ApplyMaterial(meshTransform.gameObject, material);
            }
            else
            {
                Debug.LogWarning($"Material {material.name} not found for {meshName}");
            }
        }
    }

    private void ApplyMaterial(GameObject target, Material mat)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;
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
