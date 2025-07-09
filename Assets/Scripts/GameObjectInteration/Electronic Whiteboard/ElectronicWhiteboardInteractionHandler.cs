using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ElectronicWhiteboardInteractionHandler : MonoBehaviour, IActionInterface
{
    // Constants for field names used in guard statements
    private const string FIELD_ACTION_ICON = "actionIcon";
    private const string FIELD_ACTION_HINT_TEXT_MESH = "actionHintTextMesh";
    private const string FIELD_ACTION_MESSAGE = "actionMessage";
    private const string FIELD_ELECTRONIC_WHITEBOARD_DISPLAYS = "electronicWhiteboardDisplays";
    private const string FIELD_ON_MATERIAL = "onMaterial";
    private const string FIELD_OFF_MATERIAL = "offMaterial";
    private const string FIELD_SIGNIFICANT_EVENT = "significantEvent";

    [SerializeField]
    [Tooltip("The icon displayed for this action")]
    private Image actionIcon = null;

    [SerializeField]
    [Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh = null;

    [SerializeField]
    [Tooltip("A tooltip for turning on/off the whiteboard")]
    private string actionMessage = string.Empty;

    [SerializeField]
    [Tooltip("Whether the whiteboard is on")]
    private bool electronicWhiteBoardIsOn = false;

    [SerializeField]
    [Tooltip("The electronic whiteboard Game Objects is on")]
    private GameObject[] electronicWhiteboardDisplays = null;

    [SerializeField]
    [Tooltip("Texture to use when the whiteboard is on")]
    private Material onMaterial = null;

    [SerializeField]
    [Tooltip("Texture to use when the whiteboard is off")]
    private Material offMaterial = null;

    [SerializeField]
    [Tooltip("Significant Game Event to raise")]
    private string significantEvent = string.Empty;

    private void Awake()
    {
        bool somethingNeedsToBeFixed = false;

        if (actionIcon == null)
        {
            GameLog.ErrorMessage(this, $"Action icon is not assigned. Field: {FIELD_ACTION_ICON}");
            somethingNeedsToBeFixed = true;
        }
        if (actionHintTextMesh == null)
        {
            GameLog.ErrorMessage(this, $"Action hint text mesh is not assigned. Field: {FIELD_ACTION_HINT_TEXT_MESH}");
            somethingNeedsToBeFixed = true;
        }
        if (string.IsNullOrWhiteSpace(actionMessage))
        {
            GameLog.ErrorMessage(this, $"Action message is not assigned or is empty. Field: {FIELD_ACTION_MESSAGE}");
            somethingNeedsToBeFixed = true;
        }
        if (electronicWhiteboardDisplays == null || electronicWhiteboardDisplays.Length == 0)
        {
            GameLog.ErrorMessage(this, $"Electronic whiteboard displays are not assigned or empty. Field: {FIELD_ELECTRONIC_WHITEBOARD_DISPLAYS}");
            somethingNeedsToBeFixed = true;
        }
        if (onMaterial == null)
        {
            GameLog.ErrorMessage(this, $"On material is not assigned. Field: {FIELD_ON_MATERIAL}");
            somethingNeedsToBeFixed = true;
        }
        if (offMaterial == null)
        {
            GameLog.ErrorMessage(this, $"Off material is not assigned. Field: {FIELD_OFF_MATERIAL}");
            somethingNeedsToBeFixed = true;
        }
        if (string.IsNullOrWhiteSpace(significantEvent))
        {
            GameLog.ErrorMessage(this, $"Significant event is not assigned or is empty. Field: {FIELD_SIGNIFICANT_EVENT}");
            somethingNeedsToBeFixed = true;
        }

        if (somethingNeedsToBeFixed)
        {
            GameLog.ErrorMessage(this, "Please fix the above issues in the Inspector.");
        }

        Material materialToUse = null;
        if (electronicWhiteBoardIsOn)
        {
            materialToUse = onMaterial;
        }
        else
        {
            materialToUse = offMaterial;
        }

        if (electronicWhiteboardDisplays != null)
        {
            foreach (GameObject lodDisplay in electronicWhiteboardDisplays)
            {
                if (lodDisplay != null && materialToUse != null)
                {
                    Renderer renderer = lodDisplay.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = materialToUse;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Displays interaction hints to the user, such as enabling icons and text prompts,  based on the current state of
    /// the electronic whiteboard.
    /// </summary>
    /// <remarks>This method updates the visibility and content of UI elements to advertise an interaction. 
    /// The displayed message dynamically adjusts based on whether the electronic whiteboard is  currently on or
    /// off.</remarks>
    /// <returns>Always returns <see langword="false"/>. The return value is reserved for future use or  extended functionality.</returns>
    public bool AdvertiseInteraction()
    {
        if (actionIcon != null)
        {
            actionIcon.enabled = true;
        }
        if (actionHintTextMesh != null)
        {
            actionHintTextMesh.enabled = true;
            if (electronicWhiteBoardIsOn)
            {
                actionHintTextMesh.text = actionMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Turn off");
            }
            else
            {
                actionHintTextMesh.text = actionMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Turn on");
            }
        }
        return false;
    }

    /// <summary>
    /// Performs the interaction with the electronic whiteboard, toggling its state between on and off.
    /// </summary>
    /// <returns>flase as we don't need further interactions</returns>
    public bool PerformInteraction()
    {
        Material textureToUse = null;
        if (electronicWhiteBoardIsOn)
        {
            electronicWhiteBoardIsOn = false;
            if (actionHintTextMesh != null)
            {
                actionHintTextMesh.text = actionMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Turn on");
            }
            textureToUse = offMaterial;
        }
        else
        {
            QuestManager.HandleSignificantEvent(significantEvent);
            electronicWhiteBoardIsOn = true;
            if (actionHintTextMesh != null)
            {
                actionHintTextMesh.text = actionMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Turn off");
            }
            textureToUse = onMaterial;
        }

        if (electronicWhiteboardDisplays != null)
        {
            foreach (GameObject lodDisplay in electronicWhiteboardDisplays)
            {
                if (lodDisplay != null && textureToUse != null)
                {
                    Renderer renderer = lodDisplay.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = textureToUse;
                    }
                }
            }
        }

        return false; // As we don't need further interactions
    }
}
