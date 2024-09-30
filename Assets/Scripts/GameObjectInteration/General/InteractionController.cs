using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class InteractionController : MonoBehaviour
{

    #region Variable declarations
    [SerializeField]
    [Tooltip("The interval time, in seconds, for performing raycasts")]
    private float raycastInterval = 1.0f;

    [SerializeField]
    [Tooltip("How far to look for interactable objects")]
    private float rayDistance = 5.0f;

    [SerializeField]
    [Tooltip("The layer mask for interactable objects")]
    private LayerMask interactionLayerMask;

    [SerializeField]
    [Tooltip("The name of a layer mask to exclude when looking for interactable objects")]
    private string excludeLayerMaskName;

    [SerializeField]
    [Tooltip("The player's Game Object")]
    public GameObject player = null;

    [SerializeField]
    [Tooltip("The key used to trigger object interaction")]
    private KeyCode primaryInteractionKey = KeyCode.Mouse0;

    [SerializeField]
    [Tooltip("The cursor icon, always shown")]
    private Image iconCursor = null;

    [SerializeField]
    [Tooltip("The icon to show a known action is available")]
    private Image interactionIndicatorIcon;

    [SerializeField]
    [Tooltip("The icon to show an action is unknown")]
    private Image unknownActionIcon;

    [SerializeField]
    [Tooltip("An description of the action")]
    private string unknownActionDescription;

    [SerializeField]
    [Tooltip("The Text Mesh used to display the action description")]
    private TextMeshProUGUI textMeshActionHint = null;

    [SerializeField]
    [Tooltip("Number of seconds between cache clearances")]
    private int cacheClearanceInterval = 600;

    [SerializeField]
    [Tooltip("When clearing the cache, remove items held for more than this number of seconds")]
    private int maximumCacheResidency = 1200;

    // A cache for the actions as we need to use TryGetComponent() and we don't want to be executing this too frequently
    private readonly Dictionary<int, CachedAction> actionCache = new Dictionary<int, CachedAction>();

    // The date and time that the action cache was last cleaned
    private long ticksSinceLastCacheClean;

    // The last hit object
    private GameObject lastHitObject = null;

    // We need to remember objects that provide for continued diaogue with the user (e.g., computer screen, inventory management)
    private IActionInterface continuedUserDialogueActionInterface = null;

    private PrisonerDigitalAssistantEventHandler pdaEventHandler = null;

    // The time when we should next perform a raycast to search for interactive objects
    private float nextRaycastTime = 0f;
    #endregion

    private void Start()
    {
        #region Asserts
        try
        {
            // rayDistance must be a positive value
            Assert.IsTrue(rayDistance > 0.0f, "rayDistance must be positive");

            // A interaction layer mask has been defined
            Assert.IsNotNull(LayerMask.LayerToName(interactionLayerMask), "interactionLayerMask must be provided");

            // If a layer name has been provided, it must be a valid layer name
            Assert.IsTrue(string.IsNullOrEmpty(excludeLayerMaskName) ? true : LayerMask.NameToLayer(excludeLayerMaskName) != -1, "If a layer name has been provided, it must be a valid layer name");

            // A Player has been specified
            Assert.IsNotNull(player, "Player object must be defined");

            // We should have a primary interaction key
            Assert.IsTrue(primaryInteractionKey != KeyCode.None, "The Primary Interaction Key cannot be KeyCode.None");

            // The cursor icon should not be null
            Assert.IsNotNull(iconCursor, "The cursor icon should not be null");

            // The action prompt icon should not be null
            Assert.IsNotNull(interactionIndicatorIcon, "The action prompt icon should not be null");

            // The action unknown icon should not be null
            Assert.IsNotNull(unknownActionIcon, "The action unknown icon should not be null");

            // There should be an Unknown Action Description
            Assert.IsTrue(string.IsNullOrEmpty(unknownActionDescription) == false, "There should be an Unknown Action Description");

            // The textmesh to render the hint should not be null
            Assert.IsNotNull(textMeshActionHint, "The textmesh to render the hint should not be null");

            // The minimum cache clearance must be a positive value
            Assert.IsTrue(cacheClearanceInterval > 0, "The minimum cache clearance must be a positive value");

            // The maximum cache clearance must be a positive value
            Assert.IsTrue(maximumCacheResidency > 0, "The maximum cache clearance must be a positive value");
        }
        catch (AssertionException assertionException)
        {
            GameLog.ExceptionMessage(this, "Assert exception in InteractionController Start(): {0}", assertionException.Message);
        }
        #endregion

        #region Runtime Startup Checks
        try
        {
            if (rayDistance <= 0.0f)
            {
                throw new ArgumentOutOfRangeException("Must be a positive value", nameof(rayDistance));
            }

            if (player is null)
            {
                throw new ArgumentNullException("Must be set", nameof(player));
            }

            if (primaryInteractionKey == KeyCode.None)
            {
                throw new ArgumentOutOfRangeException("Cannot be set to 'None'", nameof(primaryInteractionKey));
            }

            if (iconCursor is null)
            {
                throw new ArgumentNullException("Must be set", nameof(iconCursor));
            }

            if (interactionIndicatorIcon is null)
            {
                throw new ArgumentNullException("Must be set", nameof(interactionIndicatorIcon));
            }

            if (unknownActionIcon is null)
            {
                throw new ArgumentNullException("Must be set", nameof(unknownActionIcon));
            }

            if (string.IsNullOrEmpty(unknownActionDescription))
            {
                throw new ArgumentNullException("Must be provided", nameof(unknownActionDescription));
            }

            if (textMeshActionHint is null)
            {
                throw new ArgumentNullException("Must be provided", nameof(textMeshActionHint));
            }

            if (cacheClearanceInterval < 0)
            {
                throw new ArgumentOutOfRangeException("Must be a positive value", nameof(cacheClearanceInterval));
            }

            if (maximumCacheResidency < 0)
            {
                throw new ArgumentOutOfRangeException("Must be a positive value", nameof(maximumCacheResidency));
            }

            if (false == TryGetComponent<PrisonerDigitalAssistantEventHandler>(out pdaEventHandler))
            {
                throw new ArgumentNullException("There is no PDA Handler on {0}. Did you forget to add one in the Inspector?", this.gameObject.name);
            }

        }
        catch (ArgumentOutOfRangeException argumentOutOfRangeException)
        {
            GameLog.ExceptionMessage(this, "Argument out of range in InteractionController Start(): {0}", argumentOutOfRangeException.Message);
        }
        catch (ArgumentNullException argumentNullException)
        {
            GameLog.ExceptionMessage(this, "Argument cannot be null in InteractionController Start(): {0}", argumentNullException.Message);
        }
        catch (Exception thrownException)
        {
            GameLog.ExceptionMessage(this, "Unexpected exception: {0}", thrownException.Message);
        }
        #endregion

        ticksSinceLastCacheClean = DateTime.Now.Ticks;

        Cursor.visible = false;
    }

    // Thanks to https://answers.unity.com/questions/411793/selecting-a-game-object-with-a-mouse-click-on-it.html for help with identifying which object has been subject to a mouse-click
    private void Update()
    {
        if(pdaEventHandler is not null)
        {
            if (pdaEventHandler.ShowingPda)
            {
                return;
            }
        }

        // If we need to allow continued user interaction to occur, do so
        if (continuedUserDialogueActionInterface is not null)
        {
            // Continue interaction with the object 
            InteractionStatus statusOfInteraction = continuedUserDialogueActionInterface.ContinueInteraction();
            switch (statusOfInteraction)
            {
                case InteractionStatus.Completed:
                    GameLog.NormalMessage(this, "No more continued dialogue needed", string.Empty);
                    continuedUserDialogueActionInterface = null;

                    // For continued interaction, the player was disabled so they didn't respond to user input and move about
                    // As the player has finished interacting with the object, we need to re-enable them
                    Cursor.visible = false;
                    player.SetActive(true);
                    break;

                case InteractionStatus.ShowPdaHomeScreen:
                    pdaEventHandler.DisplayPdaApp(PdaAppId.PDAHomePage);
                    continuedUserDialogueActionInterface = null;
                    break;

            }

            // The work associated with this Update() has completed, so RETURN and end the processing
            return;
        }

        //if (Time.time >= nextRaycastTime)
        //{
            nextRaycastTime = Time.time + raycastInterval;

            //GameLog.NormalMessage(this, "Processing the normal game object interaction mechanisms");

            // When a raycast is sent out, this variable is used to record the game object it hit (if any)
            GameObject hitObject = null;

            // The action interface of the hit object
            IActionInterface hitObjectActionInterface = null;

            // Whether the hit object has other interactions (e.g., an inventory management system) or if it provides a single-shot interaction (e.g., opening/closing a door, turning a light on/off)
            bool objectHasOtherInterations = false;

            // Determine the layers we want the raycast to report on
            int interactableGameObjectsLayerMask = 1 << LayerMask.NameToLayer(excludeLayerMaskName) | interactionLayerMask.value;

            // When sending a ray out, this variable reports back on what was hit, if anything
            RaycastHit raycastHitObject = new RaycastHit();

            // Send the ray from the camera through the cursor icon for a certain distance - we only look for Game Objects that have been tagged as being interactable
            if (Physics.Raycast(Camera.main.ScreenPointToRay(iconCursor.transform.position), out raycastHitObject, rayDistance, interactableGameObjectsLayerMask))
            {
                // If we're here, it's because the ray hit an interactable object, so now we need to handle this situation

                // Determine the object that we hit
                hitObject = raycastHitObject.collider.gameObject;

                // Record the ID of the object we hit to be used as the key when it is inserted in the action cache
                int objectID = hitObject.GetInstanceID();

                // Enable the Action Icon so the player knows there is an action that can be performed
                interactionIndicatorIcon.enabled = true;

                // Get the Action Interface for the hit object
                hitObjectActionInterface = GetActionInterface(hitObject);
                if (hitObjectActionInterface is not null)
                {
                    // We only want to advertise the action when when the user is looking at something new
                    if (hitObject != lastHitObject)
                    {
                        // Advertise the action that can be performed
                        unknownActionIcon.enabled = false;
                        interactionIndicatorIcon.enabled = true;
                        textMeshActionHint.enabled = true;

                        // Let the hit object advertise its interaction mechansim as it needs to (e.g., left-click, right-click, press E, and so on)
                        // Also, record whether the object has indicated it is capable of further interactions
                        objectHasOtherInterations = hitObjectActionInterface.AdvertiseInteraction();
                    }
                    else
                    {
                        //GameLog.LogMessage(LogType.Log, hitObject,  "No need to advertise the possible actions as these are already being shown");
                    }

                    // If the user wants to interact with the object
                    if (Input.GetKeyDown(primaryInteractionKey))
                    {
                        GameLog.Message(LogType.Warning, hitObject, "Performing primary action");
                        if (hitObjectActionInterface.PerformInteraction())
                        {
                            GameLog.Message(LogType.Warning, hitObject, "Allowing for further interactions");

                            // As we're doing continued interactions, we need to disable the player so they don't respond to the keystrokes and, say, move around
                            player.SetActive(false);
                            continuedUserDialogueActionInterface = hitObjectActionInterface;
                        }
                    }
                }
                else
                {
                    // Indicate we don't know how to advertise or perform actions
                    interactionIndicatorIcon.enabled = false;
                    unknownActionIcon.enabled = true;
                    textMeshActionHint.text = unknownActionDescription;
                    textMeshActionHint.enabled = true;
                }

                if (objectHasOtherInterations)
                {
                    //GameLog.LogMessage(LogType.Log, hitObject, "Object has other interactions, so setting Last Hit Object to null so that other action advertisement will occur");
                    lastHitObject = null;
                }
                else
                {
                    //GameLog.LogMessage(LogType.Log, hitObject, "Setting Last Hit Object to {0} (id={1})", new object[] { hitObject.name, hitObject.GetInstanceID() });
                    lastHitObject = hitObject;
                }
            }
            else
            {
                // The raycast didn't hit an interactable game object, so hide all interaction icons
                lastHitObject = null;

                interactionIndicatorIcon.enabled = false;
                unknownActionIcon.enabled = false;
                textMeshActionHint.enabled = false;
            }

            // Record when "right now" is
            long ticksRightNow = DateTime.Now.Ticks;

            // Check if we should clean the cache, and do so, if needed
            if (ticksRightNow > (ticksSinceLastCacheClean + (TimeSpan.TicksPerSecond * cacheClearanceInterval)))
            {
                GameLog.NormalMessage(this, "Time to clean the Action Cache as ticksRightNow = " + ticksRightNow.ToString() + " and ticksSinceLastCacheClean = " + ticksSinceLastCacheClean.ToString());
                ticksSinceLastCacheClean = ticksRightNow;
                CleanCache(ticksRightNow);
            }
        //}

    }

    /// <summary>
    /// Clean the cache of stale items that have lived in the cache for too long
    /// </summary>
    /// <param name="ticksRightNow">The tick value for right now (DateTime.Now.Ticks)</param>
    private void CleanCache(long ticksRightNow)
    {
        CachedAction cachedAction;

        // If we remove items from the cache within the FOREACH loop, we modify the collection and the enumeration operation may not execute as expected
        // So we keep a list of all items that need to be remove and do the removal in a later step
        List<int> cachedItemsToRemove = new List<int>();

        foreach (int objectId in actionCache.Keys)
        {
            if (actionCache.TryGetValue(objectId, out cachedAction))
            {
                if (ticksRightNow > (cachedAction.InsertionTick + (TimeSpan.TicksPerSecond * maximumCacheResidency)))
                {
                    GameLog.NormalMessage(this, "<color=red>Game Object ID = {0} should be removed from the Action Cache</color>", objectId.ToString());
                    cachedItemsToRemove.Add(objectId);
                }
            }
            else
            {
                GameLog.NormalMessage(this, "Unable to retrieve action cache item for Game Object ID = {0}", objectId.ToString());
            }
        }

        // Now we can safely delete the items that need to be removed from the cache
        foreach (int objectId in cachedItemsToRemove)
        {
            actionCache.Remove(objectId);
        }
    }

    /// <summary>
    /// Get the Action Interface for a Game Object
    /// </summary>
    /// <param name="hitObject">The Game Object to get the Action Interface for</param>
    /// <returns>An Action Interface, if there is one for the Game Object; null otherwise</returns>
    private IActionInterface GetActionInterface(GameObject hitObject)
    {
        IActionInterface returnActionInterface = null;

        int hitObjectID = hitObject.GetInstanceID();
        string hitObjectName = hitObject.name;

        // See if we can find the action interface in the cache
        if (actionCache.ContainsKey(hitObjectID))
        {
            // It appears as if the Action Cache holds an entry, so try and get it
            CachedAction cachedActionItem = new CachedAction();
            if (actionCache.TryGetValue(hitObjectID, out cachedActionItem))
            {
                // Log that we got the Action Interface
                //GameLog.NormalMessage(hitObject, "Found the Action Interface for {0} (id={1}) in the Action Interface cache", hitObjectName, hitObjectID);
                return (cachedActionItem.Action);
            }
            else
            {
                // Log that we were unable to get the Action Interface
                GameLog.ErrorMessage(hitObject, "Couldn't find Action Interface for {0} (id={1}) in the Action Interface cache", hitObjectName, hitObjectID.ToString());
                return (null);
            }
        }

        if(hitObject.TryGetComponent(out returnActionInterface))
        {
            GameLog.NormalMessage(this, "<color=red>Action Interface for Game Object '{0}' ID = {1} being added to the Action Cache</color>", hitObjectName, hitObjectID.ToString());
            CachedAction cachedActionItem = new CachedAction(DateTime.Now.Ticks, returnActionInterface);
            actionCache.Add(hitObjectID, cachedActionItem);

            // Return the Action Interface
            return (returnActionInterface);
        }
        else
        {
            GameLog.ErrorMessage(hitObject, "Unable to find an Action Interface for GameObject {0}.  Did you forget to add one?", hitObject.name);
        }

        return (null);

    }

    public void HideInteractionCursors()
    {
        interactionIndicatorIcon.enabled = false;
        unknownActionIcon.enabled = false;
        textMeshActionHint.enabled = false;
    }
}
