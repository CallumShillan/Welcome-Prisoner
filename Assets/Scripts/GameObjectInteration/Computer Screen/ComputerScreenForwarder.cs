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

    private bool playerMoved = false;

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
        Debug.DrawRay(cameraThroughCursor.origin, cameraThroughCursor.direction * 100.0f, Color.red);

        uiDocument.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
        {
            Vector2 invalidPosition = new Vector2(float.NaN, float.NaN);

            if (!Physics.Raycast(cameraThroughCursor, out RaycastHit hit, 5.0f, interactableGameObjectsLayerMask))
            {
                Globals.Instance.Player.SetActive(true);
                return invalidPosition;
            }

            if(hit.collider.gameObject != gameObject)
            {
                Globals.Instance.Player.transform.SetLocalPositionAndRotation(playerOriginalTransform.position, playerOriginalTransform.rotation);
                Globals.Instance.Player.SetActive(true);
                Globals.Instance.CursorIcon.enabled = true;
                return invalidPosition;
            }


            if(!playerMoved)
            {
                playerMoved = true;
                playerOriginalTransform = Globals.Instance.Player.transform;

                Vector3 playerPosition = Globals.Instance.Player.transform.position;
                Vector3 screenPosition1 = hit.point + hit.normal * 1f; // Offset slightly to avoid z-fighting
                screenPosition1.z = playerPosition.z; // Keep the player's z position

                Globals.Instance.Player.transform.LookAt(hit.transform);
                Globals.Instance.Player.transform.position = screenPosition1;
            }

            //Globals.Instance.Player.SetActive(false);
            Globals.Instance.CursorIcon.enabled = false;

            Vector2 uv = hit.textureCoord;

            int pixelX = Mathf.FloorToInt(uv.x * renderTexture.width);
            int pixelY = Mathf.FloorToInt((1f - uv.y) * renderTexture.height); // Flip Y

            Vector2 pixelPos = new Vector2(pixelX, pixelY);
            
            if (myCursor != null)
            {
                myCursor.style.left = pixelPos.x;
                myCursor.style.top = pixelPos.y;
            }

            return pixelPos;
        });
    }


    void ForwardPointerEvents(Vector2 pixelPos)
    {
        // Hover
        var moveEvent = PointerMoveEvent.GetPooled();
        // Forward pointer move
        root.SendEvent(moveEvent);

        // Click
        if (Input.GetMouseButtonDown(0))
        {
            var downEvent = PointerDownEvent.GetPooled();
            root.SendEvent(downEvent);
        }

        if (Input.GetMouseButtonUp(0))
        {
            var upEvent = PointerUpEvent.GetPooled();
            root.SendEvent(upEvent);
        }

        // Scroll
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            var wheelEvent = WheelEvent.GetPooled();
            root.SendEvent(wheelEvent);
        }
    }

}
