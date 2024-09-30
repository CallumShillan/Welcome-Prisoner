using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StoryState { Pending, Active, Completed }

[Serializable]
public class BasicInfo
{
    public StoryState State = StoryState.Pending;
    public QuestManager.OldGameEvent CompletionEvent = QuestManager.OldGameEvent.NONE;
    public string Title = string.Empty;
    public string ShortDescription = string.Empty;
    public string LongDescription = string.Empty;

    public BasicInfo(string title, string shortDescription, string longDescription, StoryState storyState, QuestManager.OldGameEvent gameEvent)
    {
        Title = title;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        State = storyState;
        CompletionEvent = gameEvent;
    }   
}
[Serializable]
public class Story
{
    public StoryState State = StoryState.Pending;
    //public QuestManager.OldGameEvent CompletionEvent = QuestManager.OldGameEvent.NONE;
    public string GameState = string.Empty;
    public string CompletionEvent = string.Empty;
    public string Title = string.Empty;
    public string ShortDescription = string.Empty;
    public string LongDescription = string.Empty;
    public string CurrentQuestTitle = string.Empty;
    public List<string> QuestTitles = null;

    public Story(string title = "", string shortDescription = "", string longDescription = "", string completionEvent = "", string gameState = "")
    {
        this.Title = title;
        this.ShortDescription = shortDescription;
        this.LongDescription = longDescription;
        this.CompletionEvent = completionEvent;
        this.GameState = gameState;
        this.QuestTitles = new List<string>();
    }
}

[Serializable]
public class Quest
{
    public StoryState State = StoryState.Pending;
    //public QuestManager.OldGameEvent CompletionEvent = QuestManager.OldGameEvent.NONE;
    public string GameState = string.Empty;
    public string CompletionEvent = string.Empty;
    public string Title = string.Empty;
    public string ShortDescription = string.Empty;
    public string LongDescription = string.Empty;
    public string CurrentTaskTitle = string.Empty;
    public List<string> TaskTitles = null;
    public bool IsActive;

    public Quest(string title, string shortDescription, string longDescription, string completionEvent = "", string gameState = "")
    {
        this.Title = title;
        this.ShortDescription = shortDescription;
        this.LongDescription = longDescription;
        this.GameState = gameState;
        this.CompletionEvent = completionEvent;
        this.TaskTitles = new List<string>();
        this.IsActive = false;
    }
}

[Serializable]
public class Task
{
    public StoryState State = StoryState.Pending;
    //public QuestManager.OldGameEvent CompletionEvent = QuestManager.OldGameEvent.NONE;
    public string GameState = string.Empty;
    public string CompletionEvent = string.Empty;
    public string Title = string.Empty;
    public string ShortDescription = string.Empty;
    public string LongDescription = string.Empty;
    public bool IsActive;

    public Task(string title, string shortDescription, string longDescription, string completionEvent = "", string gameState = "")
    {
        this.Title = title;
        this.ShortDescription = shortDescription;
        this.LongDescription = longDescription;
        this.CompletionEvent = completionEvent;
        this.GameState = gameState;
        this.IsActive = false;
    }
}
