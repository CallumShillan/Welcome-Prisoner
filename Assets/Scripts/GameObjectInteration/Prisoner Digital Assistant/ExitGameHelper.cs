using System;
using UnityEngine;
using UnityEngine.UIElements;

public static class ExitGameHelper
{
    private static Button btnYes = null;
    private static Button btnNo = null;
    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler)
    {
        string rootVisualElementName = rootVisualElement.name;

        btnYes = rootVisualElement.Q<Button>("btn-Yes");
        if (btnYes is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'btn-Yes' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        btnYes.RegisterCallback<ClickEvent>(ev => { ExitGame(); });

        btnNo = rootVisualElement.Q<Button>("btn-No");
        if (btnNo is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'btn-No' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        btnNo.RegisterCallback<ClickEvent>(ev => { pdaEventHandler.DisplayPdaApp(PdaAppId.PDAHomePage); });

    }

    private static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
