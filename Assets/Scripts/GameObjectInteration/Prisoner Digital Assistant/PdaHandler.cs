using System;
using UnityEngine;

public class PdaHandler : MonoBehaviour
{
    // The PDA Event Handler used to control the PDA events (e.g., turn on/off, display a given PDA App)
    private PrisonerDigitalAssistantEventHandler pdaEventHandler = null;

    // Whether or not the PDA is displayed
    private bool pdaIsDisplayed = false;

    // Start is called before the first frame update
    void Start()
    {
        pdaIsDisplayed = false;

        // Get the PDA Event Handler component
        if (false == this.TryGetComponent<PrisonerDigitalAssistantEventHandler>(out pdaEventHandler))
        {
            GameLog.ErrorMessage(this, "Unable to get the PDA Event Handler. Did you forget to add one?");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pdaIsDisplayed)
            {
                pdaEventHandler.ClosePDA();
            }
            else
            {
                pdaEventHandler.DisplayPdaApp(PdaAppId.PDAHomePage);
            }
            pdaIsDisplayed = !pdaIsDisplayed;
        }
    }
}
