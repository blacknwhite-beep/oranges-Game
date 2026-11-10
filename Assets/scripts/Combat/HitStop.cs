using System.Collections;
using UnityEngine;

public sealed class HitStop : MonoBehaviour
{
    private static HitStop instance;

    private Coroutine stopRoutine;
    private float previousTimeScale;
    private float resumeTime;

    public static void Request(float duration)
    {
        if (!Application.isPlaying || duration <= 0f) return;

        if (instance == null)
        {
            GameObject hitStopObject = new GameObject("HitStop");
            instance = hitStopObject.AddComponent<HitStop>();
            DontDestroyOnLoad(hitStopObject);
        }

        instance.Begin(duration);
    }

    private void Begin(float duration)
    {
        float requestedResumeTime = Time.realtimeSinceStartup + duration;

        if (stopRoutine != null)
        {
            resumeTime = Mathf.Max(resumeTime, requestedResumeTime);
            return;
        }

        previousTimeScale = Time.timeScale;
        resumeTime = requestedResumeTime;
        Time.timeScale = 0f;
        stopRoutine = StartCoroutine(WaitAndResume());
        Debug.Log($"[HitStop] Started for {duration} seconds.");
    }

    private IEnumerator WaitAndResume()
    {
        while (Time.realtimeSinceStartup < resumeTime)
        {
            yield return null;
        }

        Time.timeScale = previousTimeScale;
        stopRoutine = null;
        Debug.Log("[HitStop] Resumed.");
    }

    private void OnDestroy()
    {
        if (stopRoutine != null)
        {
            Time.timeScale = previousTimeScale;
        }

        if (instance == this)
        {
            instance = null;
        }
    }
}
