using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Text;
using System.IO;
using UnityEngine.UIElements;
using System.Security.Cryptography;

[Serializable]
public class QuestManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The tag used for all Quest or Activity Markers")]
    private string questActivityMarkerTag;

    public static QuestManager Instance { get; private set; }

    public enum OldGameEvent { NONE, WELCOME_MESSAGE_SHOWN, PDA_USED, ROBOT_OWNERS_OFFICE_UNLOCKED, READ_RED_ROBOT_DATA_BRICK, READ_CHIEF_SECURITY_OFFICER_DATABRICK, READ_ROBOT_OWNER_WHITEBOARD, CONFERENCE_ROOM_UNLOCKED }

    private static QuestHelper questHelper = new QuestHelper();

    private static string currentQuestName = string.Empty;
    private static string currentTaskName = string.Empty;

    private static SortedDictionary<string, DateTime> allSignificantEvents = new SortedDictionary<string, DateTime>();

    private static GameObject[] allActivityMarkerGameObjects;
    private static Dictionary<string, GameObject> allActivityMarkers;

    private static List<string> activeOrCompletedQuests = null;
    private static List<string> outstandingQuests = null;

    public QuestManager()
    {
    }

    public static Story SceneStory
    {
        get
        {
            return questHelper.SceneStory;
        }
    }

    public static string CurrentQuestName
    {
        get
        {
            return currentQuestName;
        }
        set
        {
            if (false == questHelper.QuestDictionary.ContainsKey(value))
            {
                GameLog.ErrorMessage($"QuestManager CurrentQuestName: QuestDictionary does not have a quest called '{value}'");
                return;
            }

            currentQuestName = value;
            questHelper.QuestDictionary[currentQuestName].State = StoryState.Active;
        }
    }
    public static string CurrentTaskName
    {
        get
        {
            return currentTaskName;
        }
        set
        {
            if (false == questHelper.TaskDictionary.ContainsKey(value))
            {
                GameLog.ErrorMessage($"QuestManager CurrentTaskName: TaskDictionary does not have a task called '{value}'");
                return;
            }

            currentTaskName = value;
            questHelper.TaskDictionary[currentTaskName].State = StoryState.Active;
        }
    }

    public static Quest GetCurrentQuest
    {
        get
        {
            return GetQuest(CurrentQuestName);
        }
    }

    public static List<string> ActiveOrCompletedQuests
    {
        get
        {
            // Get a new List<string> for all the quest titles
            activeOrCompletedQuests = new List<string>(questHelper.QuestDictionary.Values.Count);

            // Loop through all the Quests in the Quest Dictionary
            foreach (Quest singleQuest in questHelper.QuestDictionary.Values)
            {
                if(singleQuest.State == StoryState.Active || singleQuest.State == StoryState.Completed)
                {
                    // Add the title of a single quest to the list of quest titles
                    activeOrCompletedQuests.Add(singleQuest.Title);
                }
            }

            // return the list of quests
            return activeOrCompletedQuests;
        }
    }

    public static List<string> OutstandingQuests
    {
        get
        {
            // Get a new List<string> for all the quest titles
            outstandingQuests = new List<string>(questHelper.QuestDictionary.Values.Count);

            // Loop through all the Quests in the Quest Dictionary
            foreach (Quest singleQuest in questHelper.QuestDictionary.Values)
            {
                if (singleQuest.State != StoryState.Completed)
                {
                    // Add the title of a single quest to the list of quest titles
                    outstandingQuests.Add(singleQuest.Title);
                }
            }

            // return the list of quests
            return outstandingQuests;
        }
    }
    public static Quest GetQuest(string questTitle)
    {
        // Check if the dictionary has the 
        if (questHelper.QuestDictionary.ContainsKey(questTitle))
        {
            return questHelper.QuestDictionary[questTitle];
        }
        else
        {
            return null;
        }
    }

    public static Task GetTask(string taskTitle)
    {
        if(questHelper.TaskDictionary.ContainsKey(taskTitle))
        {
            return(questHelper.TaskDictionary[taskTitle]);
        }
        else
        {
            return null;
        }
    }

    //public static List<string> AllQuests()
    //{
    //    List<string> allActiveQuests = new List<string>();

    //    foreach(Quest singleQuest in questHelper.QuestDictionary.Values)
    //    {
    //        allActiveQuests.Add(singleQuest.Title);
    //    }
    //    return allActiveQuests;
    //}

    public static void HandleSignificantEvent(string significantEvent)
    {
        // Return immediately if there is no significant event
        if(string.IsNullOrEmpty(significantEvent))
        {
            return;
        }

        // Add a datetime stamp record of when this event first occurred
        if (false == allSignificantEvents.ContainsKey(significantEvent))
        {
            allSignificantEvents.Add(significantEvent, DateTime.Now);
        }

        // Get the current quest
        Quest currentQuest = QuestManager.GetCurrentQuest;

        string currentQuestTitle = string.Empty;
        if (currentQuest is not null)
        {
            currentQuestTitle = currentQuest.Title;
        }

        bool yetToSetActivityMarker = true;
        bool allSubtasksHaveBeenCompleted = true;
        Quest singleQuest = null;

        // Loop through all outstanding quests and see if the Significant Event relates to any Quest or Task - if it does, mark it as complete
        foreach (string questTitle in QuestManager.OutstandingQuests)
        {
            singleQuest = QuestManager.GetQuest(questTitle);

            // This may have been set false
            allSubtasksHaveBeenCompleted = true;

            // Step through the subtasks for this active task
            foreach (string singleTaskTitle in singleQuest.TaskTitles)
            {
                Task singleTask = QuestManager.GetTask(singleTaskTitle);
                if (singleTask is not null)
                {
                    // Check if we have an event that matches the completion event for this task
                    if (allSignificantEvents.ContainsKey(singleTask.CompletionEvent))
                    {
                        singleTask.State = StoryState.Completed;
                        allActivityMarkers[singleTask.Title].SetActive(false);
                    }

                    //// For the current quest, we want to set the next Activity Marker to be active
                    //if (string.Equals(questTitle, currentQuestTitle, StringComparison.InvariantCultureIgnoreCase))
                    //{
                        // If we have an outstanding activity, set it active and remember we have done so
                        if (yetToSetActivityMarker && singleTask.State != StoryState.Completed)
                        {
                            yetToSetActivityMarker = false;
                            allActivityMarkers[singleTask.Title].SetActive(true);
                            singleQuest.CurrentTaskTitle = singleTask.Title;
                            singleTask.State = StoryState.Active;
                        }

                        // See if we have an uncompleted task activity for the Quest
                        if (singleTask.State != StoryState.Completed)
                        {
                            allSubtasksHaveBeenCompleted = false;
                        }
                    //}

                }
            }

            // If all subtasks are complete, the quest should be marked as complete
            if (allSubtasksHaveBeenCompleted)
            {
                singleQuest.State = StoryState.Completed;
            }

            // Reset the flag ready for the next quest in the for loop
            allSubtasksHaveBeenCompleted = true;

        }
    }

    void Awake()
    {
        try
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            // Remember the Active Scene name
            string sceneName = SceneManager.GetActiveScene().name;

            // Get the Activity Markers
            allActivityMarkerGameObjects = GameObject.FindGameObjectsWithTag(questActivityMarkerTag);

            // Instantiate a new Dictionary of all the Activity Markers
            allActivityMarkers = new Dictionary<string, GameObject>(allActivityMarkerGameObjects.Length);

            // An individual Activity Marker
            GameObject individualActivitymarker;

            // Step through all Activity Markers
            for ( int iCnt = 0; iCnt < allActivityMarkerGameObjects.Length; iCnt++)
            {
                // Get the Activity Marker, set it to be 
                individualActivitymarker = allActivityMarkerGameObjects[iCnt];

                // Instruct the Unity engine to neither render nor process the Activity Marker
                individualActivitymarker.SetActive(false);

                // Add it to the Dictionary of Activty Markers
                allActivityMarkers.Add(individualActivitymarker.name, individualActivitymarker);
            }

            // Populate all the quests
            questHelper.LoadSceneQuests();
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(this, "QuestManager Awake() exception: {0}", ex.ToString());
        }
        GameLog.NormalMessage(this, "QuestManager Awake() finished");
    }
}