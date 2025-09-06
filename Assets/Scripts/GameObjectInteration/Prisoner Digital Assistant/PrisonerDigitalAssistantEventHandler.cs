using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

public class PrisonerDigitalAssistantEventHandler : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The player's Game Object")]
    private GameObject player = null;

    [SerializeField]
    [Tooltip("The UI Document used to host and display all PDA Apps")]
    private UIDocument pdaAppHostUIDocument = null;

    [SerializeField]
    [Tooltip("The UI Document's Visual Element used as the container to host all PDA Apps")]
    private string pdaAppContainer = String.Empty;

    [SerializeField]
    [Tooltip("The UI Document's Visual Element used as the PDA Time Readout")]
    private string pdaTimeReadout = String.Empty;

    [SerializeField]
    [Tooltip("The PDA Apps")]
    private List<PrisonerDigitalAssistantApp> allPdaApps = null;

    private Label labelTimeReadout = null;

    // A parameter for the PDA App
    private string appParameter = string.Empty;
    public string AppParameter { get => appParameter; set => appParameter = value; }
    public IActionInterface ActionInterface { get => actionInterface; set => actionInterface = value; }
    public bool ShowingPda { get => showingPda; set => showingPda = value; }

    // A dictionary of root Visual Elements for each PDA App Page
    private Dictionary<PdaAppId, VisualElement> allPdaAppVisualTrees = new Dictionary<PdaAppId, VisualElement>();

    // The Root Visual Element of the PDA
    private VisualElement pdaRootVisualElement = null;

    // The Visual Element for the PDA App Host
    private VisualElement pdaAppHost = null;

    // The PDA Interaction Handler
    //private PrisonerDigitalAssistantInteractionHandler pdaInteractionHandler = null;

    private IActionInterface actionInterface = null;

    private bool showingPda = false;

    public enum ScreenTimeUpdate { Immediate, OncePerMinute };

    long lastTimeScreenUpdate = 0;

    // There are 10,000,000 ticks in a second
    // See https://docs.microsoft.com/en-us/dotnet/api/system.datetime.ticks
    private const long NUMBER_OF_TICKS_IN_ONE_MINUTE = 60 * 10000000;

    // Start is called before the first frame update
    void Start()
    {
        // Check if we have a Player Game Object
        if (player == null)
        {
            GameLog.ErrorMessage(this, "There is no Player Game Object. Did you forget to set one in the Inspector?");
            return;
        }

        // Check if we have a UI Document to function as the host for all PDS Apps
        if (pdaAppHostUIDocument == null)
        {
            GameLog.ErrorMessage(this, "Pda App Host UI Document is null. Did you forget to set one in the Inspector?");
            return;
        }

        // Get the Root Visual Element
        pdaRootVisualElement = pdaAppHostUIDocument.rootVisualElement;
        if(pdaRootVisualElement is null)
        {
            GameLog.ErrorMessage(this, "Pda App Host Root Visual Element is null");
            return;
        }

        // Get the Label used for the PDA Time Readout
        labelTimeReadout = pdaRootVisualElement.Q<Label>(pdaTimeReadout);
        if (labelTimeReadout is null)
        {
            GameLog.ErrorMessage($"Unable to find label '{pdaTimeReadout}' in Root Visual Element '{pdaRootVisualElement.name}'");
            return;
        }
        UpdateTimeReadout(DateTime.Now);

        // Check we have a name for the PDA App host Visual Element
        if (string.IsNullOrEmpty(pdaAppContainer) || string.IsNullOrWhiteSpace(pdaAppContainer))
        {
            GameLog.ErrorMessage(this, "Pda App Container is null, empty or whitespace. Did you forget to set it in the Inspector?");
            return;
        }

        // Get the Visual Element that functions as the PDA App Host
        pdaAppHost = pdaRootVisualElement.Q<VisualElement>(pdaAppContainer);
        if (pdaAppHost is null)
        {
            GameLog.ErrorMessage(this, "Pda App Host is null");
            return;
        }

        // Wire up the PDA's HOME button to display the Home Screen
        Button buttonHome = pdaRootVisualElement.Q<Button>("btn-Home");
        if (buttonHome is null)
        {
            GameLog.ErrorMessage(this, "Unable to find the Home button in the PDA App Host");
            return;
        }
        buttonHome.RegisterCallback<ClickEvent>(ev => DisplayPdaApp(PdaAppId.PDAHomePage));


        // Wire up the POWER CONTROL to turn the PDA Off
        Button buttonPowerControl = pdaRootVisualElement.Q<Button>("btn-PowerControl");
        if (buttonPowerControl is null)
        {
            GameLog.ErrorMessage(this, "Unable to find the Power Control button in the PDA App Host");
            return;
        }
        buttonPowerControl.RegisterCallback<ClickEvent>(ev => { ClosePDA(); });

        // Hide the PDA display
        pdaRootVisualElement.style.display = DisplayStyle.None;
    }

    public void Update()
    {
        UpdateScreenTimeText(ScreenTimeUpdate.OncePerMinute);
    }

    public void DisplayPdaApp(PdaAppId pdaAppIdToShow)
    {
        player.SetActive(false);

        UnityEngine.Cursor.visible = true;

        if (!showingPda)
        {
            // Remove the CSS Class that hides the PDA Frame so that the opacity transition will trigger making the UI fade in
            //pdaAppHost.RemoveFromClassList("pop-animation-hide");
            pdaRootVisualElement.style.display = DisplayStyle.Flex;
        }

        PrisonerDigitalAssistantApp pdaApp = allPdaApps.Find(app => app.pdaAppID == pdaAppIdToShow);

        // Remember the PDA App's Visual Tree Asset and Identifier 
        VisualTreeAsset appVisualTreeAsset = pdaApp.appVisualTreeAsset;
        PdaAppId pdaAppIdentifier = pdaApp.pdaAppID;

        // Check we got a Visual Tree Asset for the PDA App
        if (appVisualTreeAsset is null)
        {
            GameLog.ErrorMessage(this, $"Visual Tree Asset for PDA App {pdaAppIdentifier} is null. Did you forget to set it in the Inspector?");
            return;
        }

        // Get a Template Container to hold the App's Visual Tree Asset
        //TemplateContainer pdaAppTemplateContainer = appVisualTreeAsset.Instantiate();
        VisualElement pdaAppVisualElement = appVisualTreeAsset.CloneTree();
        if (pdaAppVisualElement is null)
        {
            GameLog.ErrorMessage(this, $"Unable to instantiate the Visual Tree Asset for PDA App {pdaAppIdentifier}");
            return;
        }

        // Add the App's Template Container to the dictionary of allPdaAppVisualTrees
        if(!allPdaAppVisualTrees.ContainsKey(pdaAppIdentifier))
        {
            allPdaAppVisualTrees.Add(pdaAppIdentifier, pdaAppVisualElement);
        }

        // Wire up the PDA Apps
        switch (pdaAppIdentifier)
        {
            case PdaAppId.PDAHomePage:
                PdaHomePageHelper.WireUp(pdaAppVisualElement, this);
                break;
            case PdaAppId.MessageReader:
                MessageReaderHelper.WireUp(pdaAppVisualElement, this, "Messages", "Please select a message from the LHS ...");
                MessageReaderHelper.CreateNavigationForKnownMessages();
            break;
            case PdaAppId.EbookReader:
                EBookReaderHelper.WireUp(pdaAppVisualElement, this);
                //EBookReaderHelper.CreateNavigationForKnownBooks();
            break;
            case PdaAppId.QuestDetails:
                TaskAppHelper.WireUp(pdaAppVisualElement, this);
            break;
            case PdaAppId.ExitGame:
                ExitGameHelper.WireUp(pdaAppVisualElement, this);
                break;
            default:
                break;
        }

        // Display the PDA, and add the PDA App's root Visual Element into the PDA App host
        //pdaRootVisualElement.style.display = DisplayStyle.Flex;
        pdaAppVisualElement.name = "homepage-root"; // Match USS selector
        pdaAppVisualElement.style.flexGrow = 1;
        pdaAppVisualElement.style.flexShrink = 1;
        pdaAppVisualElement.style.flexBasis = new StyleLength(StyleKeyword.Undefined);
        pdaAppVisualElement.style.width = new StyleLength(Length.Percent(100));
        pdaAppVisualElement.style.height = new StyleLength(Length.Percent(100));

        pdaAppHost.Clear();
        pdaAppHost.Add(allPdaAppVisualTrees[pdaAppIdToShow]);

        showingPda = true;
    }

    public void ClosePDA()
    {
        // It's possible the PDA is being displayed as a result of an action such as displaying instructions or reading a book, and so on
        if (actionInterface is not null)
        {
            actionInterface.CompleteInteraction();
        }

        pdaRootVisualElement.style.display = DisplayStyle.None;
        pdaAppHost.AddToClassList("pop-animation-hide");

        for (int childCnt = 0; childCnt < transform.childCount; childCnt++)
        {
            transform.GetChild(childCnt).gameObject.SetActive(false);
        }

        showingPda = false;

        UnityEngine.Cursor.visible = false;

        player.SetActive(true);

        // As a result of using the PDA, there may be a game message to display    
        if (Globals.Instance.AfterUseGameMessageSpeakerIconTexture != null && !string.IsNullOrEmpty(Globals.Instance.AfterUseGameMessageTitle))
        {
            DisplayGameMessage.Instance.ShowGameMessage(Globals.Instance.AfterUseGameMessageSpeakerIconTexture, Globals.Instance.AfterUseGameMessageTitle);
            Globals.Instance.AfterUseGameMessageSpeakerIconTexture = null;
            Globals.Instance.AfterUseGameMessageTitle = string.Empty;
        }

    }

    public void UpdateTimeReadout(DateTime displayDateTime)
    {
        labelTimeReadout.text = displayDateTime.ToLocalTime().ToShortTimeString();
    }

    public void UpdateScreenTimeText(ScreenTimeUpdate screenTimeUpdateStyle)
    {
        // Record what the DateTime right now is
        DateTime rightNow = DateTime.Now;

        // Update the screen time field, as instructed by the "screenTimeUpdateStyle"
        switch (screenTimeUpdateStyle)
        {
            // Perform the screen time field update immediately
            case ScreenTimeUpdate.Immediate:
                PerformScreenTimeUpdate(rightNow);
                break;

            // Perform the screen update only if there's been an elapse of 1 minute
            case ScreenTimeUpdate.OncePerMinute:
                if (rightNow.Ticks > lastTimeScreenUpdate + NUMBER_OF_TICKS_IN_ONE_MINUTE)
                {
                    PerformScreenTimeUpdate(rightNow);
                }
                break;

            // There is no other style of update, so do nothing
            default:
                break;
        }
    }

    /// <summary>
    /// Perform a screen update of the time and record when this was done
    /// </summary>
    /// <param name="displayDateTime">DateTime to display</param>
    public void PerformScreenTimeUpdate(DateTime displayDateTime)
    {
        UpdateTimeReadout(displayDateTime);

        // Adjust the Last Time Update to be precisely at the previous minute (e.g., if it update at 13:24 and 35 seconds, remove the 35 seconds)
        DateTime modifiedDateTimeToBeAtTheMinute = DateTime.Now.AddSeconds(-1.0d * displayDateTime.Second);

        // Set the last Time Screen Update to be the when the minute changed over
        lastTimeScreenUpdate = modifiedDateTimeToBeAtTheMinute.Ticks;
    }
}
