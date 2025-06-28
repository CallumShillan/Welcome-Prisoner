using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMessage
{
    private string messageTitle = string.Empty;
    private string messageContent = string.Empty;
    private bool hasBeenShown = false;
    private DateTime whenShown = DateTime.MinValue;

    public GameMessage()
    {
    }
    public GameMessage(string gameMessageTitle, string gameMessageContent)
    {
        messageTitle = gameMessageTitle;
        messageContent = gameMessageContent;
        hasBeenShown = false;
        whenShown = DateTime.MinValue;
    }

    public string MessageTitle { get => messageContent; }
    public string MessageText { get => messageContent; }
    public bool HasBeenShown { get => hasBeenShown; set => hasBeenShown = value; }
    public DateTime WhenShown { get => whenShown; set => whenShown = value; }
}

public class DescendingDateComparer : IComparer<DateTime>
{
    public int Compare(DateTime x, DateTime y)
    {
        return y.CompareTo(x);
        // Reverse the comparison
    }
}

public class GameMessages : MonoBehaviour
{
    public static GameMessages Instance {get; private set;}
    public Dictionary<string, GameMessage> AllGameMessages { get => allGameMessages;  }
    public SortedDictionary<DateTime, string> AllGameMessagesByWhenShown { get => allGameMessagesByWhenShown; }

    private Dictionary<string, GameMessage> allGameMessages = new Dictionary<string, GameMessage>();

    private SortedDictionary<DateTime, string> allGameMessagesByWhenShown = new SortedDictionary<DateTime, string>(new DescendingDateComparer());

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

            // Get the Active Scene name
            string sceneName = SceneManager.GetActiveScene().name;

            // Get the path to the Asset Resources for the Game Messages for this scene
            string sceneMessageFolder = "GameMessages/" + sceneName;

            // Load all the files in the specified resource path
            TextAsset[] allSceneMessages = Resources.LoadAll<TextAsset>(sceneMessageFolder);

            // Add them to the dictionary of Game Messages
            foreach (TextAsset sceneMessage in allSceneMessages)
            {
                allGameMessages.Add(sceneMessage.name, new GameMessage(sceneMessage.name, sceneMessage.text));
            }
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(this, "GameDocuments Awake() exception: {0}", ex.ToString());
        }
        GameLog.NormalMessage(this, "GameDocuments Awake() finished");
    }

    public void SetShown(string messageTitle)
    {
        GameMessage message = new GameMessage();

        DateTime rightNow = DateTime.Now;

        if(allGameMessages.TryGetValue(messageTitle, out message))
        {
            message.WhenShown = rightNow;
            message.HasBeenShown = true;
        }

        allGameMessagesByWhenShown.Add(rightNow, messageTitle);
    }
}
