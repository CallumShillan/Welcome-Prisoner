using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Collider))]
public class ComputerScreenForwarder : MonoBehaviour
{
    public UIDocument uiDocument;
    public Camera mainCamera;
    public RenderTexture renderTexture;

    private VisualElement root;

    void Start()
    {
        root = uiDocument?.rootVisualElement;
    }

    void Update()
    {
        if (root == null || renderTexture == null || mainCamera == null)
            return;

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject != gameObject)
                return;

            Vector2 uv = hit.textureCoord;
            Vector2 pixelPos = new Vector2(uv.x * renderTexture.width, (1f - uv.y) * renderTexture.height);

            ForwardPointerEvents(pixelPos);
        }
    }

    void ForwardPointerEvents(Vector2 pixelPos)
    {
        // Hover
        var moveEvent = PointerMoveEvent.GetPooled();
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
