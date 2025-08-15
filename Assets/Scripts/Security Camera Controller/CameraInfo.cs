using UnityEngine;

public class CameraInfo : MonoBehaviour
{

    [Header("Camera Metadata")]
    [SerializeField, Tooltip("GameObject naming suffix to indicate a Security Camera")]
    private string securityCameraSuffix = "SecurityCamera";

    [Header("Materials")]
    [SerializeField, Tooltip("Material used to display the camera's name on the monitor.")]
    private Material cameraNameMaterial;

    [SerializeField, Tooltip("Material used to render the live camera feed on the monitor.")]
    private Material cameraFeedMaterial;

    // Public getters for runtime access
    public Material CameraNameMaterial => cameraNameMaterial;
    public Material CameraFeedMaterial => cameraFeedMaterial;

    [ContextMenu("Auto-Assign Materials")]
    public void AutoAssignMaterials()
    {
        string cameraName = this.name.Replace(securityCameraSuffix, string.Empty);

        string namePath = $"SecurityCameras/CameraNames/{cameraName}";
        string feedPath = $"SecurityCameras/CameraFeeds/{cameraName}Feed";

        cameraNameMaterial = Resources.Load<Material>(namePath);
        cameraFeedMaterial = Resources.Load<Material>(feedPath);

        if (cameraNameMaterial == null)
            Debug.LogWarning($"Camera name material not found at: Resources/{namePath}");

        if (cameraFeedMaterial == null)
            Debug.LogWarning($"Camera feed material not found at: Resources/{feedPath}");
    }
}
