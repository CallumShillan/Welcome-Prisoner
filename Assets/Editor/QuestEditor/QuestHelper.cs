using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public enum OperationResult { Success, TitleAlreadyExistsInQuestsList, TitleAlreadyExistsInDictionary }

[Serializable]
public class StoryQuestsTasks
{
    public Story theStory = new Story();
    public List<Quest> theQuests = new List<Quest>();
    public List<Task> theTasks = new List<Task>();
    public List<string> theCompletionEvents = new List<string>();
    public List<string> theGameStates = new List<string>();
}

public class QuestHelper
{
    // This is the Story information for the Scene
    private Story sceneStory;

    // A dictionary of Quests and Tasks so we can get the detail for a given Title
    private Dictionary<string, Quest> questDictionary = new Dictionary<string, Quest>();
    private Dictionary<string, Task> taskDictionary = new Dictionary<string, Task>();
    private List<string> completionEvents = new List<string>();
    private List<string> gameStates = new List<string>();

    // Make the above private objects publically accessible
    public Story SceneStory { get => sceneStory; }
    public Dictionary<string, Quest> QuestDictionary { get => questDictionary; }
    public Dictionary<string, Task> TaskDictionary {  get => taskDictionary; }
    public List<string> CompletionEvents { get => completionEvents; }
    public List<string> GameStates { get => gameStates; }

    public OperationResult AddQuest(Quest newQuest)
    {
        // If Scene Story already contains this quest, return false to indicate the quest could not be added
        if(sceneStory.QuestTitles.Contains(newQuest.Title))
        {
            return OperationResult.TitleAlreadyExistsInQuestsList;
        }

        // If there is already a quest of this name, return false to indicate the quest could not be added
        if (questDictionary.ContainsKey(newQuest.Title))
        {
            return OperationResult.TitleAlreadyExistsInDictionary;
        }

        // Add the quest to the list of quest names and the quest dictionary
        sceneStory.QuestTitles.Add(newQuest.Title);
        //questNames.Add(newQuest.Title);
        questDictionary.Add(newQuest.Title, newQuest);

        return OperationResult.Success;
    }

    public bool DeleteQuest(string questTitle)
    {
        bool removedFromStory = SceneStory.QuestTitles.Remove(questTitle);
        bool removedFromDictionary = QuestDictionary.Remove(questTitle);

        return (removedFromStory && removedFromDictionary);
    }

    public bool DeleteTask(string theTaskTitle)
    {
        bool taskRemovedFromDictionary = false;

        // Remove the Task from the dictionary of Tasks
        if (TaskDictionary.Remove(theTaskTitle))
        {
            taskRemovedFromDictionary = true;
            GameLog.NormalMessage($"Task '{theTaskTitle}' was deleted from the Task Dictionary");
        }
        else
        {
            GameLog.ErrorMessage($"Unable to find Task '{theTaskTitle}' in the dictionary of Tasks");
        }

        // Remove the task from quests
        foreach (Quest singleQuest in QuestDictionary.Values)
        {
            if (singleQuest.TaskTitles.Remove(theTaskTitle))
            {
                GameLog.NormalMessage($"Task '{theTaskTitle}' was deleted from Quest '{singleQuest.Title}'");
            }
        }

        return taskRemovedFromDictionary;
    }

    public bool AddTaskToQuest(string questTitle, Task task)
    {
        // If there is NOT already a quest of this name, return false to indicate the task could not be added
        if (false == sceneStory.QuestTitles.Contains(questTitle))
        {
            return false;
        }

        // If there is NOT already a quest of this name, return false to indicate the task could not be added
        if (false == questDictionary.ContainsKey(questTitle))
        {
            return false;
        }

        // Get the quest from the quest dictionary
        Quest quest = questDictionary[questTitle];

        // Add the new Task Title to the Quest's list of Task Titles
        quest.TaskTitles.Add(task.Title);

        return true;
    }

    public bool SaveSceneQuestsAndTasks()
    {
        bool returnValue = false;

        // Get the Assets folder location and the Active Scene name
        string assetsFolder = Application.dataPath;
        string sceneName = SceneManager.GetActiveScene().name;

        string sqtJsonLocation = $"{assetsFolder}/Resources/GameQuests/{sceneName}/{sceneName}.json";

        StoryQuestsTasks sqt = new StoryQuestsTasks();

        sqt.theStory.CompletionEvent = SceneStory.CompletionEvent;
        sqt.theStory.GameState = SceneStory.GameState;
        sqt.theStory.Title = SceneStory.Title;
        sqt.theStory.ShortDescription = SceneStory.ShortDescription;
        sqt.theStory.LongDescription = SceneStory.LongDescription;
        sqt.theStory.QuestTitles = SceneStory.QuestTitles;

        foreach (string questTitle in QuestDictionary.Keys)
        {
            sqt.theQuests.Add(QuestDictionary[questTitle]);
        }

        foreach (string taskTitle in TaskDictionary.Keys)
        {
            sqt.theTasks.Add(TaskDictionary[taskTitle]);
        }

        foreach (string completionEvent in CompletionEvents)
        {
            sqt.theCompletionEvents.Add(completionEvent);
        }

        foreach( string state in GameStates)
        {
            sqt.theGameStates.Add(state);
        }

        string jsonSqt = JsonUtility.ToJson(sqt);

        using (StreamWriter outputFile = new StreamWriter(sqtJsonLocation))
        {
            outputFile.WriteLine(jsonSqt);
        }

        returnValue = true;

        return returnValue;
    }

    public bool LoadSceneQuests()
    {
        bool sceneQuestsLoaded = false;
        try
        {
            // The StoryQuestsTasks class is used to hold persisted Story, Quests and Tasks data for the Scene
            StoryQuestsTasks sqt = LoadStoryQuestTaskInfo();

            if (sqt != null)
            {
                sceneStory = sqt.theStory;

                // The hydrated StoryQuestsTasks object is processed to provide data structures that can be more easily used by the Quest Editor

                // Clear out the objects used to provide easier access by the Quest Editor control fields
                questDictionary.Clear();
                taskDictionary.Clear();
                completionEvents.Clear();
                gameStates.Clear();

                // Build the list of Quests, the dictionary of Quests, and the Tasks in a given Quest 
                foreach (Quest singleQuest in sqt.theQuests)
                {
                    questDictionary.Add(singleQuest.Title, singleQuest);
                }

                // Build the dictionary of Tasks
                foreach (Task singleTask in sqt.theTasks)
                {
                    taskDictionary.Add(singleTask.Title, singleTask);
                }

                foreach (string singleCompletionEvent in sqt.theCompletionEvents)
                {
                    completionEvents.Add(singleCompletionEvent);
                }

                foreach (string singleGameState in sqt.theGameStates)
                {
                    gameStates.Add(singleGameState);
                }

                // Indicate we loaded the scene
                sceneQuestsLoaded = true;
            }
            else
            {
                sqt = new StoryQuestsTasks();
                GameLog.WarningMessage($"QuestHelper LoadSceneQuests() didn't hydrate data and returned an empty StoryQuestsTasks object.");
            }
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(null, $"QuestHelper LoadSceneQuests() exception: {ex.ToString()}");
        }

        sceneQuestsLoaded = true;
        //try
        //{
        //    StoryQuestsTasks sqt = new StoryQuestsTasks();
        //    sqt.theStory = new Story("Welcome, Prisoner!", "Short description", "Long description", StoryState.Active, QuestManager.OldGameEvent.NONE);
        //    for (int iCnt = 1; iCnt < 10; iCnt++)
        //    {
        //        Quest singleQuest = new Quest($"Quest {iCnt}", $"This is short description for Quest {iCnt}", $"This is the LONG description for Quest {iCnt}", StoryState.Pending, QuestManager.OldGameEvent.NONE);

        //        Task singleTask = new Task($"Task {iCnt}", $"This is short description for Task {iCnt}", $"This is LONG description for Task {iCnt}", StoryState.Pending, QuestManager.OldGameEvent.NONE);
        //        sqt.theTasks.Add(singleTask);

        //        singleQuest.TaskTitles = new List<string>();

        //        for (int jCnt = 0; jCnt < iCnt; jCnt++)
        //        {
        //            singleQuest.TaskTitles.Add(sqt.theTasks[jCnt].Title);
        //        }
        //        sqt.theQuests.Add(singleQuest);
        //        sqt.theStory.QuestTitles.Add(singleQuest.Title);
        //    }

        //    string jsonSqt = JsonUtility.ToJson(sqt);
        //}
        //catch (Exception ex)
        //{
        //    GameLog.ExceptionMessage(null, ex.ToString());
        //}

        return sceneQuestsLoaded;
    }

    private StoryQuestsTasks LoadStoryQuestTaskInfo()
    {
        StoryQuestsTasks sqt = null; 
        
        // Get the Active Scene name
        string sceneName = SceneManager.GetActiveScene().name;

        // Get the path to the Asset Resources for the Game Messages for this scene
        string jsonResourceFile = "GameQuests/" + sceneName + "/" + sceneName;

        // Load the file in the specified resource path
        TextAsset textAssetJsonStoryQuestsTasks = Resources.Load<TextAsset>(jsonResourceFile);

        string jsonSqt = textAssetJsonStoryQuestsTasks.text;
        sqt = JsonUtility.FromJson<StoryQuestsTasks>(jsonSqt);

        return sqt;
    }
}
