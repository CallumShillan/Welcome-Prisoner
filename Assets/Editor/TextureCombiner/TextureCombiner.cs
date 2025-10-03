using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class TextureCombiner : EditorWindow
{
    Texture2D metallicOrBaseColor;
    Texture2D smoothness;
    enum TextureType { Metallic, BaseColor, Unknown }

    [MenuItem("Tools/Texture Combiner")]
    public static void ShowWindow()
    {
        GetWindow<TextureCombiner>("Texture Combiner");
    }

    void OnGUI()
    {
        GUILayout.Label("Combine Metallic/BaseColor and Smoothness", EditorStyles.boldLabel);
        metallicOrBaseColor = (Texture2D)EditorGUILayout.ObjectField("Metallic or BaseColor texture", metallicOrBaseColor, typeof(Texture2D), false);
        smoothness = (Texture2D)EditorGUILayout.ObjectField("Smoothness texture", smoothness, typeof(Texture2D), false);

        if (GUILayout.Button("Combine"))
        {
            CombineTextures();
        }
    }

    void CombineTextures()
    {
        if (metallicOrBaseColor == null || smoothness == null)
        {
            EditorUtility.DisplayDialog("Error", "Both textures must be set", "OK");
            return;
        }

        string metallicOrBaseColorPath = AssetDatabase.GetAssetPath(metallicOrBaseColor);
        string metallicOrBaseColorDir = Path.GetDirectoryName(metallicOrBaseColorPath);
        string metallicOrBaseColorName = Path.GetFileNameWithoutExtension(metallicOrBaseColorPath);
        string smoothnessPath = AssetDatabase.GetAssetPath(smoothness);

        TextureType textureType = TextureType.Unknown;
        if(Regex.IsMatch(metallicOrBaseColorName, "metallic", RegexOptions.IgnoreCase))
        {
            textureType = TextureType.Metallic;
        }
        else if (Regex.IsMatch(metallicOrBaseColorName, "basecolor|albedo|diffuse", RegexOptions.IgnoreCase))
        {
            textureType = TextureType.BaseColor;
        }
        else
        {
            EditorUtility.DisplayDialog("Warning", "Can't find 'metallic' 'basecolor' 'albedo' 'diffuse' in first texture - will simply append '+smooth' to its name", "OK");
        }

        if (!metallicOrBaseColor.isReadable)// || !smoothness.isReadable)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(metallicOrBaseColorPath);
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        if (!smoothness.isReadable)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(smoothnessPath);
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        int width = metallicOrBaseColor.width;
        int height = metallicOrBaseColor.height;
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] metallicOrBaseColorPixels = metallicOrBaseColor.GetPixels();
        Color[] smoothPixels = smoothness.GetPixels();

        for (int i = 0; i < metallicOrBaseColorPixels.Length; i++)
        {
            Color m = metallicOrBaseColorPixels[i];
            float s = smoothPixels[i].r; // Assuming smoothness is in red channel
            metallicOrBaseColorPixels[i] = new Color(m.r, m.g, m.b, s);
        }

        result.SetPixels(metallicOrBaseColorPixels);
        result.Apply();

        string searchPattern = textureType == TextureType.Metallic ? "metallic" :
                               textureType == TextureType.BaseColor ? "basecolor|albedo|diffuse" : "";

        string replacedName = Regex.Replace(
                                metallicOrBaseColorName,
                                searchPattern,
                                match => match.Value + "+smooth",
                                RegexOptions.IgnoreCase
                            );

        // Convert to absolute system path
        string absoluteDir = Path.GetFullPath(metallicOrBaseColorDir);
        string savePath = EditorUtility.SaveFilePanel("Save Combined Texture", absoluteDir, replacedName, "png");

        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllBytes(savePath, result.EncodeToPNG());
            AssetDatabase.Refresh();
            Debug.Log("Combined texture saved to: " + savePath);
        }

    }

}