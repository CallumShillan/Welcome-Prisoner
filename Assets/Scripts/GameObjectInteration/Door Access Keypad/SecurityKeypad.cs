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
    // Constants for field names used in guard statements
    private const string FIELD_UIDOCUMENT = "securityKeypadUserInterfaceDocument";
    private const string FIELD_ROOTVISUALELEMENT = "securityKeypadRootVisualElement";
    private const string FIELD_ROOTFRAME = "RootFrame";
    private const string FIELD_KEYPADREADOUT = "keyPadReadout";
    private const string FIELD_KEYBUTTON_PREFIX = "Key-";

    // The UI Document that will be used to render the User Interface for the Security Keypad
    private UIDocument securityKeypadUserInterfaceDocument = null;

    // Used to hide and display the Security Keypad
    private VisualElement securityKeypadRootVisualElement = null;

    // The Keypad Readout label that displays the keys that have been pressed
    private UnityEngine.UIElements.Label keyPadReadout = null;

    // The keypad characters entered by the user
    private StringBuilder enteredKeys = null;

    public delegate void HandleCheckKeycode(string keyCode);

    private HandleCheckKeycode checkKeycodeHandler = null;

    /// <summary>
    /// Initializes the component by retrieving required references and validating their assignments.
    /// </summary>
    private void Awake()
    {
        bool somethingNeedsToBeFixed = false;

        // Guard: Try to get the UIDocument component
        if (!this.TryGetComponent<UIDocument>(out securityKeypadUserInterfaceDocument))
        {
            GameLog.ErrorMessage(this, $"Unable to get the UI Document for the Security Keypad. Field: {FIELD_UIDOCUMENT}");
            somethingNeedsToBeFixed = true;
        }

        // Guard: Root Visual Element
        securityKeypadRootVisualElement = securityKeypadUserInterfaceDocument?.rootVisualElement;
        if (securityKeypadRootVisualElement == null)
        {
            GameLog.ErrorMessage(this, $"No Root Visual Element found in the Security Keypad user interface document. Field: {FIELD_ROOTVISUALELEMENT}");
            somethingNeedsToBeFixed = true;
        }

        // Guard: Root Frame
        VisualElement securityKeypadAppRootFrame = securityKeypadRootVisualElement?.Q<VisualElement>(FIELD_ROOTFRAME);
        if (securityKeypadAppRootFrame == null)
        {
            GameLog.ErrorMessage(this, $"Unable to find a Visual Element called '{FIELD_ROOTFRAME}' in the UI Document.");
            somethingNeedsToBeFixed = true;
        }

        // Guard: Keypad Readout
        keyPadReadout = securityKeypadAppRootFrame?.Q<UnityEngine.UIElements.Label>("KeypadReadout");
        if (keyPadReadout == null)
        {
            GameLog.ErrorMessage(this, $"Can't find the 'KeypadReadout' in the UI Document. Field: {FIELD_KEYPADREADOUT}");
            somethingNeedsToBeFixed = true;
        }
        if (keyPadReadout != null)
        {
            keyPadReadout.text = string.Empty;
        }

        // Hook up event handlers to the buttons
        foreach (string keyValue in new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Enter", "Delete" })
        {
            UnityEngine.UIElements.Button btnVerify = securityKeypadAppRootFrame?.Q<UnityEngine.UIElements.Button>(FIELD_KEYBUTTON_PREFIX + keyValue);
            if (btnVerify == null)
            {
                GameLog.ErrorMessage(this, $"Can't find the '{FIELD_KEYBUTTON_PREFIX}{keyValue}' button in the UI Document.");
                somethingNeedsToBeFixed = true;
            }
            else
            {
                btnVerify.RegisterCallback<ClickEvent>(ev => HandleKeypadClick(keyValue));
            }
        }

        if (somethingNeedsToBeFixed)
        {
            GameLog.ErrorMessage(this, "Please fix the above issues in the Inspector.");
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
                if (checkKeycodeHandler != null)
                {
                    checkKeycodeHandler(keyPadReadout.text);
                }
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

    /// <summary>
    /// Displays the security keypad interface and initializes the necessary components for handling keycode input.
    /// </summary>
    /// <remarks>This method ensures that the keypad interface is visible and ready for user interaction. It
    /// clears any previously entered keycode and resets the keypad readout display. The provided <paramref
    /// name="checkKeycode"/> delegate is assigned to handle keycode validation.</remarks>
    /// <param name="checkKeycode">A delegate that defines the logic for validating the entered keycode. This is used to process user input and
    /// determine whether the keycode is correct.</param>
    public void Show(HandleCheckKeycode checkKeycode)
    {
        // Guard: Ensure required references are set
        if (enteredKeys == null)
        {
            enteredKeys = new StringBuilder();
        }
        else
        {
            enteredKeys.Clear();
        }

        if (keyPadReadout != null)
        {
            keyPadReadout.text = string.Empty;
        }

        checkKeycodeHandler = checkKeycode;

        if (securityKeypadRootVisualElement != null)
        {
            securityKeypadRootVisualElement.style.display = DisplayStyle.Flex;
        }
    }

    /// <summary>
    /// Hides the security keypad by setting its display style to none.
    /// </summary>
    /// <remarks>This method makes the security keypad invisible in the user interface.  Ensure that
    /// <c>securityKeypadRootVisualElement</c> is properly initialized before calling this method.</remarks>
    public void Hide()
    {
        if (securityKeypadRootVisualElement != null)
        {
            securityKeypadRootVisualElement.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Resets the state of the keypad by clearing entered keys and resetting the display text.
    /// </summary>
    /// <remarks>This method clears the collection of entered keys and sets the keypad readout text to an
    /// empty string. Ensure that <c>enteredKeys</c> and <c>keyPadReadout</c> are properly initialized before calling
    /// this method.</remarks>
    public void Reset()
    {
        if (enteredKeys != null)
        {
            enteredKeys.Clear();
        }
        if (keyPadReadout != null)
        {
            keyPadReadout.text = string.Empty;
        }
    }
}
