using UnityEditor;
using UnityEngine;
using System.IO;

public class MaterialCreatorFromTextures : EditorWindow
{
    private string sourceFolder = "Assets/Materials + Textures/Personal/Roomnames";
    private string targetFolder = "Assets/Materials + Textures/Personal/Roomnames";

    [MenuItem("Tools/Create Materials from JPG Textures")]
    public static void ShowWindow()
    {
        GetWindow<MaterialCreatorFromTextures>("Material Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Materials from JPG Textures", EditorStyles.boldLabel);

        sourceFolder = EditorGUILayout.TextField("Source Texture Folder", sourceFolder);
        targetFolder = EditorGUILayout.TextField("Target Material Folder", targetFolder);

        if (GUILayout.Button("Generate Materials"))
        {
            CreateMaterialsFromTextures(sourceFolder, targetFolder);
        }
    }

    private void CreateMaterialsFromTextures(string textureFolder, string materialFolder)
    {
        if (!AssetDatabase.IsValidFolder(textureFolder))
        {
            Debug.LogError($"Invalid texture folder: {textureFolder}");
            return;
        }

        if (!AssetDatabase.IsValidFolder(materialFolder))
        {
            Directory.CreateDirectory(materialFolder);
            AssetDatabase.Refresh();
        }

        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { textureFolder });

        foreach (string guid in textureGuids)
        {
            string texturePath = AssetDatabase.GUIDToAssetPath(guid);

            if (!texturePath.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase))
                continue;

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null) continue;

            string textureName = Path.GetFileNameWithoutExtension(texturePath);
            string materialPath = Path.Combine(materialFolder, textureName + ".mat");

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetTexture("_BaseMap", texture);

            AssetDatabase.CreateAsset(mat, materialPath);
            Debug.Log($"Created material: {materialPath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Material generation complete.");
    }
}