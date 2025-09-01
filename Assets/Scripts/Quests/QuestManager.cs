using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages quests and tasks for the current scene, including state transitions and activity markers.
/// </summary>
[Serializable]
public class QuestManager : MonoBehaviour
{
    public enum SignificantEventState
    {
        SignificantEventUnknown,
        SignificantEventOccurred,
        SignificantEventNotOccurred
    }

    [SerializeField]
    [Tooltip("The tag used for all Quest or Activity Markers")]
    private string questActivityMarkerTag;

    public static QuestManager Instance { get; private set; }

    //private static readonly QuestHelper questHelper = new QuestHelper();

    private static string currentQuestName = string.Empty;
    private static string currentTaskName = string.Empty;

    private static readonly SortedDictionary<string, DateTime> allSignificantEvents = new();

    private static GameObject[] allActivityMarkerGameObjects = Array.Empty<GameObject>();
    private static Dictionary<string, GameObject> allActivityMarkers = new();

    /// <summary>
    /// Initializes the QuestManager instance and loads all activity markers in the scene.
    /// </summary>
    private void Awake()
    {
        // Get all activity markers in the scene
        allActivityMarkerGameObjects = GameObject.FindGameObjectsWithTag(questActivityMarkerTag) ?? Array.Empty<GameObject>();
        allActivityMarkers = new Dictionary<string, GameObject>(allActivityMarkerGameObjects.Length);

        // Mark all activity markers as inactive and store them in the dictionary
        foreach (GameObject marker in allActivityMarkerGameObjects)
        {
            marker.SetActive(false);
            allActivityMarkers[marker.name] = marker;
        }

        //questHelper.LoadStoryGraph();
    }

    /// <summary>
    /// The story associated with the current scene.
    /// </summary>
    public static Story SceneStory => QuestHelper.SceneStory;

    /// <summary>
    /// The name of the current quest.
    /// </summary>
    public static string CurrentQuestName
    {
        get => currentQuestName;
        set
        {
            if (!QuestHelper.QuestDictionary.ContainsKey(value))
            {
                GameLog.ErrorMessage($"QuestManager CurrentQuestName: QuestDictionary does not have a quest called '{value}'");
                return;
            }
            currentQuestName = value;
            QuestHelper.QuestDictionary[currentQuestName].State = StoryState.Active;
        }
    }

    /// <summary>
    /// The name of the current task.
    /// </summary>
    public static string CurrentTaskName
    {
        get => currentTaskName;
        set
        {
            if (!QuestHelper.TaskDictionary.ContainsKey(value))
            {
                GameLog.ErrorMessage($"QuestManager CurrentTaskName: TaskDictionary does not have a task called '{value}'");
                return;
            }

            currentTaskName = value;
            QuestHelper.TaskDictionary[currentTaskName].State = StoryState.Active;

            if ((allActivityMarkers != null) && (allActivityMarkers.TryGetValue(currentTaskName, out GameObject marker)))
            {
                marker.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Gets the current quest.
    /// </summary>
    public static Quest GetCurrentQuest => GetQuest(CurrentQuestName);

    /// <summary>
    /// Returns a list of quest titles that are either active or completed.
    /// </summary>
    public static List<string> ActiveOrCompletedQuests
    {
        get
        {
            List<string> result = new List<string>(QuestHelper.QuestDictionary.Count);
            foreach (Quest quest in QuestHelper.QuestDictionary.Values)
            {
                if ((quest.State == StoryState.Active) || (quest.State == StoryState.Completed))
                {
                    result.Add(quest.Title);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Returns a list of quest titles that are not yet completed.
    /// </summary>
    public static List<string> OutstandingQuests
    {
        get
        {
            List<string> result = new List<string>(QuestHelper.QuestDictionary.Count);
            foreach (Quest quest in QuestHelper.QuestDictionary.Values)
            {
                if (quest.State != StoryState.Completed)
                {
                    result.Add(quest.Title);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Gets a quest by its title.
    /// </summary>
    public static Quest GetQuest(string questTitle) =>
        QuestHelper.QuestDictionary.TryGetValue(questTitle, out Quest quest) ? quest : null;

    /// <summary>
    /// Gets a task by its title.
    /// </summary>
    public static Task GetTask(string taskTitle) =>
        QuestHelper.TaskDictionary.TryGetValue(taskTitle, out Task task) ? task : null;

    /// <summary>
    /// Gets the state of a significant event based on its title.
    /// </summary>
    /// <param name="significantEvent"></param>
    /// <returns></returns>
    public static SignificantEventState GetSignificantEventState(string significantEvent)
    {
        if (string.IsNullOrEmpty(significantEvent))
        {
            return SignificantEventState.SignificantEventUnknown;
        }
        return allSignificantEvents.ContainsKey(significantEvent) ? SignificantEventState.SignificantEventOccurred : SignificantEventState.SignificantEventNotOccurred;
    }

    public static List<string> GetAllSignificantEvents()
    {
        List<string> result = new List<string>(allSignificantEvents.Keys);
        result.Sort();
        return result;
    }

    /// <summary>
    /// Handles a significant event, updating quest and task states as needed.
    /// </summary>
    public static void HandleSignificantEvent(string significantEvent)
    {
        if (string.IsNullOrEmpty(significantEvent) || string.Equals(significantEvent, "--IRRELEVANT--"))
        {
            return;
        }

        if (!allSignificantEvents.ContainsKey(significantEvent))
        {
            allSignificantEvents.Add(significantEvent, DateTime.Now);
        }

        foreach (string questTitle in QuestManager.OutstandingQuests)
        {
            Quest singleQuest = GetQuest(questTitle);
            if ((singleQuest == null) || (singleQuest.TaskTitles == null))
            {
                continue;
            }

            bool yetToSetActivityMarker = true;
            bool allSubtasksHaveBeenCompleted = true;

            foreach (string singleTaskTitle in singleQuest.TaskTitles)
            {
                Task singleTask = GetTask(singleTaskTitle);
                if (singleTask == null)
                {
                    continue;
                }

                if (allSignificantEvents.ContainsKey(singleTask.CompletionEvent))
                {
                    singleTask.State = StoryState.Completed;
                    if ((allActivityMarkers != null) && (allActivityMarkers.TryGetValue(singleTask.Title, out GameObject marker)))
                    {
                        marker.SetActive(false);
                    }
                }

                if (yetToSetActivityMarker && (singleTask.State != StoryState.Completed))
                {
                    yetToSetActivityMarker = false;
                    if ((allActivityMarkers != null) && (allActivityMarkers.TryGetValue(singleTask.Title, out GameObject marker)))
                    {
                        marker.SetActive(true);
                    }
                    singleQuest.CurrentTaskTitle = singleTask.Title;
                    singleTask.State = StoryState.Active;
                }

                if (singleTask.State != StoryState.Completed)
                {
                    allSubtasksHaveBeenCompleted = false;
                }
            }

            if (allSubtasksHaveBeenCompleted)
            {
                singleQuest.State = StoryState.Completed;
            }
        }
    }
}