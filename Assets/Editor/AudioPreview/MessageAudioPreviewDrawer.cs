using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MessageAudioPreviewAttribute))]
public class MessageAudioPreviewDrawer : PropertyDrawer
{
    private AudioSource previewSource;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float buttonWidth = 70f;

        // Draw the string field
        Rect textRect = new Rect(position.x, position.y, position.width, lineHeight);
        property.stringValue = EditorGUI.TextField(textRect, label.text, property.stringValue);

        // Right-aligned button underneath
        Rect buttonRect = new Rect(
            position.x + position.width - buttonWidth,
            textRect.yMax + spacing,
            buttonWidth,
            lineHeight
        );

        // Load the AudioClip based on the string
        string path = $"Audio/{property.stringValue}";
        AudioClip clip = Resources.Load<AudioClip>(path);

        // Determine button label
        bool isPlaying = previewSource != null && previewSource.isPlaying;
        string buttonLabel = isPlaying ? "Stop" : "Play";

        // Button interaction
        if (GUI.Button(buttonRect, buttonLabel))
        {
            if (clip != null)
            {
                if (previewSource == null)
                {
                    previewSource = EditorUtility.CreateGameObjectWithHideFlags(
                        "EditorPreviewAudioSource",
                        HideFlags.HideAndDontSave,
                        typeof(AudioSource)
                    ).GetComponent<AudioSource>();
                }

                previewSource.clip = clip;

                if (isPlaying)
                {
                    previewSource.Stop();
                }
                else
                {
                    previewSource.Play();
                }
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Missing Audio",
                    $"Message audio clip '{property.stringValue}' not found in Resources/Audio folder.",
                    "OK"
                );
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
    }
}