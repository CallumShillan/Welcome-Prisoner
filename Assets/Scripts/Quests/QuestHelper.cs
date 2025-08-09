using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public enum OperationResult { Success, TitleAlreadyExistsInQuestsList, TitleAlreadyExistsInDictionary }

[Serializable]
public class StoryRegistry
{
    public Story theStory = new Story();
    public List<Quest> theQuests = new List<Quest>();
    public List<Task> theTasks = new List<Task>();
    public List<string> theCompletionEvents = new List<string>();
    public List<string> theGameStates = new List<string>();
}

public static class QuestHelper
{
    private static Story sceneStory = new Story();
    private static Dictionary<string, Quest> questDictionary = new Dictionary<string, Quest>();
    private static Dictionary<string, Task> taskDictionary = new Dictionary<string, Task>();
    private static List<string> completionEvents = new List<string>();
    private static List<string> gameStates = new List<string>();

    public static Story SceneStory => sceneStory;
    public static Dictionary<string, Quest> QuestDictionary => questDictionary;
    public static Dictionary<string, Task> TaskDictionary => taskDictionary;
    public static List<string> CompletionEvents => completionEvents;
    public static List<string> GameStates => gameStates;
    public static List<string> QuestTitles => sceneStory.QuestTitles;
    public static List<string> TaskTitles => sceneStory.TaskTitles;

    private static StoryRegistry storyRegistry = null;

    private static FileSystemWatcher jsonStoryRegistryWatcher;

    private static string jsonRegistryAsset = string.Empty;

    static QuestHelper()
    {
        // Initialize the quest helper when the class is first accessed
        LoadStoryGraph(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public static bool SaveStoryGraph()
    {
        try
        {
            string assetsFolder = Application.dataPath;
            string sceneName = SceneManager.GetActiveScene().name;
            string sqtJsonLocation = Path.Combine(assetsFolder, "Resources", "GameQuests", sceneName, $"{sceneName}.json");

            StoryRegistry storyRegistry = new StoryRegistry
            {
                theStory = sceneStory,
                theQuests = new List<Quest>(questDictionary.Values),
                theTasks = new List<Task>(taskDictionary.Values),
                theCompletionEvents = new List<string>(completionEvents),
                theGameStates = new List<string>(gameStates)
            };

            string jsonSqt = JsonUtility.ToJson(storyRegistry, true);

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
            GameLog.ExceptionMessage(null, $"SaveStoryGraph exception: {ex}");
            return false;
        }
    }

    public static bool LoadStoryGraph(string sceneName)
    {
        bool returnValue = false;
        try
        {
            storyRegistry = LoadStoryRegistry(sceneName);
            if (storyRegistry == null)
            {
                GameLog.WarningMessage("QuestHelper LoadStoryGraph() didn't hydrate data and returned an empty StoryQuestsTasks object.");
                return false;
            }

            sceneStory = storyRegistry.theStory ?? new Story();
            questDictionary = new Dictionary<string, Quest>();
            taskDictionary = new Dictionary<string, Task>();
            completionEvents = new List<string>(storyRegistry.theCompletionEvents ?? new List<string>());
            gameStates = new List<string>(storyRegistry.theGameStates ?? new List<string>());

            if (storyRegistry.theQuests != null)
            {
                foreach (Quest quest in storyRegistry.theQuests)
                {
                    if (!string.IsNullOrWhiteSpace(quest.Title))
                    {
                        questDictionary[quest.Title] = quest;
                    }
                }
            }

            if (storyRegistry.theTasks != null)
            {
                foreach (Task task in storyRegistry.theTasks)
                {
                    if (!string.IsNullOrWhiteSpace(task.Title))
                    {
                        taskDictionary[task.Title] = task;
                    }
                }
            }
            returnValue = true;
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(null, $"QuestHelper LoadSceneQuests() exception: {ex}");
        }
        finally
        {
            GameLog.NormalMessage($"QuestHelper LoadSceneQuests() completed with 'success' return value: {returnValue}");
        }
        return returnValue;
    }

    private static StoryRegistry LoadStoryRegistry(string sceneName)
    {
        try
        {
            jsonRegistryAsset = Path.Combine("GameQuests", sceneName, sceneName);
            TextAsset textAssetJsonStoryQuestsTasks = Resources.Load<TextAsset>(jsonRegistryAsset);

            if (textAssetJsonStoryQuestsTasks == null)
            {
                return null;
            }

            return JsonUtility.FromJson<StoryRegistry>(textAssetJsonStoryQuestsTasks.text);
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(null, $"LoadStoryQuestTaskInfo exception: {ex}");
            return null;
        }
    }
}
