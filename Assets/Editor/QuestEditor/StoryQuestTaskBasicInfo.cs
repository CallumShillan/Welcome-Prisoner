//
// With thanks to https://forum.unity.com/threads/is-there-a-way-to-input-text-using-a-unity-editor-utility.473743/
// and posting by Vedran_M: https://forum.unity.com/members/vedran_m.4124355/
//

using System;
using UnityEditor;
using UnityEngine;

public class StoryQuestTaskBasicInfo : EditorWindow
{
    string titleInputText;
    string shortDescriptionInputText;
    string longDescriptionInputText;

    StoryState state;
    QuestManager.OldGameEvent completionEvent;

    string okButton, cancelButton;
    bool initializedPosition = false;
    Action onOKButton;

    bool shouldClose = false;
    bool needToDrawControls = true;

    #region OnGUI()
    void OnGUI()
    {
        // Check if Esc/Return have been pressed
        var e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                // Escape pressed
                case KeyCode.Escape:
                    shouldClose = true;
                    break;

                // Enter pressed
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    onOKButton?.Invoke();
                    shouldClose = true;
                    break;
            }
        }

        if (shouldClose)
        {  // Close this dialog
            Close();
            //return;
        }

        // Draw our controls
        var rect = EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(12);

        EditorGUILayout.LabelField("Title");
        titleInputText = EditorGUILayout.TextField(string.Empty, titleInputText);

        //EditorGUILayout.Space();
        EditorGUILayout.LabelField("Short Description");
        shortDescriptionInputText = EditorGUILayout.TextField(string.Empty, shortDescriptionInputText);

        //EditorGUILayout.Space();
        GUIStyle styleLongDescription = new GUIStyle(EditorStyles.textArea);
        styleLongDescription.wordWrap = true;
        EditorGUILayout.LabelField("Long Description");
        longDescriptionInputText = EditorGUILayout.TextArea(longDescriptionInputText, styleLongDescription, GUILayout.Height(400), GUILayout.MinHeight(400));

        EditorGUILayout.Space(12);
        
        needToDrawControls = false;

        // Draw OK / Cancel buttons
        var r = EditorGUILayout.GetControlRect();
        r.width /= 2;
        if (GUI.Button(r, okButton))
        {
            onOKButton?.Invoke();
            shouldClose = true;
        }

        r.x += r.width;
        if (GUI.Button(r, cancelButton))
        {
            titleInputText = string.Empty;   // Cancel - delete inputText
            shortDescriptionInputText = string.Empty;   // Cancel - delete inputText
            longDescriptionInputText = string.Empty;   // Cancel - delete inputText
            shouldClose = true;
        }

//        EditorGUILayout.Space(8);
        EditorGUILayout.EndVertical();

        // Force change size of the window
        if (rect.width != 0 && minSize != rect.size)
        {
            minSize = maxSize = rect.size;
        }

        // Set dialog position next to mouse position
        if (!initializedPosition)
        {
            //var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            //position = new Rect(mousePos.x + 32, mousePos.y, position.width, position.height);
            initializedPosition = true;

            int width = 400;
            int height = 400;
            int x = (Screen.currentResolution.width - width) / 2;
            int y = (Screen.currentResolution.height - height) / 2;
            position = new Rect(x, y, width, height);

        }
    }
    #endregion OnGUI()

    #region Show()
    public static BasicInfo Show(string dialogTitle, string okButton = "OK", string cancelButton = "Cancel")
    {
        BasicInfo ret = null;

        StoryQuestTaskBasicInfo window = CreateInstance<StoryQuestTaskBasicInfo>();

        window.minSize = new Vector2(800, 400);

        window.titleContent = new GUIContent(dialogTitle);
        //window.titleInputText = "string.Empty";
        //window.shortDescriptionInputText = "string.Empty";
        //window.longDescriptionInputText = "string.Empty";
        window.okButton = okButton;
        window.cancelButton = cancelButton;
        window.onOKButton += () => ret = new BasicInfo(window.titleInputText, window.shortDescriptionInputText, window.longDescriptionInputText, window.state, window.completionEvent);
        window.ShowModal();

        return ret;
    }
    #endregion Show()
}
