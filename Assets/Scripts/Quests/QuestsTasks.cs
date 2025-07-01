using System;
using System.Collections.Generic;
using UnityEngine;

public enum StoryState { Pending, Active, Completed }

[Serializable]
public class BasicInfo
{
    public StoryState State = StoryState.Pending;
    public string Title = string.Empty;
    public string ShortDescription = string.Empty;
    public string LongDescription = string.Empty;
}
[Serializable]
public class Story
{
    public StoryState State = StoryState.Pending;
    public string GameState = string.Empty;
    public string CompletionEvent = string.Empty;
    public string Title = string.Empty;
    public string ShortDescription = string.Empty;
    public string LongDescription = string.Empty;
    public string CurrentQuestTitle = string.Empty;
    public List<string> QuestTitles = null;

    public Story()
    {
        this.GameState = string.Empty;
        this.CompletionEvent = string.Empty;
        this.Title = string.Empty;
        this.ShortDescription = string.Empty;
        this.LongDescription = string.Empty;
        this.CurrentQuestTitle = string.Empty;
        this.QuestTitles = new List<string>();
    }

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
