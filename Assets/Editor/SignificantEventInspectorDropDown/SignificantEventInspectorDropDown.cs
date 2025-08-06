using UnityEditor;
using UnityEngine;
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
        List<string> eventOptions = QuestHelper.CompletionEvents;

        int currentIndex = eventOptions.IndexOf(property.stringValue);
        if (currentIndex == -1) currentIndex = 0;

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, eventOptions.ToArray());
        property.stringValue = eventOptions[selectedIndex];
        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(QuestDropdownAttribute))]
public class QuestDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
//        QuestHelper questHelper = new QuestHelper();
//        questHelper.LoadStoryGraph(); // Ensure quests are loaded
        List<string> eventOptions = QuestHelper.QuestTitles;

        int currentIndex = eventOptions.IndexOf(property.stringValue);
        if (currentIndex == -1) currentIndex = 0;

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, eventOptions.ToArray());
        property.stringValue = eventOptions[selectedIndex];
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
        List<string> options = null;

//        QuestHelper questHelper = new QuestHelper();
//        questHelper.LoadStoryGraph();

        // Access the 'taskToInitiate' sibling property
        SerializedProperty questToInitiateProp = property.serializedObject.FindProperty("questToInitiate");

        // Optional: Get its value if it's a string, int, enum, etc.
        string questToInitiateValue = questToInitiateProp?.stringValue;


        Quest quest = QuestHelper.QuestDictionary[questToInitiateValue];
        options = quest.TaskTitles;

        int currentIndex = options.IndexOf(property.stringValue);
        if (currentIndex == -1) currentIndex = 0;

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, options.ToArray());
        property.stringValue = options[selectedIndex];
        EditorGUI.EndProperty();
    }
}