//
// With thanks to https://forum.unity.com/threads/is-there-a-way-to-input-text-using-a-unity-editor-utility.473743/
// and posting by Vedran_M: https://forum.unity.com/members/vedran_m.4124355/
//

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChooseListItem : EditorWindow
{
    string promptText;
    string titleInputText;
    string shortDescriptionInputText;
    string longDescriptionInputText;
    string[] allItems;
    int selectedTask;
    //Rect dialogPosition;

    Vector2 scrollPosition = Vector2.zero;

    StoryState state;
    QuestManager.OldGameEvent completionEvent;

    string okButton, cancelButton;
    bool initializedPosition = false;
    Action onOKButton;

    bool shouldClose = false;

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
        Rect rectBeginVertical = EditorGUILayout.BeginVertical();

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField(promptText);

        EditorGUILayout.Space(12);

        // Start a scroll view
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Display the list of tasks
        selectedTask = GUILayout.SelectionGrid(selectedTask, allItems, 1);

        // End the scrollview
        GUILayout.EndScrollView();

        // Draw OK / Cancel buttons
        EditorGUILayout.Space(12);

        Rect rectButtons = EditorGUILayout.GetControlRect();
        rectButtons.width /= 2;
        if (GUI.Button(rectButtons, okButton))
        {
            onOKButton?.Invoke();
            shouldClose = true;
        }

        rectButtons.x += rectButtons.width;
        if (GUI.Button(rectButtons, cancelButton))
        {
            shouldClose = true;
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.EndVertical();
    }
    #endregion OnGUI()

    #region Show()
    public static string Show(string dialogTitle, string promptText, List<string> listTitleStrings, List<string> listDescriptionStrings, string okButton = "OK", string cancelButton = "Cancel")
    {
        string selectedListItem = string.Empty;

        ChooseListItem chooseListItemWindow = CreateInstance<ChooseListItem>();

        int width = 400;
        int height = 1000;

        string[] displayStrings = new string[listTitleStrings.Count];
        string separatorString = string.Empty;
        if (listDescriptionStrings is not null)
        {
            separatorString = " - ";
            for (int iCnt = 0; iCnt < listTitleStrings.Count; iCnt++)
            {
                displayStrings[iCnt] = $"{listTitleStrings[iCnt]}{separatorString}{listDescriptionStrings[iCnt]}";
            }
        }
        else
        {
            displayStrings = listTitleStrings.ToArray();
        }


        chooseListItemWindow.maxSize = chooseListItemWindow.minSize = new Vector2(width, height);

        chooseListItemWindow.titleContent = new GUIContent(dialogTitle);
        chooseListItemWindow.promptText = promptText;
        chooseListItemWindow.allItems = displayStrings;
        chooseListItemWindow.okButton = okButton;
        chooseListItemWindow.cancelButton = cancelButton;
        chooseListItemWindow.onOKButton += () => selectedListItem = chooseListItemWindow.ChosenItem(separatorString);
        chooseListItemWindow.ShowModal();

        return selectedListItem;
    }
    public static string Show(string dialogTitle, string promptText, List<string> listStrings, string okButton = "OK", string cancelButton = "Cancel")
    {
        return Show(dialogTitle, promptText, listStrings, null, okButton, cancelButton);
    }
    #endregion Show()

    private string ChosenItem(string titleDescriptionSeparator)
    {
        string ret = string.Empty;

        if (string.IsNullOrEmpty(titleDescriptionSeparator))
        {
            ret = allItems[selectedTask];
        }
        else
        {
            ret = allItems[selectedTask].Split(titleDescriptionSeparator)[0];
        }

        return ret;
    }
}