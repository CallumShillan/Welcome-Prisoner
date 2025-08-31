
using System.Text.RegularExpressions;

public static class GameUtils
{
    public static string ActionNameHint(string action, string name, string hint)
    {
        return hint
            .Replace("{ACTION}", action)
            .Replace("{NAME}",
                Regex.Replace(name,
                    "(\\B[A-Z])",
                    " $1")
            );
    }

    public static string SplitPascalCase(string input)
    {
        return Regex.Replace(input, "(\\B[A-Z])", " $1");
    }

    public static bool InitiateQuestAndTask(string questToInitiate, string taskToInitiate)
    {
        // Determine if we have a quest and task to initiate
        bool hasQuest = !string.IsNullOrWhiteSpace(questToInitiate);
        bool hasTask = !string.IsNullOrWhiteSpace(taskToInitiate);

        if (hasQuest && hasTask)
        {
            QuestManager.CurrentQuestName = questToInitiate;
            QuestManager.CurrentTaskName = taskToInitiate;
            return true;
        }

        return false;
    }
}
