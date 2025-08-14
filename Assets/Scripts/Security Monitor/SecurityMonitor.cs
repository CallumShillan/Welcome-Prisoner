using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecurityMonitor : MonoBehaviour
{
    [SerializeField, Tooltip("RawImage component to display the camera feed.")]
    private RawImage rawImage;

    [SerializeField, Tooltip("TextMeshProUGUI component to display the camera name.")]
    private TextMeshProUGUI label;

    public void Setup(RenderTexture renderTexture, string cameraName)
    {
        rawImage.texture = renderTexture as Texture;
        label.text = cameraName;
    }
}
