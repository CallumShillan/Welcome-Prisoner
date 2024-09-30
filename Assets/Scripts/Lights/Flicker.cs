using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Thanks to: https://answers.unity.com/questions/742466/camp-fire-light-flicker-control.html

public class Flicker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The maximum step down in a given flicker")]
    private float maxReduction = 0.2f;

    [SerializeField]
    [Tooltip("The maximum step up in a given flicker")]
    private float maxIncrease = 0.2f;

    [SerializeField]
    [Tooltip("How fast the flicker is (bigger is slower)")]
    private float rateDamping = 0.1f;

    [SerializeField]
    [Tooltip("How pronounced the flicker is (bigger is more)")]
    private float strength = 300.0f;

    [SerializeField]
    [Tooltip("Whether to stop flickering")]
    private bool stopFlickering = false;

    private Light lightSource;
    private float baseIntensity;
    private bool isFlickering;
    
    // Start is called before the first frame update
    void Start()
    {
        lightSource = GetComponent<Light>();
        if (false == TryGetComponent<Light>(out lightSource))
        {
            GameLog.ErrorMessage(this, "Flicker script must have a Light Component on {0}. Did you forget to add one in the Inspector?", this.name);
            return;
        }

        baseIntensity = lightSource.intensity;
        StartCoroutine(DoFlicker());
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopFlickering && !isFlickering)
        {
            StartCoroutine(DoFlicker());
        }
    }

    private IEnumerator DoFlicker()
    {
        isFlickering = true;
        while (!stopFlickering)
        {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity, Random.Range(baseIntensity - maxReduction, baseIntensity + maxIncrease), strength * Time.deltaTime);
            yield return new WaitForSeconds(rateDamping);
        }
        isFlickering = false;
    }
}
