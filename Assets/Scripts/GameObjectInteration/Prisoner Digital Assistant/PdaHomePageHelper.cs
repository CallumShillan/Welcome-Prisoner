using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PdaHomePageHelper : MonoBehaviour
{
    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler)
    {
        string[] allButtons = new string[] { "btn-EbookReader", "btn-QuestDetails", "btn-ExitGame", "btn-MessageReader" };
        string appIdentifier = string.Empty;

        foreach (string buttonName in allButtons)
        {
            Button aButton = rootVisualElement.Q<Button>(buttonName);
            if (aButton is null)
            {
                GameLog.ErrorMessage($"Unable to find button '{buttonName}' in Root Visual Element '{rootVisualElement.name}'");
                continue;
            }

            string pdaApplicationIdentifier = buttonName.Substring(4);

            PdaAppId pdaAppIdentifier;
            if (Enum.TryParse(pdaApplicationIdentifier, out pdaAppIdentifier))
            {
                aButton.RegisterCallback<ClickEvent>(ev => pdaEventHandler.DisplayPdaApp(pdaAppIdentifier));
            }
            else
            {
                GameLog.ErrorMessage($"Unable to convert '{pdaApplicationIdentifier}' to an enumeration of PdaAppId, so unable to determine which PDA App to display");
                continue;
            }
        }
    }
}
