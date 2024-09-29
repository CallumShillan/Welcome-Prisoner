using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public enum EditorTaskState { Pending, Active, Completed }
public enum ListViewFoldoutState { Collapsed, Expanded }

public class QuestEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private bool problemHookingUpControls = false;

    private TextField sceneState = null;
    private TextField questState = null;
    private TextField taskState = null;

    private TextField sceneCompletionEvent = null;
    private TextField questCompletionEvent = null;
    private TextField taskCompletionEvent = null;

    private Button chooseSceneCompletionEvent = null;
    private Button chooseQuestCompletionEvent = null;
    private Button chooseTaskCompletionEvent = null;

    private Button chooseSceneState = null;
    private Button chooseQuestState = null;
    private Button chooseTaskState = null;

    private ListView listviewSceneCompletionEvent = null;
    private ListView listviewQuestCompletionEvent = null;
    private ListView listviewTaskCompletionEvent = null;

    private TextField sceneTitle = null;
    private TextField questTitle = null;
    private TextField taskTitle = null;

    private TextField sceneShortDescription = null;
    private TextField questShortDescription = null;
    private TextField taskShortDescription = null;

    private TextField sceneLongDescription = null;
    private TextField questLongDescription = null;
    private TextField taskLongDescription = null;

    private ListView listviewSceneQuests = null;
    private ListView listviewQuestTasks = null;
    private ListView listviewCompletionEvents = null;

    private TextField taskCurrentSubtask = null;
    private TreeView treeviewPrerequisiteItems = null;

    private ToolbarButton toolbarButtonReload = null;
    private ToolbarButton toolbarButtonSave = null;
    private ToolbarButton toolbarButtonNewQuest = null;
    private ToolbarButton toolbarButtonSaveQuest = null;
    private ToolbarButton toolbarButtonDeleteQuest = null;
    private ToolbarButton toolbarButtonNewTask = null;
    private ToolbarButton toolbarButtonDeleteTask = null;
    private ToolbarButton toolbarButtonChooseTask = null;
    private ToolbarButton toolbarButtonAddTaskToQuests = null;

    private Foldout listviewFoldout = null;


    private List<Quest> allGameTasksInScene = null;

    private int id = 0;

    private string selectedTreeviewTaskTitle = string.Empty;

    private Dictionary<string, Task> gameTaskDictionary = null;
    private List<string> subtasksOfSelectedTask = new List<string>();

    private QuestHelper questHelper = null;

    private string focussedControl = string.Empty;

    [MenuItem("Tools/Quest Editor")]
    public static void ShowQuestEditor()
    {
        QuestEditor questEditorWindow = GetWindow<QuestEditor>();
        questEditorWindow.titleContent = new GUIContent("Quest Editor");

        // Calculate position for the new window to be in the center of the screen
        float width = 2000;
        float height = 1000;
        float x = (Screen.currentResolution.width - width) / 2;
        float y = (Screen.currentResolution.height - height) / 2;

        // Set the position and size of the window
        questEditorWindow.position = new Rect(x, y, width, height);
    }

    public void Awake()
    {
        questHelper = new QuestHelper();
        questHelper.LoadSceneQuests();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        //VisualElement root = rootVisualElement;

        // Instantiate Quest Editor UXML
        VisualElement uxmlQuestEditor = m_VisualTreeAsset.Instantiate();
        //root.Add(uxmlQuestEditor);
        rootVisualElement.Add(uxmlQuestEditor);

        // Wire up the user controls: treeview, listview, textfields, command buttons and so on
        Toolbar t = rootVisualElement.Q<Toolbar>("StoryToolbar");
        HookupUserInterfaceControls();

        // If we got a Scene Story
        if(questHelper.SceneStory is not null)
        {
            // Set the scene fields
            sceneTitle.value = questHelper.SceneStory.Title;
            sceneShortDescription.value = questHelper.SceneStory.ShortDescription;
            sceneLongDescription.value = questHelper.SceneStory.LongDescription;
            sceneCompletionEvent.value = questHelper.SceneStory.CompletionEvent;
            sceneState.value = questHelper.SceneStory.GameState;
        }

        // If the Quest had Tasks
        if(questHelper.SceneStory.QuestTitles.Count > 0)
        {
            // Hookup the ListView of Quests in the Scene Story
            HookupControl(out listviewSceneQuests, "listviewSceneQuests", rootVisualElement.name, ListViewFoldoutState.Expanded, questHelper.SceneStory.QuestTitles, OnQuestItemsChosen);
        }

    }

    #region Hookup User Interface Controls

    void HookupUserInterfaceControls()
    {
        string rootVisualElementName = rootVisualElement.name;

        // The Scene, Quest and Task state text fields
        HookupControl(out sceneState, "txtSceneState", rootVisualElementName);
        HookupControl(out questState, "txtQuestState", rootVisualElementName);
        HookupControl(out taskState, "txtTaskState", rootVisualElementName);

        // The Scene, Quest and Task select state buttons
        HookupControl(out chooseSceneState, "buttonChooseSceneState", rootVisualElementName, ChooseSceneState);
        HookupControl(out chooseQuestState, "buttonChooseQuestState", rootVisualElementName, ChooseQuestState);
        HookupControl(out chooseTaskState, "buttonChooseTaskState", rootVisualElementName, ChooseTaskState);

        // The Scene, Quest and Task completion event text fields
        HookupControl(out sceneCompletionEvent, "txtSceneCompletionEvent", rootVisualElementName);
        HookupControl(out questCompletionEvent, "txtQuestCompletionEvent", rootVisualElementName);
        HookupControl(out taskCompletionEvent, "txtTaskCompletionEvent", rootVisualElementName);

        // The Scene, Quest and Task select completion event buttons
        HookupControl(out chooseSceneCompletionEvent, "buttonChooseSceneCompletionEvent", rootVisualElementName, ChooseSceneCompletionEvent);
        HookupControl(out chooseQuestCompletionEvent, "buttonChooseQuestCompletionEvent", rootVisualElementName, ChooseQuestCompletionEvent);
        HookupControl(out chooseTaskCompletionEvent, "buttonChooseTaskCompletionEvent", rootVisualElementName, ChooseTaskCompletionEvent);

        // The Scene, Quest and Task title text fields
        HookupControl(out sceneTitle, "txtSceneTitle", rootVisualElementName);
        HookupControl(out questTitle, "txtQuestTitle", rootVisualElementName);
        HookupControl(out taskTitle, "txtTaskTitle", rootVisualElementName);

        // The Scene, Quest and Task Short Description text fields
        HookupControl(out sceneShortDescription, "txtSceneShortDescription", rootVisualElementName);
        HookupControl(out questShortDescription, "txtQuestShortDescription", rootVisualElementName);
        HookupControl(out taskShortDescription, "txtTaskShortDescription", rootVisualElementName);

        // The Scene, Quest and Task Long Description text fields
        HookupControl(out sceneLongDescription, "txtSceneLongDescription", rootVisualElementName);
        HookupControl(out questLongDescription, "txtQuestLongDescription", rootVisualElementName);
        HookupControl(out taskLongDescription, "txtTaskLongDescription", rootVisualElementName);

        // The Listviews for the Quests and Tasks
        HookupControl(out listviewSceneQuests, "listviewSceneQuests", rootVisualElementName, ListViewFoldoutState.Expanded, OnQuestItemsChosen);
        HookupControl(out listviewQuestTasks, "listviewQuestTasks", rootVisualElementName, ListViewFoldoutState.Expanded, OnTaskItemsChosen);

        // The Toolbar Buttons
        HookupControl(out toolbarButtonReload, "toolbarButtonReload", rootVisualElementName, Reload);
        HookupControl(out toolbarButtonSave, "toolbarButtonSave", rootVisualElementName, Save);
        HookupControl(out toolbarButtonNewQuest, "toolbarButtonNewQuest", rootVisualElementName, NewQuest);
        HookupControl(out toolbarButtonSaveQuest, "toolbarButtonSaveQuest", rootVisualElementName, SaveQuest);
        HookupControl(out toolbarButtonDeleteQuest, "toolbarButtonDeleteQuest", rootVisualElementName, DeleteQuest);
        HookupControl(out toolbarButtonNewTask, "toolbarButtonNewTask", rootVisualElementName, NewTask);
        HookupControl(out toolbarButtonDeleteTask, "toolbarButtonDeleteTask", rootVisualElementName, DeleteTask);
        HookupControl(out toolbarButtonChooseTask, "toolbarButtonChooseTask", rootVisualElementName, ChooseTask);
        HookupControl(out toolbarButtonAddTaskToQuests, "toolbarButtonAddTaskToQuests", rootVisualElementName, AddTaskToQuests);
    }

    void HookupControl(out Button control, string controlName, string rootVisualElementName, Action buttonClicked)
    {
        control = rootVisualElement.Q<Button>(controlName);
        if (control is null)
        {
            problemHookingUpControls = true;
            GameLog.ErrorMessage($"Unable to find Button '{controlName}' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        control.clicked += buttonClicked;
    }

    void HookupControl(out EnumField control, string controlName, string rootVisualElementName, StoryState initialValue)
    {
        control = rootVisualElement.Q<EnumField>(controlName);
        if (control is null)
        {
            problemHookingUpControls = true;
            GameLog.ErrorMessage($"Unable to find EnumField '{controlName}' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        control.Init(initialValue);
    }

    void HookupControl(out EnumField control, string controlName, string rootVisualElementName, QuestManager.OldGameEvent initialValue)
    {
        control = rootVisualElement.Q<EnumField>(controlName);
        if (control is null)
        {
            problemHookingUpControls = true;
            GameLog.ErrorMessage($"Unable to find EnumField '{controlName}' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        control.Init(initialValue);
    }

    void HookupControl(out ListView listviewControl, string controlName, string rootVisualElementName, ListViewFoldoutState openOrClosed, Action<IEnumerable<object>> itemsChosenHandler)
    {
        HookupControl(out listviewControl, controlName, rootVisualElementName, openOrClosed, null, itemsChosenHandler);
    }

    void HookupControl(out ListView control, string controlName, string rootVisualElementName, ListViewFoldoutState openOrClosed, List<string> listviewValues, Action<IEnumerable<object>> itemsChosenHandler = null)
    {
        control = rootVisualElement.Q<ListView>(controlName);

        if (control is null)
        {
            problemHookingUpControls = true;
            GameLog.ErrorMessage($"Unable to find ListView '{controlName}' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        listviewFoldout = control.Q<Foldout>(ListView.foldoutHeaderUssClassName);
        if (listviewFoldout is null)
        {
            problemHookingUpControls = true;
            GameLog.ErrorMessage($"Unable to find ListView Foldout '{ListView.foldoutHeaderUssClassName}' in '{controlName}'");
            return;
        }
        listviewFoldout.value = openOrClosed == ListViewFoldoutState.Expanded ? true : false;

        if (itemsChosenHandler is not null)
        {
            control.selectionChanged += itemsChosenHandler;
        }

        // The "makeItem" function is called when the ListView needs more items to render
        Func<VisualElement> makeItem = () => new Label();
        control.makeItem = makeItem;

        if (listviewValues is not null)
        {
            control.itemsSource = listviewValues;

            // As the user scrolls through the list, the ListView object recycles elements created by the "makeItem" function,
            // and it invokes the "bindItem" callback to associate the element with the matching data item (specified as an index in the list).
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as Label).text = listviewValues[i];
            };
            control.bindItem = bindItem;
        }

        // Add a dropdown to the listview
        control.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));

        // Register Focus In/Out event handlers so we can record which ListView control has focus
        control.RegisterCallback<FocusInEvent, ListView>(OnElementFocus, control);
        control.RegisterCallback<FocusOutEvent, ListView>(OnElementLostFocus, control);
    }

    void HookupControl(out TextField control, string controlName, string rootVisualElementName, string defaultValue = "")
    {
        control = rootVisualElement.Q<TextField>(controlName);
        if (control is null)
        {
            problemHookingUpControls = true;
            GameLog.ErrorMessage($"Unable to find TextField '{controlName}' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        control.value = defaultValue;

        control.isDelayed = false;
        control.RegisterValueChangedCallback(OnValueChanged);

        // Register Focus In/Out event handlers so we can record which TextField control has focus
        control.RegisterCallback<FocusInEvent, TextField>(OnElementFocus, control);
        control.RegisterCallback<FocusOutEvent, TextField>(OnElementLostFocus, control);
    }

    void HookupControl(out ToolbarButton control, string controlName, string rootVisualElementName, Action buttonClicked)
    {
        control = rootVisualElement.Q<ToolbarButton>(controlName);
        if (control is null)
        {
            problemHookingUpControls = true;
            GameLog.ErrorMessage($"Unable to find Button '{controlName}' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        control.clicked += buttonClicked;
    }
    #endregion

    /// <summary>
    /// Build right-click contextual menu
    /// </summary>
    /// <param name="evt"></param>
    void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Delete", OnMenuAction, DropdownMenuAction.AlwaysEnabled);
    }

    /// <summary>
    /// Invoked when the dropdown ListView's dropdown "delete" menu item is actioned
    /// </summary>
    /// <param name="action">The dropdown menu action</param>
    void OnMenuAction(DropdownMenuAction action)
    {
        // The selected item in the ListView that that had the dropdown "delete" invoked 
        string selectedItem = string.Empty;

        switch (focussedControl)
        {
            case "listviewSceneQuests":
                selectedItem = listviewSceneQuests.selectedItem as string;
                if (EditorUtility.DisplayDialog("Quest Deletion", $"Are you sure you want to delete '{selectedItem}'?", "Yes", "No"))
                {
                    questHelper.DeleteQuest(selectedItem);
                    ResetQuestAndTaskInfoPanel();
                    listviewSceneQuests.RefreshItems();
                }
                break;
            case "listviewQuestTasks":
                selectedItem = listviewQuestTasks.selectedItem as string;
                if (EditorUtility.DisplayDialog("Task Deletion", $"Are you sure you want to delete '{selectedItem}'?", "Yes", "No"))
                {
                    questHelper.DeleteTask(selectedItem);
                    ResetTaskPanel();
                    // Rebuild the Task and tasks listviews
                    listviewQuestTasks.RefreshItems();
                    listviewSceneQuests.RefreshItems();
                }
                break;
        }
    }

    private void OnValueChanged(ChangeEvent<string> evt)
    {
        string theQuestTitle = string.Empty;
        string theTaskTitle = string.Empty;
        Quest theQuest = null;
        Task theTask = null;

        Debug.Log($"{focussedControl} changed from '{evt.previousValue}' to '{evt.newValue}'");

        if (string.IsNullOrEmpty(focussedControl))
        {
            return;
        }

        switch (focussedControl)
        {
            case "txtSceneState":
                questHelper.SceneStory.GameState = evt.newValue;
                break;
            case "txtQuestState":
                theQuestTitle = questTitle.value;
                theQuest = questHelper.QuestDictionary[theQuestTitle];
                if (theQuest is not null)
                {
                    theQuest.GameState = evt.newValue;
                    questHelper.QuestDictionary[theQuestTitle] = theQuest;
                }
                break;
            case "txtTaskState":
                theTaskTitle = taskTitle.value;
                theTask = questHelper.TaskDictionary[theTaskTitle];
                if (theTask is not null)
                {
                    theTask.GameState = evt.newValue;
                    questHelper.TaskDictionary[theTaskTitle] = theTask;
                }
                break;
            case "txtSceneCompletionEvent":
                questHelper.SceneStory.CompletionEvent = evt.newValue;
                break;
            case "txtQuestCompletionEvent":
                theQuestTitle = questTitle.value;
                theQuest = questHelper.QuestDictionary[theQuestTitle];
                if (theQuest is not null)
                {
                    theQuest.CompletionEvent = evt.newValue;
                    questHelper.QuestDictionary[theQuestTitle] = theQuest;
                }
                break;
            case "txtTaskCompletionEvent":
                theTaskTitle = taskTitle.value;
                theTask = questHelper.TaskDictionary[theTaskTitle];
                if (theTask is not null)
                {
                    theTask.CompletionEvent = evt.newValue;
                    questHelper.TaskDictionary[theTaskTitle] = theTask;
                }
                break;
            case "txtSceneTitle":
                questHelper.SceneStory.Title = evt.newValue;
                break;
            case "txtQuestTitle":
                string oldQuestTitle = evt.previousValue;
                theQuest = questHelper.QuestDictionary[oldQuestTitle];
                if (theQuest is not null)
                {
                    theQuest.Title = evt.newValue;
                    questHelper.QuestDictionary.Remove(oldQuestTitle);
                    questHelper.QuestDictionary.Add(theQuest.Title, theQuest);
                }
                break;
            case "txtTaskTitle":
                string oldTaskTitle = evt.previousValue;
                theTask = questHelper.TaskDictionary[oldTaskTitle];
                if (theTask is not null)
                {
                    theTask.Title = evt.newValue;
                    questHelper.TaskDictionary.Remove(oldTaskTitle);
                    questHelper.TaskDictionary.Add(theTask.Title, theTask);
                }
                break;
            case "txtSceneShortDescription":
                questHelper.SceneStory.ShortDescription = evt.newValue;
                break;
            case "txtQuestShortDescription":
                theQuestTitle = questTitle.value;
                theQuest = questHelper.QuestDictionary[theQuestTitle];
                if (theQuest is not null)
                {
                    theQuest.ShortDescription = evt.newValue;
                    questHelper.QuestDictionary[theQuestTitle] = theQuest;
                }
                break;
            case "txtTaskShortDescription":
                theTaskTitle = taskTitle.value;
                theTask = questHelper.TaskDictionary[theTaskTitle];
                if (theTask is not null)
                {
                    theTask.ShortDescription = evt.newValue;
                    questHelper.TaskDictionary[theTaskTitle] = theTask;
                }
                break;
            case "txtSceneLongDescription":
                questHelper.SceneStory.LongDescription = evt.newValue;
                break;
            case "txtQuestLongDescription":
                theQuestTitle = questTitle.value;
                theQuest = questHelper.QuestDictionary[theQuestTitle];
                if (theQuest is not null)
                {
                    theQuest.LongDescription = evt.newValue;
                    questHelper.QuestDictionary[theQuestTitle] = theQuest;
                }
                break;
            case "txtTaskLongDescription":
                theTaskTitle = taskTitle.value;
                theTask = questHelper.TaskDictionary[theTaskTitle];
                if (theTask is not null)
                {
                    theTask.LongDescription = evt.newValue;
                    questHelper.TaskDictionary[theTaskTitle] = theTask;
                }
                break;
        }
    }

    void ResetQuestAndTaskInfoPanel()
    {
        ResetQuestPanel();
        ResetTaskPanel();
    }

    void ResetQuestPanel()
    {
        // Clear out the Quest Info pane
        questTitle.value = string.Empty;
        questShortDescription.value = string.Empty;
        questLongDescription.value = string.Empty;
        questCompletionEvent.value = string.Empty;
        questState.value = string.Empty;
    }

    void ResetTaskPanel()
    {
        // Clear out the Task Info pane
        taskTitle.value = string.Empty;
        taskShortDescription.value = string.Empty;
        taskLongDescription.value = string.Empty;
        taskCompletionEvent.value = string.Empty;
        taskState.value = string.Empty;
    }

    void OnElementFocus(FocusInEvent focusInEvent, ListView control)
    {
        focussedControl = control.name;
    }

    void OnElementLostFocus(FocusOutEvent focusInEvent, ListView control)
    {
        focussedControl = string.Empty;
    }

    void OnElementFocus(FocusInEvent focusInEvent, TextField control)
    {
        focussedControl = control.name;
    }

    void OnElementLostFocus(FocusOutEvent focusInEvent, TextField control)
    {
        focussedControl = string.Empty;
    }

    private void OnQuestItemsChosen(IEnumerable<object> chosenItems)
    {
        string questName = string.Empty;

        foreach(object o in chosenItems)
        {
            questName = o.ToString();
            break;
        }

        if (questHelper.QuestDictionary.ContainsKey(questName))
        {
            Quest chosenQuest = questHelper.QuestDictionary[questName];
            if (chosenQuest != null)
            {
                // Clear out the Task Info pane
                ResetTaskPanel();

                // Set the Quest Info pane
                questState.value = chosenQuest.GameState;
                questCompletionEvent.value = chosenQuest.CompletionEvent;
                questTitle.value = chosenQuest.Title;
                questShortDescription.value = chosenQuest.ShortDescription;
                questLongDescription.value = chosenQuest.LongDescription;
                listviewQuestTasks.itemsSource = chosenQuest.TaskTitles;
                if (chosenQuest.TaskTitles.Count > 0)
                {
                    string firstTaskTitle = chosenQuest.TaskTitles[0];

                    if(questHelper.TaskDictionary.ContainsKey(firstTaskTitle))
                    {
                        Task firstTask = questHelper.TaskDictionary[firstTaskTitle];
                        if (firstTask != null)
                        {
                            taskState.value = firstTask.GameState;
                            taskTitle.value = firstTask.Title;
                            taskShortDescription.value = firstTask.ShortDescription;
                            taskLongDescription.value = firstTask.LongDescription;
                            taskCompletionEvent.value = firstTask.CompletionEvent;
                        }
                    }
                }
                else
                {
                    ResetTaskPanel();
                }

                // As the user scrolls through the list, the ListView object recycles elements created by the "makeItem" function,
                // and it invokes the "bindItem" callback to associate the element with the matching data item (specified as an index in the list).
                Action<VisualElement, int> bindItem = (e, i) =>
                {
                    (e as Label).text = listviewQuestTasks.itemsSource[i] as string;
                };

                listviewQuestTasks.bindItem = bindItem;
            }
        }
    }

    private void OnTaskItemsChosen(IEnumerable<object> chosenItems)
    {
        // For the name of the Task
        string taskName = string.Empty;

        // If we have a set of chosen Task items
        if(chosenItems != null)
        {
            // Get the name of the first selected Task
            foreach (object o in chosenItems)
            {
                taskName = o.ToString();
                break;
            }

            // If the Task Dictionary has an entry for the Task
            if(questHelper.TaskDictionary.ContainsKey(taskName))
            {
                // Retrieve the Task from the Dictionary
                Task chosenTask = questHelper.TaskDictionary[taskName];
                
                // If we got a Task
                if (chosenTask != null)
                {
                    // Set the Task Pane's fields
                    SetTaskPanel(chosenTask);
                }
            }
        }
    }

    private void ChooseSceneState()
    {
        sceneState.value = ChooseListItem.Show("Select game state", "Please select the game state for the scene", questHelper.GameStates);
        questHelper.SceneStory.GameState = sceneState.value;
    }

    private void ChooseQuestState()
    {
        questState.value = ChooseListItem.Show("Select game state", "Please select the game state for the quest", questHelper.GameStates);

        string theQuestTitle = questTitle.value;
        Quest theQuest = questHelper.QuestDictionary[theQuestTitle];
        if (theQuest is not null)
        {
            theQuest.GameState = questState.value;
            questHelper.QuestDictionary[theQuestTitle] = theQuest;
        }
    }

    private void ChooseTaskState()
    {
        taskState.value = ChooseListItem.Show("Select game state", "Please select the game state for the task", questHelper.GameStates);

        string theTaskTitle = taskTitle.value;
        Task theTask = questHelper.TaskDictionary[theTaskTitle];
        if (theTask is not null)
        {
            theTask.GameState = taskState.value;
            questHelper.TaskDictionary[theTaskTitle] = theTask;
        }
    }

    private void ChooseSceneCompletionEvent()
    {
        sceneCompletionEvent.value = ChooseListItem.Show("Select completion event", "Please select the completion event for the scene", questHelper.CompletionEvents);
        questHelper.SceneStory.CompletionEvent = sceneCompletionEvent.value;
    }

    private void ChooseQuestCompletionEvent()
    {
        questCompletionEvent.value = ChooseListItem.Show("Select completion event", "Please select the completion event for the quest", questHelper.CompletionEvents);

        string theQuestTitle = questTitle.value;
        Quest theQuest = questHelper.QuestDictionary[theQuestTitle];
        if (theQuest is not null)
        {
            theQuest.CompletionEvent = questState.value;
            questHelper.QuestDictionary[theQuestTitle] = theQuest;
        }
    }

    private void ChooseTaskCompletionEvent()
    {
        taskCompletionEvent.value = ChooseListItem.Show("Select completion event", "Please select the completion event for the task", questHelper.CompletionEvents);

        string theTaskTitle = taskTitle.value;
        Task theTask = questHelper.TaskDictionary[theTaskTitle];
        if (theTask is not null)
        {
            theTask.CompletionEvent = taskCompletionEvent.value;
            questHelper.TaskDictionary[theTaskTitle] = theTask;
        }
    }

    private void AddTaskToQuests()
    {
        Task theTask = null;
        QuestEditor singleQuest = null;

        // Remember the Task Title
        string theTaskTitle = taskTitle.value;
        theTaskTitle.Trim();

        // Check it is not empty
        if(string.IsNullOrEmpty(theTaskTitle))
        {
            EditorUtility.DisplayDialog("Add Task to Quest(s)", "Please enter a title for the Task", "OK");
            return;
        }

        // Get, or create, the Task
        if (questHelper.TaskDictionary.ContainsKey(theTaskTitle))
        {
            theTask = questHelper.TaskDictionary[theTaskTitle];
        }
        else
        {
            theTask = new Task(theTaskTitle, taskShortDescription.value, taskLongDescription.value, taskCompletionEvent.value, taskState.value);
            questHelper.TaskDictionary.Add(theTaskTitle, theTask);
        }

        // Display the popup to choose Quests to which the Task should be added
        List<string> selectedQuests = ChooseQuests.Show("Basic Quest Info", questHelper.SceneStory.QuestTitles);

        // If one or more Quests were selected
        if (selectedQuests is not null)
        {
            // Loop through all selected Quests
            foreach (string item in selectedQuests)
            {
                // Add the Task to the Quest
                questHelper.AddTaskToQuest(item, theTask);
            }
        }

        // Refresh the List View of Tasks in the Quest panel
        listviewQuestTasks.RefreshItems();
    }

    void Reload()
    {

    }

    void Save()
    {
        if(EditorUtility.DisplayDialog("Save Story, Quest and Tasks", "Do you want to save the Story, Quest and Tasks information", "Yes", "No"))
        {
            questHelper.SceneStory.QuestTitles = (List<string>) listviewSceneQuests.itemsSource;
            questHelper.SaveSceneQuestsAndTasks();
        }
    }

    void NewQuest()
    {
        ResetQuestPanel();
    }

    void SaveQuest()
    {
        Quest newQuest = new Quest(questTitle.value, questShortDescription.value, questLongDescription.value, questCompletionEvent.value, questState.value);

        if(newQuest is not null)
        {
            OperationResult addQuestResult = questHelper.AddQuest(newQuest);

            switch(addQuestResult)
            {
                case OperationResult.TitleAlreadyExistsInQuestsList:
                    EditorUtility.DisplayDialog("Save Quest", $"A quest with title '{questTitle.value}' is already in the list of quests for the scene story", "Ok");
                    break;
                case OperationResult.TitleAlreadyExistsInDictionary:
                    EditorUtility.DisplayDialog("Save Quest", $"A quest with title '{questTitle.value}' is already in the dictionary of quests", "Ok");
                    break;
                case OperationResult.Success:
                    listviewSceneQuests.Rebuild();
                    break;
                default:
                    EditorUtility.DisplayDialog("Save Quest", $"Unexpected error '{addQuestResult.ToString()}' occurred when trying to save quest '{questTitle.value}'", "Ok");
                    break;
            }
        }
    }

    void DeleteQuest()
    {
        string theQuestTitle = questTitle.text;

        // Check if the user really wants to delete the quest
        if (EditorUtility.DisplayDialog("Quest Deletion", $"Are you sure you want to delete '{theQuestTitle}'?", "Yes", "No"))
        {
            // Delete the Quest
            if (questHelper.DeleteQuest(theQuestTitle))
            {
                GameLog.NormalMessage($"Quest '{theQuestTitle}' was deleted from the Scene Story list of Quests and the Quest Dictionary");
            }
            else
            {
                GameLog.ErrorMessage($"Unable properly delete Quest '{theQuestTitle}'");
            }

            // Clear out the quest's title, short description, and long description and its compleetion event
            questTitle.value = string.Empty;
            questShortDescription.value = string.Empty;
            questLongDescription.value = string.Empty;
            questCompletionEvent.value = string.Empty;
            questState.value = string.Empty;

            // Set the ListView of tasks in the quest to be an empty list of strings
            listviewQuestTasks.itemsSource = new List<string>();

            // Rebuild the quest and tasks listviews
            listviewQuestTasks.Rebuild();
            listviewSceneQuests.Rebuild();
        }

    }

    void NewTask()
    {
        ResetTaskPanel();
    }

    void DeleteTask()
    {
        string theTaskTitle = taskTitle.text;

        // Check if the user really wants to delete the Task
        if (EditorUtility.DisplayDialog("Task Deletion", $"Are you sure you want to delete '{theTaskTitle}'?", "Yes", "No"))
        {
            if (questHelper.DeleteTask(theTaskTitle))
            {
                ResetTaskPanel();

                // Rebuild the listviews
                listviewQuestTasks.Rebuild();
                listviewSceneQuests.Rebuild();
            }
        }

    }

    void ChooseTask()
    {
        //string titleDescriptionSeparator = " - ";
        List<string> allTaskTitles = new List<string>();
        List<string> allTaskShortDescriptions = new List<string>();

        Task singleTask = null;
        foreach (string singleTaskTitle in questHelper.TaskDictionary.Keys)
        {
            singleTask = questHelper.TaskDictionary[singleTaskTitle];
            allTaskTitles.Add(singleTaskTitle);
            allTaskShortDescriptions.Add(singleTask.ShortDescription);
        }

        string chosenTaskTitle = ChooseListItem.Show("Select task to edit", "Please select which task you wish to edit", allTaskTitles, allTaskShortDescriptions);

        if (false == string.IsNullOrEmpty(chosenTaskTitle))
        {
            Task chosenTask = questHelper.TaskDictionary[chosenTaskTitle];
            SetTaskPanel(chosenTask);
        }
    }

    void btnSaveClicked()
    {
        questHelper.SaveSceneQuestsAndTasks();
    }

    void SetTaskPanel(Task singleTask)
    {
        if(singleTask is not null)
        {
            taskTitle.value = singleTask.Title;
            taskShortDescription.value = singleTask.ShortDescription;
            taskLongDescription.value = singleTask.LongDescription;
            taskCompletionEvent.value = singleTask.CompletionEvent;
            taskState.value = singleTask.GameState;
        }
        else
        {
            SetTaskPanel();
        }
    }

    void SetTaskPanel()
    {
        taskTitle.value = string.Empty;
        taskShortDescription.value = string.Empty;
        taskLongDescription.value = string.Empty;
        taskCompletionEvent.value = string.Empty;
        taskState.value = string.Empty;
    }
}
