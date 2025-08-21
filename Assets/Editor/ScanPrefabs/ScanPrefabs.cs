using UnityEditor;
using UnityEngine;
using System.IO;

public class AudioListenerScanner
{
    [MenuItem("Tools/Scan Prefabs for AudioListeners")]
    public static void ScanPrefabs()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && prefab.GetComponentInChildren<AudioListener>(true) != null)
            {
                Debug.Log($"AudioListener found in prefab: {path}", prefab);
            }
        }
    }
}
