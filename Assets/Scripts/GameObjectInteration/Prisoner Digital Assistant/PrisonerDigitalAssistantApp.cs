using System;
using UnityEngine;
using UnityEngine.UIElements;

// All the PDA Applications
[Serializable]
// public enum PdaAppId { HomeScreen, Library, EbookReader, Notepad, Story, Options };
public enum PdaAppId { PDAHomePage, EbookReader, QuestDetails, MessageReader, ExitGame };

[Serializable]
public struct PrisonerDigitalAssistantApp
{
    [SerializeField]
    [Tooltip("The ID of the PDA App")]
    public PdaAppId pdaAppID;

    [SerializeField]
    [Tooltip("The Visual Tree Asset for the PDA App")]
    public VisualTreeAsset appVisualTreeAsset;
}
