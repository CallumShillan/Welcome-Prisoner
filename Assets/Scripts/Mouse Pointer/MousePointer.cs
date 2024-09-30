using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The mouse cursor for the game")]
    private Texture2D mouseCursor = null;

    void Awake()
    {
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
    }
}
