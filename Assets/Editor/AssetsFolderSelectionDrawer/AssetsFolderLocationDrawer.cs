using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AssetsFolderLocationAttribute))]
public class AssetsFolderLocationDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AssetsFolderLocationAttribute folderAttr = (AssetsFolderLocationAttribute)attribute;

        // First line: string field
        Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(fieldRect, property, new GUIContent(folderAttr.label));

        // Second line: "Choose…" button aligned right
        float buttonWidth = 80f;
        Rect buttonRect = new Rect(position.xMax - buttonWidth, fieldRect.yMax + EditorGUIUtility.standardVerticalSpacing, buttonWidth, EditorGUIUtility.singleLineHeight);

        if (GUI.Button(buttonRect, "Choose…"))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Assets Folder", "Assets", "");

            if (!string.IsNullOrEmpty(selectedPath) && selectedPath.StartsWith(Application.dataPath))
            {
                string relativePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                property.stringValue = relativePath;
            }
        }
    }
}
