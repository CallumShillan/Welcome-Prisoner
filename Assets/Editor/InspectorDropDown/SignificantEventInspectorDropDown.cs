using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(StringDropdownAttribute))]
public class SignificantEventDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //QuestHelper questHelper = new QuestHelper();
//        questHelper.LoadStoryGraph(); // Ensure quests are loaded
        List<string> displayedOptions = QuestHelper.CompletionEvents;
        displayedOptions.Sort((a, b) =>
        {
            bool aPrefixed = a.StartsWith("--");
            bool bPrefixed = b.StartsWith("--");

            // Move "--" prefixed strings to the top
            if (aPrefixed && !bPrefixed) return -1;
            if (!aPrefixed && bPrefixed) return 1;

            // Otherwise do a standard alphabetical comparison
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
        });


        int currentIndex = displayedOptions.IndexOf(property.stringValue);
        if (currentIndex == -1) currentIndex = 0;

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, displayedOptions.ToArray());
        property.stringValue = displayedOptions[selectedIndex];
        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(QuestDropdownAttribute))]
public class QuestDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        List<string> displayedOptions = QuestHelper.QuestTitles;
        displayedOptions.Sort();

        int currentIndex = displayedOptions.IndexOf(property.stringValue);
        if (currentIndex == -1) currentIndex = 0;

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, displayedOptions.ToArray());
        property.stringValue = displayedOptions[selectedIndex];
        EditorGUI.EndProperty();
    }
}

/// <summary>
/// Provides a custom property drawer for fields decorated with the <see cref="TaskDropdownAttribute"/>. This drawer
/// displays a dropdown menu populated with task titles from a quest, allowing users to select a task.
/// </summary>
/// <remarks>The dropdown options are dynamically populated based on the value of the sibling property
/// <c>questToInitiate</c>, which specifies the quest whose tasks should be listed. Ensure that the
/// <c>questToInitiate</c> property is correctly set and references a valid quest in the
/// <c>QuestHelper.QuestDictionary</c>.</remarks>
[CustomPropertyDrawer(typeof(TaskDropdownAttribute))]
public class TaskDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        List<string> displayedOptions = null;

        // Access the 'questToInitiate' sibling property as we only want to list the tasks that the quest has, not every single task
        SerializedProperty questToInitiateProp = property.serializedObject.FindProperty("questToInitiate");

        // Get its value
        string questToInitiateValue = questToInitiateProp?.stringValue;

        // Get the quest chosen in the Inspector, and its tasks
        Quest quest = QuestHelper.QuestDictionary[questToInitiateValue];
        displayedOptions = quest.TaskTitles;
        displayedOptions.Sort();

        int currentIndex = displayedOptions.IndexOf(property.stringValue);
        if (currentIndex == -1) currentIndex = 0;

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, displayedOptions.ToArray());
        property.stringValue = displayedOptions[selectedIndex];
        EditorGUI.EndProperty();
    }
}