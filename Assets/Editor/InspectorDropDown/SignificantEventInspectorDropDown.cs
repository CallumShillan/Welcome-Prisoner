using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

/// <summary>
/// Custom property drawer for fields marked with <see cref="SignificantEventDropdownAttribute"/>.
/// Displays a dropdown menu in the inspector populated with significant event names from the global story list.
/// Events prefixed with "--" are prioritized to appear at the top, followed by a case-insensitive alphabetical sort.
/// Ensures the current property value is selected if valid, and updates the string property based on user selection.
/// </summary>
[CustomPropertyDrawer(typeof(SignificantEventDropdownAttribute))]
public class SignificantEventDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        List<string> displayedOptions = Globals.Instance.CompletionEvents;
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

/// <summary>
/// Custom property drawer for fields marked with <see cref="QuestDropdownAttribute"/>.
/// Renders a dropdown in the inspector populated with quest names from the global story list.
/// Automatically selects the current value if present, or defaults to the first entry.
/// Updates the serialized string property based on user selection.
/// </summary>
[CustomPropertyDrawer(typeof(QuestDropdownAttribute))]
public class QuestDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        List<string> displayedOptions = Globals.Instance.QuestTitles;
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
        displayedOptions = new List<string>(quest.TaskTitles);
        displayedOptions.Sort();

        int currentIndex = displayedOptions.IndexOf(property.stringValue);
        if (currentIndex == -1) currentIndex = 0;

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, displayedOptions.ToArray());
        property.stringValue = displayedOptions[selectedIndex];
        EditorGUI.EndProperty();
    }
}