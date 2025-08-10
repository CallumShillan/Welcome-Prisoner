using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

[CustomPropertyDrawer(typeof(GameMessageFileAttribute))]
public class GameMessageFileDrawer : PropertyDrawer
{
    private string[] fileNames;
    private AudioSource previewSource;
    private string sceneName;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Three lines: dropdown + Show Message + Audio Preview
        return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        if (fileNames == null)
        {
            string folderPath = Path.Combine(Application.dataPath, $"Resources/GameMessages/{sceneName}");
            if (Directory.Exists(folderPath))
            {
                fileNames = Directory.GetFiles(folderPath, "*.txt")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToArray();
            }
            else
            {
                fileNames = new string[] { "(No files found)" };
            }
        }

        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        // Row 1: Dropdown
        Rect dropdownRect = new Rect(position.x, position.y, position.width, lineHeight);
        int selectedIndex = Mathf.Max(0, System.Array.IndexOf(fileNames, property.stringValue));
        selectedIndex = EditorGUI.Popup(dropdownRect, label.text, selectedIndex, fileNames);
        property.stringValue = fileNames.Length > 0 ? fileNames[selectedIndex] : "";

        // Row 2: Show Message button on RHS
        float buttonWidth = 80f;
        Rect messageLabelRect = new Rect(position.x, position.y + lineHeight + spacing, position.width - buttonWidth - 4f, lineHeight);
        Rect messageButtonRect = new Rect(position.x + position.width - buttonWidth, messageLabelRect.y, buttonWidth, lineHeight);

        EditorGUI.LabelField(messageLabelRect, "View Message:");
        if (GUI.Button(messageButtonRect, "Show 📄"))
        {
            string filePath = Path.Combine(Application.dataPath, $"Resources/GameMessages/{sceneName}/{property.stringValue}.txt");
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                EditorUtility.DisplayDialog("Message Preview", content, "Close");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Message file not found.", "OK");
            }
        }

        // Row 3: Audio preview
        Rect labelRect = new Rect(position.x, position.y + (lineHeight + spacing) * 2, position.width - buttonWidth - 4f, lineHeight);
        Rect buttonRect = new Rect(position.x + position.width - buttonWidth, labelRect.y, buttonWidth, lineHeight);

        string audioPath = $"Audio/{sceneName}/{property.stringValue}";
        AudioClip clip = Resources.Load<AudioClip>(audioPath);

        if (clip != null)
        {
            EditorGUI.LabelField(labelRect, "Preview Audio:");
            if (GUI.Button(buttonRect, IsPlaying(clip) ? "Stop 🔇" : "Play 🔊"))
            {
                TogglePreview(clip);
            }
        }
        else
        {
            EditorGUI.LabelField(labelRect, "No Audio Found");
        }

        EditorGUI.EndProperty();
    }

    private void TogglePreview(AudioClip clip)
    {
        if (previewSource == null)
        {
            GameObject tempGO = new GameObject("AudioPreview");
            tempGO.hideFlags = HideFlags.HideAndDontSave;
            previewSource = tempGO.AddComponent<AudioSource>();
        }

        if (previewSource.isPlaying && previewSource.clip == clip)
        {
            previewSource.Stop();
        }
        else
        {
            previewSource.clip = clip;
            previewSource.Play();
        }
    }

    private bool IsPlaying(AudioClip clip)
    {
        return previewSource != null && previewSource.isPlaying && previewSource.clip == clip;
    }
}