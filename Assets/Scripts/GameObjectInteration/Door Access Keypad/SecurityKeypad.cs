using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SecurityKeypad : MonoBehaviour
{
    // The UI Document that will be used to render the User Interface for the Security Keypad
    private UIDocument securityKeypadUserInterfaceDocument = null;

    // Used to hide and display the Security Keypad
    private VisualElement securityKeypadRootVisualElement = null;

    // The Keypad Readout label that displays the keys that have been pressed
    private UnityEngine.UIElements.Label keyPadReadout = null;

    // The keypad characters entered by the user
    private StringBuilder enteredKeys = null;

    // Whether the interaction of the Security Keypad is completed
    //private bool interactionIsCompleted = false;
    
    public delegate void HandleCheckKeycode(string keyCode);

    private HandleCheckKeycode checkKeycodeHandler = null;


    // Start is called before the first frame update
    void Start()
    {
        if( false == this.TryGetComponent<UIDocument>(out securityKeypadUserInterfaceDocument) )
        {
            GameLog.ErrorMessage(this, "Unable to get the UI Document for the Security Keypad.");
            return;
        }

        // Remember the Security Keypad's Root Visual Element
        securityKeypadRootVisualElement = securityKeypadUserInterfaceDocument.rootVisualElement;
        if (securityKeypadRootVisualElement is null)
        {
            GameLog.ErrorMessage(this, "No Root Visual Element found in the Security Keypad user interface document.");
            return;
        }

        // Get the Root Frame of the Security Keypad UI
        VisualElement securityKeypadAppRootFrame = securityKeypadRootVisualElement.Q<VisualElement>("RootFrame");
        if (securityKeypadAppRootFrame is null)
        {
            GameLog.ErrorMessage(this, "Unable to find a Visual Element called 'RootFrame' in the UI Document.");
            return;
        }

        // Clear the Keypad Readout label from
        keyPadReadout = securityKeypadAppRootFrame.Q<UnityEngine.UIElements.Label>("KeypadReadout");
        if (keyPadReadout is null)
        {
            GameLog.ErrorMessage(this, "Can't find the 'KeypadReadout' in the UI Document.");
            return;
        }
        keyPadReadout.text = string.Empty;

        // Hook up event handlers to the buttons
        foreach (string keyValue in new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Enter", "Delete" })
        {
            UnityEngine.UIElements.Button btnVerify = securityKeypadAppRootFrame.Q<UnityEngine.UIElements.Button>("Key-" + keyValue);
            if (btnVerify is null)
            {
                GameLog.ErrorMessage(this, "Can't find the 'Key-" + keyValue + "' button in the UI Document.");
                return;
            }
            else
            {
                btnVerify.RegisterCallback<ClickEvent>(ev => HandleKeypadClick(keyValue));
            }
        }

        enteredKeys = new StringBuilder();

        this.Hide();
    }

    /// <summary>
    /// Handle a keyclick event from one of the Security keypad buttons
    /// </summary>
    /// <param name="keyValue">The value of the keypad that was pressed/clicked</param>
    private void HandleKeypadClick(string keyValue)
    {
        switch (keyValue.ToUpperInvariant())
        {
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
            case "0":
                enteredKeys.Append(keyValue);
                keyPadReadout.text = enteredKeys.ToString();
                break;

            case "ENTER":
                checkKeycodeHandler(keyPadReadout.text);
                break;

            case "DELETE":
                if (enteredKeys.Length > 0)
                {
                    enteredKeys.Remove(enteredKeys.Length - 1, 1);
                    keyPadReadout.text = enteredKeys.ToString();
                }
                break;
        }
    }

    public void Show(HandleCheckKeycode checkKeycode)
    {
        // Show the UI
        enteredKeys.Clear();
        keyPadReadout.text = string.Empty;

        checkKeycodeHandler = checkKeycode;

        securityKeypadRootVisualElement.style.display = DisplayStyle.Flex;
    }
    public void Hide()
    {
        // Hide the UI
        securityKeypadRootVisualElement.style.display = DisplayStyle.None;
    }

    public void Reset()
    {
        enteredKeys.Clear();
        keyPadReadout.text = string.Empty;
    }
}
