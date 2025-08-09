using System.Collections.Generic;
using UnityEngine;

public class AudioClips : Singleton<AudioClips>
{
    private static Dictionary<string, AudioClip> audioClipDictionary;

    static public float PlayClipAtPoint(string audioClipName, Vector3 position)
    {
        if (audioClipDictionary is null)
        {
            AudioClip[] audioClips;

            // Load all AudioClips from the Resources/Audio folder
            audioClips = Resources.LoadAll<AudioClip>("Audio");

            audioClipDictionary = new Dictionary<string, AudioClip>();

            // Load the named AudioClips into the dictionary
            foreach (AudioClip singleAudioClip in audioClips)
            {
                audioClipDictionary.Add(singleAudioClip.name, singleAudioClip);
            }
        }

        if (audioClipDictionary.TryGetValue(audioClipName, out AudioClip clipToPLay))
        {
            AudioSource.PlayClipAtPoint(clipToPLay, position);
        }
        else
        {
            GameLog.ErrorMessage($"Audio Clip {audioClipName} not found");
            return 0f;
        }

        return clipToPLay.length;
    }
}

