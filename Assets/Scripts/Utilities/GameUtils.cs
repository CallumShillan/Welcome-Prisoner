
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
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

    public static void DisplayInteractionMessage(InteractionMessage theMessage)
    {
        if(theMessage is not null)
        {
            if (theMessage.ShowGameMessageAfterInteraction)
            {
                // Show the game message
                DisplayGameMessage.Instance.ShowGameMessage(theMessage.SpeakerIconTexture, theMessage.GameMessageTitle);
            }
        }
    }

    //public static IEnumerator FadeIn(Material mat, float duration)
    //{
    //    // Color is a struct, so modify a copy and then reassign
    //    Color color = mat.color;
    //    color.a = 0f;
    //    mat.color = color;

    //    float elapsed = 0f;
    //    while (elapsed < duration)
    //    {
    //        elapsed += Time.deltaTime;
    //        color.a = Mathf.Clamp01(elapsed / duration);
    //        mat.color = color;
    //        yield return null;
    //    }
    //}

    public static void SetLayerRecursively(GameObject root, string newLayer)
    {
        int layer = LayerMask.NameToLayer(newLayer);
        if (layer == -1)
        {
            GameLog.WarningMessage($"Layer \"{newLayer}\" does not exist.");
            return;
        }

        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = layer;
        }
    }
}
