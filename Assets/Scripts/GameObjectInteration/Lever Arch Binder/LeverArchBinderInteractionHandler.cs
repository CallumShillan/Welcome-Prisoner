using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LeverArchBinderInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField]
    [Tooltip("The name of the book that will be displayed")]
    private string bookName = string.Empty;

    [SerializeField]
    [Tooltip("The icon displayed for this action")]
    private UnityEngine.UI.Image actionIcon = null;

    [SerializeField]
    [Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh;

    [SerializeField]
    [Tooltip("A tooltip about the action")]
    private string actionHintMessage = string.Empty;

    [SerializeField]
    [Tooltip("The Game Object UI Document for the Security Keypad user interface")]
    private GameObject leverArchBinderUserInterfaceGameObject = null;

    // Used to hide and display the Security Keypad
    private VisualElement leverArchBinderRootVisualElement = null;

    // The Game Object that has the UI Document for the Security Keypad
    private UIDocument leverArchBinderUserInterfaceDocument = null;

    // The page that displays the text in the Lever Arch Binder
    private UnityEngine.UIElements.Label leverArchBinderPage = null;

    // Displays the name of the Lever Arch Binder
    private UnityEngine.UIElements.Label leverArchBinderName = null;

    // Whether the interaction of the Security Keypad is completed
    private InteractionStatus interactionIsCompleted = InteractionStatus.Continuing;

    // The original Action Hint message as the one displayed to the use may have keywords replaced by values
    private string originalActionHintMessage = null;

    private int currentPageNumber = 0;
    private int numberOfPages = 0;

    private List<string> bookPages;

    //private GameBook thisGameDocument = null;

    // Start is called before the first frame update
    public void Start()
    {
        // Save the original Action Hint message
        originalActionHintMessage = actionHintMessage;

        bookPages = GameDocuments.Instance.AllBooks[bookName].ThePages;
        numberOfPages = bookPages.Count;

        // The Game Object that has the UI Document for the Lever Arch Binder
        if (false == leverArchBinderUserInterfaceGameObject.TryGetComponent<UIDocument>(out leverArchBinderUserInterfaceDocument))
        {
            GameLog.ErrorMessage(leverArchBinderUserInterfaceGameObject, "Unable to get a UI Document for the Lever Arch Binder. Did you forget to add one in the Game Object Inspector?");
            return;
        }

        // Remember the Lever Arch Binder's Root Visual Element
        leverArchBinderRootVisualElement = leverArchBinderUserInterfaceDocument.rootVisualElement;
        if (leverArchBinderRootVisualElement is null)
        {
            GameLog.ErrorMessage(leverArchBinderUserInterfaceDocument, "No Root Visual Element found in the Lever Arch Binder user interface document.");
            return;
        }

        // Get the Root Frame of the Lever Arch Binder UI
        VisualElement leverArchBinderAppRootFrame = leverArchBinderRootVisualElement.Q<VisualElement>("RootFrame");
        if (leverArchBinderAppRootFrame is null)
        {
            GameLog.ErrorMessage(leverArchBinderUserInterfaceGameObject, "Unable to find a Visual Element called 'RootFrame' in the UI Document.");
            return;
        }

        // Get the label for the Binder Name in the Lever Arch Binder
        leverArchBinderName = leverArchBinderAppRootFrame.Q<UnityEngine.UIElements.Label>("LeverArchBinderName");
        leverArchBinderName.text = this.name;

        // Get the Page in the Lever Arch Binder
        leverArchBinderPage = leverArchBinderAppRootFrame.Q<UnityEngine.UIElements.Label>("LeverArchBinderPage");
        leverArchBinderPage.text = String.Empty;

        // Hook up event handlers to the buttons
        foreach (string keyValue in new string[] { "First", "Previous", "Next", "Last" })
        {
            UnityEngine.UIElements.Button btnVerify = leverArchBinderAppRootFrame.Q<UnityEngine.UIElements.Button>("Key-" + keyValue);
            if (null != btnVerify)
            {
                btnVerify.RegisterCallback<ClickEvent>(ev => HandleNavigationEvent(keyValue));
            }
        }

        // Hide the UI until it is activated
        leverArchBinderRootVisualElement.style.display = DisplayStyle.None;
    }
    public bool AdvertiseInteraction()
    {
        actionIcon.enabled = true;
        actionHintTextMesh.enabled = true;
        actionHintTextMesh.text = actionHintMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Read");

        return (true);
    }

    /// <summary>
    /// Perform the interaction with the Security Keypad by displaying the UI and setting everything to an initial status so they're ready for use
    /// </summary>
    /// <returns>TRUE as we need further interactions with the user</returns>
    public bool PerformInteraction()
    {
        UnityEngine.Cursor.visible = true;
        interactionIsCompleted = InteractionStatus.Continuing;
        leverArchBinderRootVisualElement.style.display = DisplayStyle.Flex;

        HandleNavigationEvent("FIRST");

        return (true); // As we need further interactions to handle page movements
    }

    /// <summary>
    /// Terminate continued interaction if the ESCAPE key has been pressed
    /// </summary>
    /// <returns>TRUE if continued termination is no longer needed</returns>
    public InteractionStatus ContinueInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // NB the other mechanism by which the interaction is deemed complete is by the user entering the correct keycode and pressing the CLOSE button
            interactionIsCompleted = InteractionStatus.Completed;
        }

        // Hide the UI, if interaction is completed
        if (InteractionStatus.Completed == interactionIsCompleted)
        {
            leverArchBinderRootVisualElement.style.display = DisplayStyle.None;
        }

        return (interactionIsCompleted);
    }

    /// <summary>
    /// Handle a keyclick event from one of the Security keypad buttons
    /// </summary>
    /// <param name="keyValue">The value of the keypad that was pressed/clicked</param>
    public void HandleNavigationEvent(string keyValue)
    {
        switch (keyValue.ToUpperInvariant())
        {
            case "FIRST":
                currentPageNumber = 0;
                break;
            case "PREVIOUS":
                if (currentPageNumber > 0)
                {
                    currentPageNumber--;
                }
                break;
            case "NEXT":
                if(currentPageNumber < numberOfPages-1)
                {
                    currentPageNumber++;
                }
                break;
            case "LAST":
                currentPageNumber = numberOfPages-1;
                break;
            default:
                break;
        }
        leverArchBinderPage.text = bookPages[currentPageNumber];
    }


    /// <summary>
    /// Set an action message to indicate whether the named Lever Arch Binder will be locked or unlocked
    /// </summary>
    /// <returns></returns>
    private string SetActionMessage()
    {
        string returnActionMessage = originalActionHintMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Read");

        return (returnActionMessage);
    }
}
