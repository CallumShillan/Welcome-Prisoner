using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public static class TaskAppHelper
{
    private static ListView questListView = null;
    private static Label questNameTitle = null;
    private static MultiColumnTreeView taskTreeview = null;

    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler)
    {
        string rootVisualElementName = rootVisualElement.name;

        questListView = rootVisualElement.Q<ListView>("activeQuestsListView");
        if (questListView is null)
        {
            GameLog.ErrorMessage($"Unable to find listView 'activeQuestsListView' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        questListView.fixedItemHeight = Globals.Instance.UiStyles.QuestListviewFixedItemHeight;

        //List<string> questNames = QuestManager.ActiveOrCompletedQuests;
        List<string> questNames = QuestManager.SceneStory.QuestTitles;

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

        questListView.itemsSource = questNames;
        questListView.selectionType = SelectionType.None;

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
