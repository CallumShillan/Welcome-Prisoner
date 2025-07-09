using UnityEditor;
using UnityEngine;
using System;

[InitializeOnLoad]
public static class AutoSaveEditor
{
    private const string AutoSaveEnabledKey = "AutoSaveEditor_Enabled";

    private static double lastSaveTime = 0;
    private static double saveInterval = 300; // 5 minutes
    private static bool isEnabled = true;

    static AutoSaveEditor()
    {
        isEnabled = EditorPrefs.GetBool(AutoSaveEnabledKey, true);
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (Application.isPlaying)
        {
            Debug.Log($"[AutoSave] Game is playing, so unable to save");
            return;
        }

        if (!isEnabled)
        {
            Debug.Log($"[AutoSave] not enabled, so unable to save");
            return;
        }

        if (EditorApplication.timeSinceStartup - lastSaveTime > saveInterval)
        {
            SaveAll();
            lastSaveTime = EditorApplication.timeSinceStartup;
        }
    }

    private static void SaveAll()
    {
        Debug.Log($"[AutoSave] Saving at {DateTime.Now}");
        EditorApplication.ExecuteMenuItem("File/Save");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/AutoSave/Enable AutoSave")]
    private static void EnableAutoSave()
    {
        isEnabled = true;
        EditorPrefs.SetBool(AutoSaveEnabledKey, true);
        Debug.Log("AutoSave Enabled");
    }

    [MenuItem("Tools/AutoSave/Enable AutoSave", true)]
    private static bool EnableAutoSaveValidate()
    {
        Menu.SetChecked("Tools/AutoSave/Enable AutoSave", isEnabled);
        return true;
    }

    [MenuItem("Tools/AutoSave/Disable AutoSave")]
    private static void DisableAutoSave()
    {
        isEnabled = false;
        EditorPrefs.SetBool(AutoSaveEnabledKey, false);
        Debug.Log("AutoSave Disabled");
    }

    [MenuItem("Tools/AutoSave/Disable AutoSave", true)]
    private static bool DisableAutoSaveValidate()
    {
        Menu.SetChecked("Tools/AutoSave/Disable AutoSave", !isEnabled);
        return true;
    }
}
