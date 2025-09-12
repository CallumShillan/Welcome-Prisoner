using System;
using UnityEngine;

[Serializable]

public class InteractionMessage
{
    // Whether to display a message of an interaction.
    [SerializeField, Tooltip("Should a Game Message be shown after the interaction?")]
    private bool showGameMessageAfterInteraction = false;
    public bool ShowGameMessageAfterInteraction => showGameMessageAfterInteraction;

    // The texture used to represent the speaker's icon in the game message.
    [SerializeField, Tooltip("The speaker's face icon texture")]
    private Texture2D speakerIconTexture;
    public Texture2D SpeakerIconTexture => speakerIconTexture;

    // The title of the game message that will be displayed.
    [SerializeField, Tooltip("The game message title")]
    [GameMessageFile, MessageAudioPreview]
    private string gameMessageTitle = string.Empty;
    public string GameMessageTitle => gameMessageTitle;

}
