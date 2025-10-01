using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class TextureCombiner : EditorWindow
{
    Texture2D metallic;
    Texture2D smoothness;
    string outputName = "Combined_BaseMap";

    [MenuItem("Tools/Texture Combiner")]
    public static void ShowWindow()
    {
        GetWindow<TextureCombiner>("Texture Combiner");
    }

    void OnGUI()
    {
        GUILayout.Label("Combine Metallic and Smoothness", EditorStyles.boldLabel);
        metallic = (Texture2D)EditorGUILayout.ObjectField("Metallic Map", metallic, typeof(Texture2D), false);
        smoothness = (Texture2D)EditorGUILayout.ObjectField("Smoothness Map", smoothness, typeof(Texture2D), false);

        if (GUILayout.Button("Combine"))
        {
            CombineTextures();
        }
    }

    void CombineTextures()
    {
        if (metallic == null || smoothness == null)
        {
            Debug.LogError("Both textures must be assigned.");
            return;
        }

        if (!metallic.isReadable || !smoothness.isReadable)
        {
            Debug.LogError("One or both textures are not readable. Enable 'Read/Write' in import settings.");
            return;
        }

        int width = metallic.width;
        int height = metallic.height;

        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] metallicPixels = metallic.GetPixels();
        Color[] smoothPixels = smoothness.GetPixels();

        for (int i = 0; i < metallicPixels.Length; i++)
        {
            Color m = metallicPixels[i];
            float s = smoothPixels[i].r; // Assuming smoothness is in red channel
            metallicPixels[i] = new Color(m.r, m.g, m.b, s);
        }

        result.SetPixels(metallicPixels);
        result.Apply();

        string metallicPath = AssetDatabase.GetAssetPath(metallic);
        string metallicDir = Path.GetDirectoryName(metallicPath);
        string metallicName = Path.GetFileNameWithoutExtension(metallicPath);
        string replacedName = Regex.Replace(
                                    metallicName,
                                    "metallic",
                                    "metallic+smooth",
                                    RegexOptions.IgnoreCase
                                );


        // Convert to absolute system path
        string absoluteDir = Path.GetFullPath(metallicDir);
        string savePath = EditorUtility.SaveFilePanel("Save Combined Texture", absoluteDir, replacedName, "png");

        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllBytes(savePath, result.EncodeToPNG());
            AssetDatabase.Refresh();
            Debug.Log("Combined texture saved to: " + savePath);
        }

    }

}