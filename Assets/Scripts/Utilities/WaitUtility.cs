using UnityEngine;
using System.Collections;

public static class WaitUtility 
{
    public static void StartWait(float seconds)
    {
        CoroutineRunner.Instance.StartStaticCoroutine(WaitCoroutine(seconds));
    }

    private static IEnumerator WaitCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}

public class CoroutineRunner : Singleton<CoroutineRunner>
{
    public void StartStaticCoroutine(IEnumerator coroutine)
    {
        GameLog.NormalMessage($"Waiting starts at {Time.time}");
        StartCoroutine(coroutine);
        GameLog.NormalMessage($"Waiting ends at {Time.time}");
    }
}
