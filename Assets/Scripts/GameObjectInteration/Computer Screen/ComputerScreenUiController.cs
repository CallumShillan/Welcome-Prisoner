using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Analytics.IAnalytic;

public class ComputerScreenUiController : MonoBehaviour
{
    [SerializeField, Tooltip("The User Interface Document")]
    private UIDocument uiDocument;

    private Label statusLabel;
    private Button prevButton;
    private Button nextButton;
    private Button exitButton;
    private VisualElement cameraRender;

    private int numSecurityCameras = 0;
    private int currentCameraIndex = 0;
    private GameObject[] securityCameras;

    private ListView cameraControlListView;

    private bool initializationError = false;

    private enum Direction
    {
        Previous,
        Next
    }

    private void Awake()
    {
        initializationError = false;

        if (uiDocument == null)
        {
            Debug.LogError($"{nameof(ComputerScreenUiController)}: UIDocument is not assigned in the inspector.");
            initializationError = true;
        }

        securityCameras = Globals.Instance.SecurityCameras;
        numSecurityCameras = securityCameras?.Length ?? 0;

        if (numSecurityCameras == 0)
        {
            Debug.LogWarning($"{nameof(ComputerScreenUiController)}: No security cameras found in Globals.");
            initializationError = true;
        }
    }

    private void OnEnable()
    {
        if (initializationError)
        {
            Debug.LogError($"{nameof(ComputerScreenUiController)}: Initialization error detected. UI will not function.");
            return;
        }

        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError($"{nameof(ComputerScreenUiController)}: rootVisualElement is null.");
            initializationError = true;
            return;
        }

        prevButton = root.Q<Button>("buttonPrev");
        if (prevButton == null)
        {
            Debug.LogError("Prev button is missing in the UIDocument.");
        }

        nextButton = root.Q<Button>("buttonNext");
        if (nextButton == null)
        {
            Debug.LogError("Next button is missing in the UIDocument.");
        }

        exitButton = root.Q<Button>("buttonExit");
        if (exitButton == null)
        {
            Debug.LogError("Exit button is missing in the UIDocument.");
        }

        statusLabel = root.Q<Label>("StatusLabel");
        if (statusLabel == null)
        {
            Debug.LogError("Status label is missing in the UIDocument.");
        }

        cameraRender = root.Q<VisualElement>("CameraRender");
        if (cameraRender == null)
        {
            Debug.LogError("Camera render element is missing in the UIDocument.");
        }

        cameraControlListView = root.Q<ListView>("CameraControlListView");
        if (cameraControlListView == null)
        {
            Debug.LogError("Camera Control ListView is missing in the UIDocument.");
        }

        if (prevButton == null || nextButton == null || exitButton == null || statusLabel == null || cameraRender == null || cameraControlListView == null)
        {
            initializationError = true;
            return;
        }

        prevButton.clicked += OnPrevClicked;
        nextButton.clicked += OnNextClicked;
        exitButton.clicked += OnExitClicked;

        cameraControlListView.itemsSource = securityCameras;
        //cameraControlListView.fixedItemHeight = 32;
        cameraControlListView.selectionType = SelectionType.Single;

        cameraControlListView.makeItem = () =>
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            var nameLabel = new Label();
            nameLabel.style.flexGrow = 1;

            var activeToggle = new Toggle("Active");
            activeToggle.name = "Active";

            container.Add(activeToggle);
            container.Add(nameLabel);

            return container;
        };

        cameraControlListView.bindItem = (element, i) =>
        {
            var camera = securityCameras[i];
            var activeToggle = element.Q<Toggle>("Active");
            var nameLabel = element.Q<Label>();

            nameLabel.text = camera.name;
            activeToggle.value = camera.activeSelf;

            activeToggle.RegisterValueChangedCallback(evt =>
            {
                camera.SetActive(evt.newValue);
            });
        };


        DisplaySecurityCamera();
    }

    private void ToggleCamera(GameObject camera, bool newValue)
    {
        if (camera == null)
        {
            Debug.LogError("ToggleCamera: camera is null.");
            return;
        }
        var cameraInfo = camera.GetComponent<CameraInfo>();
        if (cameraInfo == null)
        {
            Debug.LogError($"ToggleCamera: CameraInfo component not found on {camera.name}.");
            return;
        }
        camera.gameObject.SetActive(newValue);
    }

    private void OnDisable()
    {
        if (initializationError)
        {
            return;
        }
        if (prevButton != null)
        {
            prevButton.clicked -= OnPrevClicked;
        }
        if (nextButton != null)
        {
            nextButton.clicked -= OnNextClicked;
        }
        if (exitButton != null)
        {
            exitButton.clicked -= OnExitClicked;
        }
    }

    private void OnPrevClicked()
    {
        MoveToCamera(Direction.Previous);
        DisplaySecurityCamera();
    }

    private void OnNextClicked()
    {
        MoveToCamera(Direction.Next);
        DisplaySecurityCamera();
    }

    private void OnExitClicked()
    {
        Globals.Instance.PlayerInteraction.UiApplicationExited = true;
    }

    private void MoveToCamera(Direction movement)
    {
        if (numSecurityCameras == 0)
        {
            return;
        }

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

    private void DisplaySecurityCamera()
    {
        if (numSecurityCameras == 0 || securityCameras == null || statusLabel == null || cameraRender == null)
        {
            GameLog.ErrorMessage("ComputerScreenUiController: Cannot display camera - initialization error.");
            return;
        }

        var cameraObj = securityCameras[currentCameraIndex];
        string cameraName = GameUtils.SplitPascalCase(cameraObj.name);
        if (cameraObj == null)
        {
            statusLabel.text = $"Camera {cameraName} not found";
            cameraRender.style.backgroundImage = null;
            return;
        }

        var cameraInfo = cameraObj.GetComponent<CameraInfo>();
        if (cameraInfo == null || cameraInfo.CameraRenderTexture == null)
        {
            statusLabel.text = $"Cannot render {cameraName}";
            cameraRender.style.backgroundImage = null;
            return;
        }

        statusLabel.text = cameraName;
        cameraRender.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(cameraInfo.CameraRenderTexture));
    }
}

