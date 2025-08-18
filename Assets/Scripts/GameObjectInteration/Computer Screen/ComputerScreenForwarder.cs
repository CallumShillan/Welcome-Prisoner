using Invector.vCharacterController;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Collider))]
public class ComputerScreenForwarder : MonoBehaviour
{
    [SerializeField, Tooltip("The UI Document that will be rendered onto the mesh.")]
    private UIDocument uiDocument;

    [SerializeField, Tooltip("The main camera used to render the UI to the RenderTexture.")]
    private Camera mainCamera;

    [SerializeField, Tooltip("The RenderTexture that receives the UI output.")]
    private RenderTexture renderTexture;

    [SerializeField, Tooltip("The layer mask for UI interactable screens")]
    private LayerMask interactionLayerMask;

    [SerializeField, Tooltip("The name of a layer mask to exclude when looking for UI interactable screens")]
    private string excludeLayerMaskName;

    private VisualElement root;
    private VisualElement myCursor;
    private Transform playerOriginalTransform;


    void Start()
    {
        root = uiDocument?.rootVisualElement;
        myCursor = root.Q<VisualElement>("Cursor");
    }

    void Update()
    {
        if (root == null || renderTexture == null || mainCamera == null)
            return;

        // Determine the layers we want the raycast to report on
        int interactableGameObjectsLayerMask = 1 << LayerMask.NameToLayer(excludeLayerMaskName) | interactionLayerMask.value;

        Ray cameraThroughCursor = Camera.main.ScreenPointToRay(Input.mousePosition);

        uiDocument.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
        {
            Vector2 invalidPosition = new Vector2(float.NaN, float.NaN);

            if (!Physics.Raycast(cameraThroughCursor, out RaycastHit hit, 5.0f, interactableGameObjectsLayerMask))
            {
                Globals.Instance.Player.SetActive(true);
                Globals.Instance.CursorIcon.enabled = true;
                return invalidPosition;
            }

            Globals.Instance.CursorIcon.enabled = false;
            Globals.Instance.Player.SetActive(false);

            Vector2 uv = hit.textureCoord;

            int pixelX = Mathf.FloorToInt(uv.x * renderTexture.width);
            int pixelY = Mathf.FloorToInt((1f - uv.y) * renderTexture.height); // Flip Y

            Vector2 pixelPos = new Vector2(pixelX, pixelY);

            return pixelPos;
        });
    }
}
