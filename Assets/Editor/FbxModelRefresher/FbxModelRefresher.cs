using UnityEditor;
using UnityEngine;
using System.IO;

public class ModelRefresher : EditorWindow
{
    private const string SourcePathKey = "ModelRefresher_SourcePath";
    private const string TargetFolderKey = "ModelRefresher_TargetFolder";

    private string sourcePath = "";
    private string targetFolder = "Assets/Models and Prefabs";
    private string targetFileName = "";

    [MenuItem("Window/Model Refresher")]
    public static void OpenWindow()
    {
        var window = GetWindow<ModelRefresher>();
        window.titleContent = new GUIContent("Model Refresher");
        window.Show();
    }

    private void OnEnable()
    {
        sourcePath = EditorPrefs.GetString(SourcePathKey, "");
        targetFolder = EditorPrefs.GetString(TargetFolderKey, "Assets/Models");

        if (!string.IsNullOrEmpty(sourcePath))
            targetFileName = Path.GetFileName(sourcePath);
    }

    private void OnGUI()
    {
        GUILayout.Space(8);
        GUILayout.Label("🔁 Refresh External Model", EditorStyles.boldLabel);
        GUILayout.Space(4);

        // Source file
        EditorGUILayout.BeginHorizontal();
        string newSourcePath = EditorGUILayout.TextField("Source File", sourcePath);
        if (newSourcePath != sourcePath)
        {
            sourcePath = newSourcePath;
            EditorPrefs.SetString(SourcePathKey, sourcePath);
            targetFileName = Path.GetFileName(sourcePath);
        }

        if (GUILayout.Button("Browse...", GUILayout.Width(80)))
        {
            string selected = EditorUtility.OpenFilePanel("Select Source Model", "", "fbx,blend");
            if (!string.IsNullOrEmpty(selected))
            {
                sourcePath = selected;
                targetFileName = Path.GetFileName(selected);
                EditorPrefs.SetString(SourcePathKey, sourcePath);
            }
        }
        EditorGUILayout.EndHorizontal();

        // Target folder
        EditorGUILayout.BeginHorizontal();
        string newTargetFolder = EditorGUILayout.TextField("Target Folder", targetFolder);
        if (newTargetFolder != targetFolder)
        {
            targetFolder = newTargetFolder;
            EditorPrefs.SetString(TargetFolderKey, targetFolder);
        }

        if (GUILayout.Button("Browse...", GUILayout.Width(80)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Target Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(selected))
            {
                if (selected.StartsWith(Application.dataPath))
                {
                    targetFolder = "Assets" + selected.Substring(Application.dataPath.Length);
                    EditorPrefs.SetString(TargetFolderKey, targetFolder);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Target folder must be inside the Assets directory.", "OK");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (GUILayout.Button("📥 Copy & Refresh", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(targetFolder) || string.IsNullOrEmpty(targetFileName))
            {
                EditorUtility.DisplayDialog("Error", "Please specify both source file and target folder.", "OK");
                return;
            }

            string targetPath = Path.Combine(targetFolder, targetFileName);
            RefreshModel(sourcePath, targetPath);
        }
    }

    private void RefreshModel(string source, string targetRelativePath)
    {
        string targetFullPath = Path.Combine(Application.dataPath, targetRelativePath.Replace("Assets/", ""));
        string targetDir = Path.GetDirectoryName(targetFullPath);

        if (!Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);

        File.Copy(source, targetFullPath, true);
        AssetDatabase.Refresh();

        this.ShowNotification(new GUIContent($"✅ Model copied from {source} to {targetRelativePath.Replace("\\","/")}"));
        Debug.Log($"✅ Model copied from {source} to {targetRelativePath}");
    }
}