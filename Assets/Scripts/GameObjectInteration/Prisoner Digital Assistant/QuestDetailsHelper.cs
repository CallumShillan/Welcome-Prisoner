using System.Text;
using UnityEngine.UIElements;

public static class QuestDetailsHelper
{
    private static GroupBox leftHandNavigationGroupBox = null;
    private static Foldout referenceActiveQuestsFoldout = null;
    private static GroupBox questList = null;
    private static Button referenceQuestNameButton = null;
    private static Label labelQuestName = null;
    private static Label labelQuestDetails = null;
    private static PrisonerDigitalAssistantEventHandler thePdaEventHandler;

    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler)
    {
        string appIdentifier = string.Empty;
        string rootVisualElementName = rootVisualElement.name;

        thePdaEventHandler = pdaEventHandler;

        leftHandNavigationGroupBox = rootVisualElement.Q<GroupBox>("lefthand-nav");
        if (leftHandNavigationGroupBox is null)
        {
            GameLog.ErrorMessage($"Unable to find foldout 'lefthand-nav' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        referenceActiveQuestsFoldout = rootVisualElement.Q<Foldout>("active-quests-foldout");
        if (referenceActiveQuestsFoldout is null)
        {
            GameLog.ErrorMessage($"Unable to find foldout 'active-quests-foldout' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        questList = rootVisualElement.Q<GroupBox>("quest-list");
        if (questList is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'quest-list' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        // Remember the single button in the Quest List that acts as a refernce for all others quests to be added
        // Clicking a quest button will update the Quest Details text shown to the player
        referenceQuestNameButton = rootVisualElement.Q<Button>("btn-QuestName");
        if (referenceQuestNameButton is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'btn-QuestName' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        questList.Remove(referenceQuestNameButton);

        labelQuestName = rootVisualElement.Q<Label>("label-QuestName");
        if (labelQuestName is null)
        {
            GameLog.ErrorMessage($"Unable to find label 'label-QuestName' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelQuestName.text = "No active task ...";

        labelQuestDetails = rootVisualElement.Q<Label>("label-QuestDetails");
        if (labelQuestDetails is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'label-QuestDetails' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelQuestDetails.text = string.Empty;
    }

    public static void CreateNavigationForKnownQuests()
    {
        // Remove all View Quest Buttons from the Active Quests Foldout
        referenceActiveQuestsFoldout.contentContainer.Clear();

        // Create a Button for each active quest and add it to the Foldout
        Button viewQuestButton = null;

        string firstActiveQuestTitle = string.Empty;

        foreach (string questTitle in QuestManager.ActiveOrCompletedQuests)
        {
            if(string.IsNullOrEmpty(firstActiveQuestTitle))
            {
                firstActiveQuestTitle = questTitle;
            }

            viewQuestButton = new Button();
            viewQuestButton.text = questTitle;
            viewQuestButton.name = questTitle;
            viewQuestButton.RegisterCallback<ClickEvent>(ev => { DisplayQuest(questTitle); });

            // Apply the CSS from the refernce Quest Name button
            foreach(string cssClassName in referenceQuestNameButton.GetClasses())
            {
                viewQuestButton.AddToClassList(cssClassName);
            }

            // Add the Document Button to the Foldout
            referenceActiveQuestsFoldout.contentContainer.Add(viewQuestButton);
        }

        // Display the first active quest
        DisplayQuest(firstActiveQuestTitle);
    }

    public static void DisplayQuest(string questTitle)
    {
        StringBuilder allQuestDetails = new StringBuilder();

        Quest gameQuest = QuestManager.GetQuest(questTitle);

        if (gameQuest is not null)
        {
            allQuestDetails.AppendLine(gameQuest.ShortDescription);
            allQuestDetails.AppendLine();
            allQuestDetails.AppendLine("You have the following tasks:");

            foreach (string singleTaskName in gameQuest.TaskTitles)
            {
                Task singleTask = QuestManager.GetTask(singleTaskName);
                allQuestDetails.AppendLine($"<b>{singleTask.Title}</b> ({singleTask.State.ToString()})");
                allQuestDetails.AppendLine(singleTask.ShortDescription);
                allQuestDetails.AppendLine();
            }
            labelQuestDetails.text = allQuestDetails.ToString();
            labelQuestName.text = questTitle;
        }
    }
}
