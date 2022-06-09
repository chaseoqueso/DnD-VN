using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatBackground : MonoBehaviour
{
    [SerializeField] private Image background;
    
    private const float FADE_DURATION = 1.5f;

    void Start()
    {
        StartCoroutine(fadeRoutine(false, FADE_DURATION));
    }

    private IEnumerator fadeRoutine(bool fadeIn, float duration)
    {
        float progress = 0;
        float startTime = Time.time;

        float startAlpha = fadeIn ? 1 : 0;
        float endAlpha = fadeIn ? 0 : 1;

        while(progress < 1)
        {
            progress = (Time.time - startTime) / duration;
            background.color = new Color(1, 1, 1, Mathf.SmoothStep(startAlpha, endAlpha, progress));
            yield return null;
        }

        StartCoroutine(fadeRoutine(!fadeIn, FADE_DURATION));
    }
}
