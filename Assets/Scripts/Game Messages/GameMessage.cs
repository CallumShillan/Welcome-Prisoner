using System;

/// <summary>
/// Represents a message shown in the game, with title, content, and display tracking.
/// </summary>
public class GameMessage
{
    public string MessageTitle { get; }
    public string MessageText { get; }
    public bool HasBeenShown { get; set; }
    public DateTime WhenShown { get; set; }

    public GameMessage()
    {
    }

    public GameMessage(string messageTitle, string messageText)
    {
        MessageTitle = messageTitle;
        MessageText = messageText;
        HasBeenShown = false;
        WhenShown = DateTime.MinValue;
    }
}
