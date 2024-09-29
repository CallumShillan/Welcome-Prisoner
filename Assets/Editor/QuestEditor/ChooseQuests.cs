//
// With thanks to https://forum.unity.com/threads/is-there-a-way-to-input-text-using-a-unity-editor-utility.473743/
// and posting by Vedran_M: https://forum.unity.com/members/vedran_m.4124355/
//

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChooseQuests : EditorWindow
{
    string titleInputText;
    string shortDescriptionInputText;
    string longDescriptionInputText;
    List<string> allQuests;
    static List<bool> selectedQuests = null;

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

        EditorGUILayout.LabelField("All Quests");

        for (int iCnt = 0; iCnt < allQuests.Count; ++iCnt)
        {
            selectedQuests[iCnt] = EditorGUILayout.ToggleLeft(allQuests[iCnt], selectedQuests[iCnt]);
        }

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
            for (int iCnt = 0; iCnt < allQuests.Count; ++iCnt)
            {
                selectedQuests[iCnt] = false;
            }
            shouldClose = true;
        }

        //        EditorGUILayout.Space(8);
        EditorGUILayout.EndVertical();

        // Force change size of the window
        if (rect.width != 0 && minSize != rect.size)
        {
            minSize = maxSize = rect.size;
        }

        // Set dialog position in the screeen center
        if (!initializedPosition)
        {
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
    public static List<string> Show(string dialogTitle, List<string> allQuests, string okButton = "OK", string cancelButton = "Cancel")
    {
        List<string> ret = null;
        if(selectedQuests is null)
        {
            selectedQuests = new List<bool>(allQuests.Count);
            foreach (string questName in allQuests)
            {
                selectedQuests.Add(false);
            }
        }
        else
        {
            for( int iCnt = 0; iCnt < selectedQuests.Count; iCnt++)
            {
                selectedQuests[iCnt] = false;
            }
        }

        ChooseQuests window = CreateInstance<ChooseQuests>();

        window.minSize = new Vector2(800, 400);

        window.titleContent = new GUIContent(dialogTitle);
        window.allQuests = allQuests;
        window.okButton = okButton;
        window.cancelButton = cancelButton;
        window.onOKButton += () => ret = window.SelectedQuests();
        window.ShowModal();

        return ret;
    }
    #endregion Show()

    private List<string> SelectedQuests()
    {
        List<string> returnQuests = new List<string>();

        for (int iCnt = 0; iCnt < allQuests.Count; ++iCnt)
        {
            bool isSelected = selectedQuests[iCnt];
            if ( isSelected )
            {
                returnQuests.Add(allQuests[iCnt]);
            }
        }

        return returnQuests;
    }
}
