using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorUtilities
{
    private List<string> questTitles = null;
    public List<string> QuestTitles
    {
        get
        {
            if (questTitles == null)
            {
                questTitles = QuestHelper.QuestTitles;
            }
            return questTitles;
        }
    }

    /// <summary>
    /// Not needed as TaskDropdownDrawer gets tasks from the selected quest
    /// </summary>
    //private List<string> taskTitles = null;
    //public List<string> TaskTitles
    //{
    //    get
    //    {
    //        if (taskTitles == null)
    //        {
    //            taskTitles = QuestHelper.TaskTitles;
    //        }
    //        return taskTitles;
    //    }
    //}

    private List<string> completionEvents = null;
    public List<string> CompletionEvents
    {
        get
        {
            if (completionEvents == null || completionEvents.Count == 0)
            {
                completionEvents = QuestHelper.CompletionEvents;
            }
            return completionEvents;
        }
    }
    private static readonly Lazy<EditorUtilities> lazy =
        new Lazy<EditorUtilities>(() => new EditorUtilities());

    public static EditorUtilities Instance { get { return lazy.Value; } }

    private EditorUtilities()
    {
    }

}
