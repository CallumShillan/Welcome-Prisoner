using Invector.vCharacterController;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Collider))]
public class ComputerScreenInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField, Tooltip("The significant event to raise")]
    private string significantEvent = string.Empty;

    [SerializeField, Tooltip("The UI Document that will be rendered onto the mesh.")]
    private UIDocument uiDocument;

    [SerializeField, Tooltip("The camera used to render the UI to the RenderTexture.")]
    private Camera renderTextureCamera;

    [SerializeField, Tooltip("The RenderTexture that receives the UI output.")]
    private RenderTexture renderTexture;

    [SerializeField, Tooltip("The layer mask for UI interactable screens")]
    private LayerMask interactionLayerMask;

    [SerializeField, Tooltip("The name of a layer mask to exclude when looking for UI interactable screens")]
    private string excludeLayerMaskName;

    private VisualElement root;

    private bool initializationError = false;

    private Vector2 cachedPixelPos = new Vector2(float.NaN, float.NaN);

    private PlayerInteraction playerInteraction;
    private TextMeshProUGUI actionHintTextMesh;

    void Awake()
    {
        initializationError = false;

        if (uiDocument == null)
        {
            Debug.LogError($"[{nameof(ComputerScreenInteractionHandler)}] UIDocument is not assigned on '{gameObject.name}'. Disabling component.");
            initializationError = true;
        }
        if (renderTextureCamera == null)
        {
            Debug.LogError($"[{nameof(ComputerScreenInteractionHandler)}] Render Texture Camera is not assigned on '{gameObject.name}'. Disabling component.");
            initializationError = true;
        }
        if (renderTexture == null)
        {
            Debug.LogError($"[{nameof(ComputerScreenInteractionHandler)}] RenderTexture is not assigned on '{gameObject.name}'. Disabling component.");
            initializationError = true;
        }
        if (interactionLayerMask == 0)
        {
            // Not fatal, but warn
            Debug.LogWarning($"[{nameof(ComputerScreenInteractionHandler)}] InteractionLayerMask is not set on '{gameObject.name}'.");
        }
        if (string.IsNullOrWhiteSpace(excludeLayerMaskName))
        {
            // Not fatal, but warn
            Debug.LogWarning($"[{nameof(ComputerScreenInteractionHandler)}] ExcludeLayerMaskName is not set on '{gameObject.name}'.");
        }

        root = uiDocument != null ? uiDocument.rootVisualElement : null;
        if (root == null)
        {
            Debug.LogError($"[{nameof(ComputerScreenInteractionHandler)}] rootVisualElement is null on '{gameObject.name}'. Disabling component.");
            initializationError = true;
        }

        uiDocument.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) => cachedPixelPos);
    }

    public void Start()
    {
        playerInteraction = Globals.Instance.PlayerInteraction;
        if (playerInteraction == null)
        {
            Debug.LogError($"[{nameof(ComputerScreenInteractionHandler)}] PlayerInteraction is not found in Globals. Disabling component.");
            initializationError = true;
        }

        actionHintTextMesh = playerInteraction.ActionHintTextMesh;
        if (actionHintTextMesh == null)
        {
            Debug.LogError($"[{nameof(ComputerScreenInteractionHandler)}] ActionHintTextMesh is not assigned in PlayerInteraction. Disabling component.");
            initializationError = true;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool AdvertiseInteraction()
    {
        actionHintTextMesh.text = GameUtils.ActionNameHint("Use", name, "{ACTION} {NAME}");
        playerInteraction.UiApplicationExited = false;
        return true;
    }

    public bool PerformInteraction()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        if (initializationError)
        {
            return false;
        }

        CacheMousePosition();

        return true;
    }

    public InteractionStatus ContinueInteraction()
    {
        CacheMousePosition();

        if (playerInteraction.UiApplicationExited)
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            return InteractionStatus.Completed;
        }
        else
        {
            return InteractionStatus.Continuing;
        }
    }

    private void CacheMousePosition()
    {
        int computerScreenLayer = 1 << LayerMask.NameToLayer(excludeLayerMaskName) | interactionLayerMask.value;

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out RaycastHit hit, 5.0f, computerScreenLayer))
        {
            Vector2 uv = hit.textureCoord;
            int pixelX = Mathf.FloorToInt(uv.x * renderTexture.width);
            int pixelY = Mathf.FloorToInt((1f - uv.y) * renderTexture.height); // Flip Y
            cachedPixelPos = new Vector2(pixelX, pixelY);
        }
        else
        {
            cachedPixelPos = new Vector2(float.NaN, float.NaN);
        }
    }
}
