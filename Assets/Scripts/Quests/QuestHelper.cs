using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private Story sceneStory = new Story();
    private Dictionary<string, Quest> questDictionary = new Dictionary<string, Quest>();
    private Dictionary<string, Task> taskDictionary = new Dictionary<string, Task>();
    private List<string> completionEvents = new List<string>();
    private List<string> gameStates = new List<string>();

    public Story SceneStory => sceneStory;
    public Dictionary<string, Quest> QuestDictionary => questDictionary;
    public Dictionary<string, Task> TaskDictionary => taskDictionary;
    public List<string> CompletionEvents => completionEvents;
    public List<string> GameStates => gameStates;

    public OperationResult AddQuest(Quest newQuest)
    {
        if (sceneStory.QuestTitles == null)
        {
            sceneStory.QuestTitles = new List<string>();
        }

        if (string.IsNullOrWhiteSpace(newQuest?.Title))
        {
            GameLog.ErrorMessage("Cannot add quest: Title is null or empty.");
            return OperationResult.TitleAlreadyExistsInQuestsList;
        }

        if (sceneStory.QuestTitles.Contains(newQuest.Title))
        {
            return OperationResult.TitleAlreadyExistsInQuestsList;
        }

        if (questDictionary.ContainsKey(newQuest.Title))
        {
            return OperationResult.TitleAlreadyExistsInDictionary;
        }

        sceneStory.QuestTitles.Add(newQuest.Title);
        questDictionary.Add(newQuest.Title, newQuest);

        return OperationResult.Success;
    }

    public bool DeleteQuest(string questTitle)
    {
        if (string.IsNullOrWhiteSpace(questTitle))
        {
            GameLog.ErrorMessage("Cannot delete quest: Title is null or empty.");
            return false;
        }

        bool removedFromStory = sceneStory.QuestTitles?.Remove(questTitle) ?? false;
        bool removedFromDictionary = questDictionary.Remove(questTitle);
        return removedFromStory && removedFromDictionary;
    }

    public bool DeleteTask(string taskTitle)
    {
        if (string.IsNullOrWhiteSpace(taskTitle))
        {
            GameLog.ErrorMessage("Cannot delete task: Title is null or empty.");
            return false;
        }

        bool removed = taskDictionary.Remove(taskTitle);
        if (removed)
        {
            GameLog.NormalMessage($"Task '{taskTitle}' was deleted from the Task Dictionary");
        }
        else
        {
            GameLog.ErrorMessage($"Unable to find Task '{taskTitle}' in the dictionary of Tasks");
        }

        foreach (KeyValuePair<string, Quest> questPair in questDictionary)
        {
            Quest quest = questPair.Value;
            if (quest.TaskTitles != null && quest.TaskTitles.Remove(taskTitle))
            {
                GameLog.NormalMessage($"Task '{taskTitle}' was deleted from Quest '{quest.Title}'");
            }
        }

        return removed;
    }

    public bool AddTaskToQuest(string questTitle, Task task)
    {
        if (string.IsNullOrWhiteSpace(questTitle) || task == null || string.IsNullOrWhiteSpace(task.Title))
        {
            GameLog.ErrorMessage("Cannot add task to quest: Invalid quest or task title.");
            return false;
        }

        Quest quest;
        if (!questDictionary.TryGetValue(questTitle, out quest))
        {
            GameLog.ErrorMessage($"Quest '{questTitle}' not found.");
            return false;
        }

        if (quest.TaskTitles == null)
        {
            quest.TaskTitles = new List<string>();
        }

        if (!quest.TaskTitles.Contains(task.Title))
        {
            quest.TaskTitles.Add(task.Title);
        }

        return true;
    }

    public bool SaveSceneQuestsAndTasks()
    {
        try
        {
            string assetsFolder = Application.dataPath;
            string sceneName = SceneManager.GetActiveScene().name;
            string sqtJsonLocation = Path.Combine(assetsFolder, "Resources", "GameQuests", sceneName, $"{sceneName}.json");

            StoryQuestsTasks sqt = new StoryQuestsTasks
            {
                theStory = sceneStory,
                theQuests = new List<Quest>(questDictionary.Values),
                theTasks = new List<Task>(taskDictionary.Values),
                theCompletionEvents = new List<string>(completionEvents),
                theGameStates = new List<string>(gameStates)
            };

            string jsonSqt = JsonUtility.ToJson(sqt, true);

            string dir = Path.GetDirectoryName(sqtJsonLocation);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(sqtJsonLocation, jsonSqt);

            return true;
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(null, $"SaveSceneQuestsAndTasks exception: {ex}");
            return false;
        }
    }

    public bool LoadSceneQuests()
    {
        try
        {
            StoryQuestsTasks sqt = LoadStoryQuestTaskInfo();
            if (sqt == null)
            {
                GameLog.WarningMessage("QuestHelper LoadSceneQuests() didn't hydrate data and returned an empty StoryQuestsTasks object.");
                return false;
            }

            sceneStory = sqt.theStory ?? new Story();
            questDictionary = new Dictionary<string, Quest>();
            taskDictionary = new Dictionary<string, Task>();
            completionEvents = new List<string>(sqt.theCompletionEvents ?? new List<string>());
            gameStates = new List<string>(sqt.theGameStates ?? new List<string>());

            if (sqt.theQuests != null)
            {
                foreach (Quest quest in sqt.theQuests)
                {
                    if (!string.IsNullOrWhiteSpace(quest.Title))
                    {
                        questDictionary[quest.Title] = quest;
                    }
                }
            }

            if (sqt.theTasks != null)
            {
                foreach (Task task in sqt.theTasks)
                {
                    if (!string.IsNullOrWhiteSpace(task.Title))
                    {
                        taskDictionary[task.Title] = task;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(null, $"QuestHelper LoadSceneQuests() exception: {ex}");
            return false;
        }
    }

    private StoryQuestsTasks LoadStoryQuestTaskInfo()
    {
        try
        {
            string sceneName = SceneManager.GetActiveScene().name;
            string jsonResourceFile = $"GameQuests/{sceneName}/{sceneName}";
            TextAsset textAssetJsonStoryQuestsTasks = Resources.Load<TextAsset>(jsonResourceFile);

            if (textAssetJsonStoryQuestsTasks == null)
            {
                return null;
            }

            return JsonUtility.FromJson<StoryQuestsTasks>(textAssetJsonStoryQuestsTasks.text);
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(null, $"LoadStoryQuestTaskInfo exception: {ex}");
            return null;
        }
    }
}
