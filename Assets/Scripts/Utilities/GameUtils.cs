
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine.UIElements;

public static class GameUtils
{
    public static string ActionNameHint(string action, string name, string hint)
    {
        hint = hint
            .Replace("{ACTION}", action)
            .Replace("{NAME}", name);

        hint = SplitPascalCase(hint);

        return hint;
    }

    public static string SplitPascalCase(string input)
    {
        const string placeholder = "_p_d_a_";

        // Step 1: Replace all "PDA" with placeholder
        input = input.Replace("PDA", placeholder);

        // Step 2: Apply spacing regex
        input = Regex.Replace(input, "(\\B[A-Z])", " $1");

        // Step 3: Restore "PDA" or " PDA"
        if(input.StartsWith(placeholder))
        {
            input = input.Replace(placeholder, "PDA");
        }
        else
        {
            input = input.Replace(placeholder, " PDA");
        }

        return input;
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

    public static void SetPickingModeRecursive(this VisualElement element, PickingMode mode)
    {
        element.pickingMode = mode;
        foreach (var child in element.Children())
        {
            child.SetPickingModeRecursive(mode);
        }
    }

}
