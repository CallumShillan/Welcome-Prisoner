using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all game messages, loading them per scene and tracking when they are shown.
/// </summary>
public class GameMessages : Singleton<GameMessages>
{
    /// <summary>
    /// All loaded game messages, keyed by their title.
    /// </summary>
    public IReadOnlyDictionary<string, GameMessage> AllGameMessages => allGameMessages;

    /// <summary>
    /// All shown game messages, sorted by when they were shown (descending).
    /// </summary>
    public IReadOnlyDictionary<DateTime, string> AllGameMessagesByWhenShown => allGameMessagesByWhenShown;

    private readonly Dictionary<string, GameMessage> allGameMessages = new Dictionary<string, GameMessage>();
    private readonly SortedDictionary<DateTime, string> allGameMessagesByWhenShown = new SortedDictionary<DateTime, string>(new DescendingDateComparer());

    /// <inheritdoc/>
    protected override void Awake()
    {
        // Ensure only one instance exists and persists across scenes.
        base.Awake();

        try
        {
            // Load all game messages for the current scene from Resources.
            string sceneName = SceneManager.GetActiveScene().name;
            string sceneMessageFolder = $"GameMessages/{sceneName}";
            TextAsset[] allSceneMessages = Resources.LoadAll<TextAsset>(sceneMessageFolder);

            // Return if no messages were found for the scene
            if (allSceneMessages == null || allSceneMessages.Length == 0)
            {
                GameLog.NormalMessage(this, $"No game messages found for scene: {sceneName}");
                return;
            }

            foreach (TextAsset sceneMessage in allSceneMessages)
            {
                if (!allGameMessages.TryAdd(sceneMessage.name, new GameMessage(sceneMessage.name, sceneMessage.text)))
                {
                    GameLog.WarningMessage(this, $"Duplicate GameMessage key detected: {sceneMessage.name}");
                }
            }

            GameLog.NormalMessage(this, $"{nameof(GameMessages)} Awake() finished");
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(this, $"{nameof(GameMessages)} Awake() exception: {ex}");
        }
    }

    /// <summary>
    /// Marks a message as shown and records the time.
    /// </summary>
    /// <param name="messageTitle">The title of the message to mark as shown.</param>
    public void SetShown(string messageTitle)
    {
        if (!allGameMessages.TryGetValue(messageTitle, out var message))
        {
            GameLog.WarningMessage(this, $"SetShown called with unknown messageTitle: {messageTitle}");
            return;
        }

        DateTime rightNow = DateTime.UtcNow;
        message.WhenShown = rightNow;
        message.HasBeenShown = true;
        allGameMessagesByWhenShown.Add(rightNow, messageTitle);
    }
}
