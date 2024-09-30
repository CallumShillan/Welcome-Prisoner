//using System;
//using System.Collections.Generic;

////public enum TaskId { None, UploadRedRobotCoreInstructions }
//public enum TaskState { Pending, Active, Completed }
////public enum TaskItem { Pda, ConferenceRoomKeyCode, RoleOwnerOfficesKeyCode, PresentToRedRobots, SilencedAlarm }

//[Serializable]
//public class GameTask
//{
//    public string UniqueId;
//    public TaskState State;
//    public string Title;
//    public string Description;
//    public string CurrentSubtask = string.Empty;
//    public List<string> Subtasks;
//    public List<string> PrequisiteItems;
//    public List<string> RewardItems;
//    public QuestManager.GameEvent CompletionEvent;

//    public GameTask(TaskState state, string title, string description, QuestManager.GameEvent completetionEvent)
//    {
//        UniqueId = Guid.NewGuid().ToString();
//        State = state;
//        Title = title;
//        Description = description;
//        CurrentSubtask = string.Empty;
//        Subtasks = new List<string>();
//        PrequisiteItems = new List<string>();
//        RewardItems = new List<string>();
//        CompletionEvent = completetionEvent;
//    }
//    public GameTask(string uniqueID, TaskState state, string title, string description, QuestManager.GameEvent completetionEvent)
//    {
//        UniqueId = uniqueID;
//        State = state;
//        Title = title;
//        Description = description;
//        CurrentSubtask = string.Empty;
//        Subtasks = new List<string>();
//        PrequisiteItems = new List<string>();
//        RewardItems = new List<string>();
//        CompletionEvent = completetionEvent;
//    }
//}

