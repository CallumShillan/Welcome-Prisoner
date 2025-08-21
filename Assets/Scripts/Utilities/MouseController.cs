using UnityEngine;
using System.Runtime.InteropServices;

public static class MouseController
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    /// <summary>
    /// Moves the mouse cursor to the specified screen position.
    /// </summary>
    /// <param name="screenPosition">Absolute screen coordinates (pixels)</param>
    public static void MoveMouse(Vector2 screenPosition)
    {
        SetCursorPos((int)screenPosition.x, (int)screenPosition.y);
    }

    /// <summary>
    /// Moves the mouse to the center of the main display.
    /// </summary>
    public static void CenterMouseOnPrimaryDisplay()
    {
        int centerX = Screen.currentResolution.width / 2;
        int centerY = Screen.currentResolution.height / 2;
        SetCursorPos(centerX, centerY);
    }
}
