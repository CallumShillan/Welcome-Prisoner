using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMessage
{
    private string theMessage = string.Empty;
    private bool hasBeenShown = false;
    private DateTime whenShown = DateTime.MinValue;

    public GameMessage(string gameMessage)
    {
        theMessage = gameMessage;
        hasBeenShown = false;
        whenShown = DateTime.MinValue;
    }

    public string MessageText { get => theMessage; }
    public bool HasBeenShown { get => hasBeenShown; set => hasBeenShown = value; }
    public DateTime WhenShown { get => whenShown; set => whenShown = value; }
}

public class GameMessages : MonoBehaviour
{
    public static GameMessages Instance {get; private set;}
    public Dictionary<string, GameMessage> AllGameMessages { get => allGameMessages;  }

    private Dictionary<string, GameMessage> allGameMessages = new Dictionary<string, GameMessage>();
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
                allGameMessages.Add(sceneMessage.name, new GameMessage(sceneMessage.text));
            }
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(this, "GameDocuments Awake() exception: {0}", ex.ToString());
        }
        GameLog.NormalMessage(this, "GameDocuments Awake() finished");
    }

    public GameMessage Message(string messageKey)
    {
        if (false == allGameMessages.ContainsKey(messageKey))
        {
            return (null);
        }

        return allGameMessages[messageKey];
    }
}
