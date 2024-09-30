using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ElectronicWhiteboardInteractionHandler : MonoBehaviour, IActionInterface
{
    [SerializeField]
    [Tooltip("The icon displayed for this action")]
    private Image actionIcon = null;

    [SerializeField]
    [Tooltip("The text mesh to display the hint")]
    private TextMeshProUGUI actionHintTextMesh;

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

    public void Start()
    {
        Material materialToUse = null;
        if (electronicWhiteBoardIsOn)
        {
            materialToUse = onMaterial;
        }
        else
        {
            materialToUse = offMaterial;
        }

        foreach (GameObject lodDisplay in electronicWhiteboardDisplays)
        {
            lodDisplay.GetComponent<Renderer>().material = materialToUse;
        }
    }

    public bool AdvertiseInteraction()
    {
        actionIcon.enabled = true;
        actionHintTextMesh.enabled = true;

        if (electronicWhiteBoardIsOn)
        {
            actionHintTextMesh.text = actionMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Turn off");
        }
        else
        {
            actionHintTextMesh.text = actionMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Turn on");
        }
        return (false);
    }

    public bool PerformInteraction()
    {
        Material textureToUse = null;
        if (electronicWhiteBoardIsOn)
        {
            electronicWhiteBoardIsOn = false;
            actionHintTextMesh.text = actionMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Turn on");
            textureToUse = offMaterial;
        }
        else
        {
            QuestManager.HandleSignificantEvent(significantEvent);

            electronicWhiteBoardIsOn = true;
            actionHintTextMesh.text = actionMessage.Replace("{NAME}", this.name).Replace("{ACTION}", "Turn off");
            textureToUse = onMaterial;
        }

        foreach( GameObject lodDisplay in electronicWhiteboardDisplays )
        {
            lodDisplay.GetComponent<Renderer>().material = textureToUse;
        }

        return (false); // As we don't need further interactions
    }
}
