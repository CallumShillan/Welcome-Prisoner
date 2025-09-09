using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class TaskAppHelper
{
    private static VisualElement theTaskListVisualElement = null;

    private static ListView questListView = null;
    private static Label questNameTitle = null;
    private static MultiColumnTreeView taskTreeview = null;

    private static bool yetToWireUp = true;

    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler)
    {
        if (rootVisualElement is null)
        {
            GameLog.ErrorMessage($"Root Visual Element is null, cannot wire up Task App");
            return;
        }

        string rootVisualElementName = rootVisualElement.name;

        theTaskListVisualElement = rootVisualElement;

        questListView = rootVisualElement.Q<ListView>("activeQuestsListView");
        if (questListView is null)
        {
            GameLog.ErrorMessage($"Unable to find listView 'activeQuestsListView' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        questListView.Clear();
        questListView.fixedItemHeight = Globals.Instance.UiStyles.QuestListviewFixedItemHeight;

        //List<string> questNames = QuestManager.ActiveOrCompletedQuests;
        List<string> questNames = QuestManager.SceneStory.QuestTitles;

        if (yetToWireUp)
        {
            questListView.makeItem = () =>
            {
                var container = new VisualElement();
                container.AddToClassList("quest-listview-item-container");

                var button = new Button();
                button.name = "questListViewButton";
                button.AddToClassList(Globals.Instance.UiStyles.QuestButtonClass);

                container.Add(button);
                return container;
            };

            questListView.bindItem = (element, index) =>
            {
                var button = element.Q<Button>("questListViewButton");
                button.text = questNames[index];

                button.clicked += () => DisplayQuest(questNames[index]);
            };

            questNameTitle = rootVisualElement.Q<Label>("questNameTitle");
            if (questNameTitle is null)
            {
                GameLog.ErrorMessage($"Unable to find label 'questNameTitle' in Root Visual Element '{rootVisualElementName}'");
                return;
            }
            questNameTitle.text = "No active task ...";

            taskTreeview = rootVisualElement.Q<MultiColumnTreeView>("tasksTreeView");
            if (taskTreeview is null)
            {
                GameLog.ErrorMessage($"Unable to find button 'taskTreeView' in Root Visual Element '{rootVisualElementName}'");
                return;
            }

            taskTreeview.AddToClassList("tasks-treeview");
            taskTreeview.columns.Add(new Column { title = "Active", width = 100, resizable = true });
            taskTreeview.columns.Add(new Column { title = "Title", width = 300, resizable = true });
            taskTreeview.columns.Add(new Column { title = "Short Description", width = 500, resizable = true });
            taskTreeview.columns.Add(new Column { title = "Status", width = 150, resizable = true });
            taskTreeview.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            taskTreeview.style.flexGrow = 1;
            taskTreeview.style.height = 1;

            foreach (var column in taskTreeview.columns)
            {
                column.makeCell = () =>
                {
                    VisualElement container = new VisualElement();

                    Label taskLabel = new Label();
                    taskLabel.AddToClassList(Globals.Instance.UiStyles.TaskLabelClass);
                    taskLabel.name = "taskLabel";

                    container.Add(taskLabel);

                    return container;
                };

                column.bindCell = (element, item) =>
                {
                    var label = element.Q<Label>("taskLabel");
                    var task = taskTreeview.GetItemDataForIndex<Task>(item);
                    if (label is not null && task is not null)
                    {
                        switch (column.title)
                        {
                            case "Active":
                                label.text = task.IsActive ? "Yes" : "No";
                                label.userData = task.Title;
                                break;
                            case "Title":
                                label.text = GameUtils.SplitPascalCase(task.Title);
                                label.userData = task.Title;
                                break;
                            case "Short Description":
                                label.text = task.ShortDescription;
                                label.userData = task.Title;
                                break;
                            case "Status":
                                label.text = task.State.ToString();
                                label.userData = task.Title;
                                break;
                            default:
                                label.text = "huh";
                                label.userData = "huh";
                                break;
                        }
                    }
                };
            }

            taskTreeview.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.clickCount == 1)
                {
                    var row = evt.target as VisualElement;
                    string clickedTaskTitle = row.userData as string;

                    Task clickedTask = QuestHelper.TaskDictionary[clickedTaskTitle];
                    if (clickedTask is not null)
                    {
                        ShowMessage(clickedTask.Title, "\nShort Description " + clickedTask.ShortDescription + "\nLong Description " + clickedTask.LongDescription + "\nCompletion Event: " + clickedTask.CompletionEvent.ToString());
                    }
                }
            });

            yetToWireUp = false;
        }

        questListView.itemsSource = questNames;
        questListView.Rebuild();
        questListView.selectionType = SelectionType.None;
    }

    private static void ShowMessage(string messageTitle, string messageText)
    {
        UIDocument detailedTaskViewUiDocument = Globals.Instance.GameMessageUiDocument;

        detailedTaskViewUiDocument.panelSettings.sortingOrder = 999;

        VisualElement detailedTaskViewRootVisualElement = detailedTaskViewUiDocument.rootVisualElement;
        detailedTaskViewRootVisualElement.style.display = DisplayStyle.Flex;

        Debug.Log($"ShowMessage called with title: {messageTitle}, text: {messageText}");
        Debug.Log($"Root display: {detailedTaskViewRootVisualElement.style.display}, childCount: {detailedTaskViewRootVisualElement.childCount}");
        Debug.Log($"Resolved display: {detailedTaskViewRootVisualElement.resolvedStyle.display}");

        // Hide the speaker icon
        var speakerIcon = detailedTaskViewRootVisualElement.Q<VisualElement>("SpeakerIconTexture");
        if (speakerIcon != null)
        {
            speakerIcon.style.display = DisplayStyle.None;
        }

        // Set label texts
        var titleLabel = detailedTaskViewRootVisualElement.Q<Label>("SpeakerAndMessageTitle");
        if (titleLabel != null)
        {
            titleLabel.text = messageTitle;
        }

        var messageLabel = detailedTaskViewRootVisualElement.Q<Label>("Message");
        if (messageLabel != null)
        {
            messageLabel.text = messageText;
        }

        // Hook up dismiss button
        var dismissButton = detailedTaskViewRootVisualElement.Q<Button>("DismissButton");
        if (dismissButton != null)
        {
            dismissButton.clicked -= DismissHandler;
            dismissButton.clicked += DismissHandler;
        }
        theTaskListVisualElement.parent.parent.style.display = DisplayStyle.None;
        Globals.Instance.GameMessageUiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        Globals.Instance.GameMessageUiDocument.rootVisualElement.visible = true;
    }
    private static void DismissHandler()
    {
        Globals.Instance.GameMessageUiDocument.rootVisualElement.style.display = DisplayStyle.None;
        theTaskListVisualElement.parent.parent.style.display = DisplayStyle.Flex;
        Globals.Instance.Player.SetActive(false);
        Globals.Instance.GameMessageUiDocument.rootVisualElement.visible = false;
        Debug.Log("DismissHandler called");
    }

    public static void DisplayQuest(string questTitle)
    {
        Quest gameQuest = QuestManager.GetQuest(questTitle);

        if (gameQuest is not null)
        {
            questNameTitle.text = questTitle;

            var items = new List<TreeViewItemData<Task>>();
            foreach ( string taskName in gameQuest.TaskTitles)
            {
                Task singleTask = QuestManager.GetTask(taskName);
                items.Add(new TreeViewItemData<Task>(singleTask.Title.GetHashCode(), singleTask));
            }

            // Clear out old data, set new data, and refresh the treeview
            taskTreeview.Clear();
            taskTreeview.SetRootItems(items);
            taskTreeview.RefreshItems();
        }
        else
        {
            questNameTitle.text = "No active task ...";
            taskTreeview.Clear();
            taskTreeview.RefreshItems();
        }
    }
}
