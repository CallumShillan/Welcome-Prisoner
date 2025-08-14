using UnityEngine;

[DisallowMultipleComponent]
public class CameraLabelConfig : MonoBehaviour
{
    [Tooltip("Tag used to identify camera GameObjects")]
    public string cameraTag = "SecurityCamera";

    [Tooltip("Folder path to save baked label textures")]
    public string labelFolder = "Assets/Materials + Textures/Personal/Labels";
}
