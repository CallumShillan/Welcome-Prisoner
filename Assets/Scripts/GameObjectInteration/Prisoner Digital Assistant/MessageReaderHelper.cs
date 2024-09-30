//using Mono.Cecil.Cil;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Xml.Linq;
public static class MessageReaderHelper
{
    private static ScrollView messageNameScrollView = null;
    private static Button referenceMessageButton = null;
    private static Label labelMessageName = null;
    private static Label labelMessageText = null;
    private static Label labelMessageDateTime = null;

    private static PrisonerDigitalAssistantEventHandler thePdaEventHandler;

    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler)
    {
        string appIdentifier = string.Empty;

        thePdaEventHandler = pdaEventHandler;

        string rootVisualElementName = rootVisualElement.name;

        // Get the ScrollView used to house all the buttons for each message
        messageNameScrollView = rootVisualElement.Q<ScrollView>("messageNameScrollView");
        if (messageNameScrollView is null)
        {
            GameLog.ErrorMessage($"Unable to find Scroller 'messageNameScrollView' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        // Get the reference message button name
        referenceMessageButton = rootVisualElement.Q<Button>("btn-MessageName");
        if (referenceMessageButton is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'btn-MessageName' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        // Clear the scroller of all book name references
        messageNameScrollView.Clear();

        labelMessageName = rootVisualElement.Q<Label>("label-MessageName");
        if (labelMessageName is null)
        {
            GameLog.ErrorMessage($"Unable to find label 'label-MessageName' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelMessageName.text = "Choose a message to view from the left-hand navigation ...";

        labelMessageText = rootVisualElement.Q<Label>("label-MessageText");
        if (labelMessageText is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'label-MessageText' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelMessageText.text = string.Empty;

        labelMessageDateTime = rootVisualElement.Q<Label>("label-MessageDateTime");
        if (labelMessageDateTime is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'label-MessageDateTime' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelMessageDateTime.text = string.Empty;
    }

    public static void CreateNavigationForKnownMessages()
    {
        messageNameScrollView.Clear();

        foreach (string messageName in GameMessages.Instance.AllGameMessages.Keys)
        {
            // Get the message being read
            GameMessage theMessage = GameMessages.Instance.AllGameMessages[messageName];

            if(theMessage.HasBeenShown)
            {
                // Create a Button for each document in the room and add it to the Foldout
                Button viewMessageButton = null;

                viewMessageButton = new Button();
                viewMessageButton.text = messageName;
                viewMessageButton.name = messageName;

                // Copy the CSS classes from the reference Document Button
                foreach (string cssClass in referenceMessageButton.GetClasses())
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
        labelMessageName.text = messageName;
        labelMessageDateTime.text = string.Format("{0} {1}", theMessage.WhenShown.ToLongDateString(), theMessage.WhenShown.ToLongTimeString());
        labelMessageText.text = theMessage.MessageText;

        // Set the book's display button to indicate it is selected
        Button bookButton = messageNameScrollView.Q<Button>(messageName);
        bookButton.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;

        // Display the eBook Reader app
        thePdaEventHandler.DisplayPdaApp(PdaAppId.MessageReader);
    }
}
