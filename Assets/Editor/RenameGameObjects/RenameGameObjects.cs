using UnityEditor;
using UnityEngine;

public static class RenameGameObjects
{
    [MenuItem("Tools/Remove GameObject Name Spaces")]
    static void RemoveSpacesFromName()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.Log("No GameObjects selected.");
            return;
        }

        Debug.Log($"Selected GameObjects ({selectedObjects.Length}):");
        foreach (GameObject obj in selectedObjects)
        {
            // Only process GameObjects that are in the scene (Hierarchy)
            if (!obj.scene.IsValid())
                continue;

            string originalName = obj.name;
            string newName = originalName.Replace(" ", "");
            if (originalName != newName)
            {
                Undo.RecordObject(obj, "Remove Spaces From GameObject Name");
                obj.name = newName;
                Debug.Log($"Renamed '{originalName}' to '{newName}'");
            }
        }
    }
}

