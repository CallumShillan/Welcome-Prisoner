using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioClipPreviewAttribute))]
public class AudioClipPreviewDrawer : PropertyDrawer
{
    private AudioSource previewSource;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Define height for the AudioClip field and the button below it
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        // Rect for the AudioClip field (top)
        Rect clipRect = new Rect(position.x, position.y, position.width, lineHeight);
        EditorGUI.PropertyField(clipRect, property, label);

        if (property.propertyType == SerializedPropertyType.ObjectReference &&
            property.objectReferenceValue is AudioClip clip)
        {
            // Rect for the Play/Stop button (below)
            float buttonWidth = 70f;

            // Right-aligned button below the clip
            Rect buttonRect = new Rect(
                position.x + position.width - buttonWidth,     // align to right
                clipRect.yMax + spacing,
                buttonWidth,
                lineHeight
            );

            string buttonLabel = previewSource != null && previewSource.isPlaying ? "Stop" : "Play";

            if (GUI.Button(buttonRect, buttonLabel))
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

                if (previewSource.isPlaying)
                    previewSource.Stop();
                else
                    previewSource.Play();
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Increase total height to fit both the field and button
        return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
    }
}