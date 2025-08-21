using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ComputerScreenUiController : MonoBehaviour
{
    [SerializeField, Tooltip("The User Interface Document")]
    private UIDocument uiDocument;

    [SerializeField, Tooltip("The material to display on the computer screen")]
    private GameObject computerScreenMesh;

    private Label statusLabel;
    private Button prevButton;
    private Button nextButton;

    private int numSecurityCameras = 0;
    private int currentCameraIndex = 0;
    private GameObject[] securityCameras;

    enum Direction
    {
        Previous,
        Next
    }

    private void Awake()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned in the inspector.");
        }
        
        securityCameras = Globals.Instance.SecurityCameras;

        numSecurityCameras  = securityCameras.Length;
    }

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        root.Q<Button>("buttonPrev").clicked += () =>
        {
            MoveToCamera(Direction.Previous);
            DisplaySecurityCamera();
        };

        root.Q<Button>("buttonNext").clicked += () =>
        {
            MoveToCamera(Direction.Next);
            DisplaySecurityCamera();
        };

        root.Q<Button>("buttonExit").clicked += () =>
        {
            Globals.Instance.PlayerInteraction.UiApplicationExited = true;
        };

        statusLabel = root.Q<Label>("StatusLabel");
        DisplaySecurityCamera();
    }

    private void MoveToCamera(Direction movement)
    {
        switch (movement)
        {
            case Direction.Previous:
                currentCameraIndex = (currentCameraIndex - 1 + numSecurityCameras) % numSecurityCameras;
                break;
            case Direction.Next:
                currentCameraIndex = (currentCameraIndex + 1) % numSecurityCameras;
                break;
        }

    }

    void DisplaySecurityCamera()
    {
        GameObject camera = securityCameras[currentCameraIndex];

        CameraInfo cameraInfo = camera.GetComponent<CameraInfo>();

        Renderer renderer = computerScreenMesh.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = cameraInfo.CameraFeedMaterial;
        }

        statusLabel.text = $"Camera: {camera.name}";
    }
}

