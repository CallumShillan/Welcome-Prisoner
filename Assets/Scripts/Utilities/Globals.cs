using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour
{
    public static Globals Instance { get; private set; }
    public AudioSource CurrentAudioSource { get => currentAudioSource; set => currentAudioSource = value; }

    private AudioSource currentAudioSource;

    void Awake()
    {
        try
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(this, "Globals Awake() exception: {0}", ex.ToString());
        }
        GameLog.NormalMessage(this, "Globals Awake() finished");
    }
}
