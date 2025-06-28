//using Mono.Cecil.Cil;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Collections;
public static class MessageReaderHelper
{
    private static ScrollView messageNameScrollView = null;
    private static Button referenceMessageNameButton = null;
    private static Label labelItemCollectionName = null;
    private static Label labelItemNameTitle = null;
    private static Label labelMessageText = null;
    //private static Label labelMessageName = null;
    //private static Label labelMessageDateTime = null;

    private static PrisonerDigitalAssistantEventHandler thePdaEventHandler;

    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler, string itemCollectionName, string defaultItemNameTitle)
    {
        string appIdentifier = string.Empty;

        thePdaEventHandler = pdaEventHandler;

        string rootVisualElementName = rootVisualElement.name;

        // Get the label for the ItemCollectionName
        labelItemCollectionName = rootVisualElement.Q<Label>("label_ItemCollectionName");
        if (labelItemCollectionName is null)
        {
            GameLog.ErrorMessage($"Unable to find Label 'label_ItemCollectionName' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelItemCollectionName.text = itemCollectionName;

        // Get the label for the ItemNameTitle
        labelItemNameTitle = rootVisualElement.Q<Label>("label_ItemNameTitle");
        if (labelItemNameTitle is null)
        {
            GameLog.ErrorMessage($"Unable to find Label 'label_ItemNameTitle' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelItemNameTitle.text = defaultItemNameTitle;

        // Get the ScrollView used to house all the buttons for each message
        messageNameScrollView = rootVisualElement.Q<ScrollView>("scrollview_ItemNames");
        if (messageNameScrollView is null)
        {
            GameLog.ErrorMessage($"Unable to find Scroller 'scrollview_ItemNames' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        // Get the reference message button name
        referenceMessageNameButton = rootVisualElement.Q<Button>("button_ItemName");
        if (referenceMessageNameButton is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'button_ItemName' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        labelMessageText = rootVisualElement.Q<Label>("label_ItemContents");
        if (labelMessageText is null)
        {
            GameLog.ErrorMessage($"Unable to find label 'label_ItemContents' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelMessageText.text = string.Empty;

    }

    public static void CreateNavigationForKnownMessages()
    {
        messageNameScrollView.Clear();

        foreach (string messageName in GameMessages.Instance.AllGameMessagesByWhenShown.Values)
        {
            // Get the message being read
            GameMessage theMessage = GameMessages.Instance.AllGameMessages[messageName];

            if (theMessage.HasBeenShown)
            {
                // Create a Button for each document in the room and add it to the Foldout
                Button viewMessageButton = null;

                viewMessageButton = new Button();
                viewMessageButton.text = messageName;
                viewMessageButton.name = messageName;

                // Copy the CSS classes from the reference Document Button
                foreach (string cssClass in referenceMessageNameButton.GetClasses())
                {
                    viewMessageButton.AddToClassList(cssClass);
                }

                // Register the cluckback to view the message
                viewMessageButton.RegisterCallback<ClickEvent>(ev => { DisplayMessage(messageName); });

                // Add the View Message button to the scroll view
                messageNameScrollView.Add(viewMessageButton);
            }
        }
    }

    public static void DisplayMessage(string messageName)
    {
        // Get the message being read
        GameMessage theMessage = GameMessages.Instance.AllGameMessages[messageName];

        // Set the message contents
        labelItemNameTitle.text  = messageName;
        labelMessageText.text = $"Shown on {theMessage.WhenShown.ToLongDateString()} {theMessage.WhenShown.ToLongTimeString()}<br><br>{theMessage.MessageText}";
    }
}
