using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Duration of the pulsation")]
    private float totalSeconds = 2.0f;

    [SerializeField]
    [Tooltip("The maximum intensity to reach (0-8)")]
    private float maxIntensity = 8.0f;

    [SerializeField]
    [Tooltip("The minimum Intensity to reach (0-8)")]
    private float minIntensity = 0.0f;

    [SerializeField]
    [Tooltip("Whether to stop pulsating")]
    private bool stopPulsating = false;

    private Light theLight = null;

    // Start is called before the first frame update
    void Start()
    {
        if(false == TryGetComponent<Light>(out theLight))
        {
            GameLog.ErrorMessage(this, "Pulsate script must have a Light Component on {0}. Did you forget to add one in the Inspector?", this.name);
            return;
        }

        StartCoroutine(SinglePulsation());

    }

    // Update is called once per frame
    void Update()
    {
        if(!stopPulsating)
        {
            StartCoroutine(SinglePulsation());
        }
    }

    public IEnumerator SinglePulsation()
    {
        float waitTime = totalSeconds / 2;
        // Get half of the seconds (One half to get brighter and one to get darker)
        while (theLight.intensity < maxIntensity)
        {
            theLight.intensity += Mathf.PingPong(Time.time, maxIntensity);
            //Time.deltaTime / waitTime;        // Increase intensity
            //Mathf.Lerp(theLight.intensity, maxIntensity, strength * Time.deltaTime);
            yield return null;
        }
        while (theLight.intensity > minIntensity)
        {
            theLight.intensity -= Mathf.PingPong(Time.time, maxIntensity);
                //Time.deltaTime / waitTime;        //Decrease intensity
            //theLight.intensity -= Mathf.Lerp(theLight.intensity, minIntensity, strength * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
