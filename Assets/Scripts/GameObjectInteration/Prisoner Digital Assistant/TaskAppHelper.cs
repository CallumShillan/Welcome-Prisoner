using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public static class TaskAppHelper
{
    private static VisualElement theRootVisualElement = null;
    private static ListView questListView = null;
    private static Label questNameTitle = null;
    private static MultiColumnTreeView taskTreeview = null;

    private static bool yetToWireUp = true;

    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler)
    {
        string rootVisualElementName = rootVisualElement.name;

        if (rootVisualElement is null)
        {
            GameLog.ErrorMessage($"Root Visual Element '{rootVisualElementName}' is null, cannot wire up Task App");
            return;
        }

        theRootVisualElement = rootVisualElement;

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

        if(yetToWireUp)
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
            //taskTreeview.fixedItemHeight = 64;
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

                    //container.AddToClassList("quest-listview-item-container");;
                    //taskLabel.AddToClassList(Globals.Instance.UiStyles.TaskLabelClass);

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
                                break;
                            case "Title":
                                label.text = GameUtils.SplitPascalCase(task.Title);
                                break;
                            case "Short Description":
                                label.text = task.ShortDescription;
                                break;
                            case "Status":
                                label.text = task.State.ToString();
                                break;
                            default:
                                label.text = "huh";
                                break;
                        }
                    }
                };
            }

            taskTreeview.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    // Find the row container that was clicked
                    var row = evt.target as VisualElement;
                    while (row != null && !row.ClassListContains("unity-tree-view__row"))
                    {
                        row = row.parent;
                    }

                    if (row == null)
                        return;

                    // Get the index from the row's userData
                    if (row.userData is TreeViewItemData<object> itemData)
                    {
                        var selectedTask = itemData.data as Task;

                        // Extract the Title column value
                        string taskTitle = selectedTask.Title;
                        if (!string.IsNullOrEmpty(taskTitle))
                        {
                            ShowMessage(taskTitle + "\nShort Description " + selectedTask.ShortDescription + "\nLong Description " + selectedTask.LongDescription + "\nCompletion Event: " + selectedTask.CompletionEvent.ToString());
                        }
                    }
                }
            });

            yetToWireUp = false;
        }

        questListView.itemsSource = questNames;
        questListView.Rebuild();
        questListView.selectionType = SelectionType.None;

    }

    private static void ShowMessage(string messageText)
    {
        var overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.top = 0;
        overlay.style.left = 0;
        overlay.style.right = 0;
        overlay.style.bottom = 0;
        overlay.style.backgroundColor = new Color(0, 0, 0, 0.5f); // semi-transparent

        var dialog = new VisualElement();
        dialog.style.width = 300;
        dialog.style.marginTop = 100;
        dialog.style.marginLeft = Length.Percent(50);
        dialog.style.translate = new Translate(-150, 0, 0); // center horizontally
        dialog.style.backgroundColor = Color.gray;
        dialog.style.paddingTop = 10;
        dialog.style.paddingBottom = 10;
        dialog.style.paddingLeft = 15;
        dialog.style.paddingRight = 15;
        dialog.style.borderTopLeftRadius = 6;
        dialog.style.borderBottomRightRadius = 6;

        var label = new Label(messageText);
        label.style.unityTextAlign = TextAnchor.MiddleCenter;
        label.style.marginBottom = 10;

        var okButton = new Button(() =>
        {
            overlay.RemoveFromHierarchy(); // dismiss
        })
        {
            text = "OK"
        };
        okButton.style.alignSelf = Align.Center;

        dialog.Add(label);
        dialog.Add(okButton);
        overlay.Add(dialog);

        theRootVisualElement.Add(overlay);
    }


    private static string GetColumnValue(object item, string columnName)
    {
        if (item is Task task)
            return task.Title;

        // If using a dictionary or dynamic structure:
        if (item is IDictionary<string, object> dict && dict.TryGetValue(columnName, out var value))
            return value?.ToString();

        return null;
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
