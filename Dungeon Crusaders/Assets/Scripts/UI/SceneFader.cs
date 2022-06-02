using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private AudioSettingsManager audioSettingsManager;
    [SerializeField] private Image image;
    [SerializeField][Range(0f, 10f)] private float fadeDuration = 1f;

    public IEnumerator FadeInScene()
    {
        yield return Fade(1f, 0f, fadeDuration);
    }

    public IEnumerator FadeOutScene()
    {
        yield return Fade(0f, 1f, fadeDuration);
    }

    private IEnumerator Fade(float from, float to, float fadeDuration)
    {
        SetProgress(from);

        var progress = 0f;
        while (progress < 1f)
        {
            var value = Mathf.Lerp(from, to, progress);
            SetProgress(value);

            progress += Time.unscaledDeltaTime / fadeDuration;

            yield return null;
        }

        SetProgress(to);

        yield return null;
    }

    private void SetProgress(float progress)
    {
        audioSettingsManager.UpdateMasterVolume(1 - progress);
        SetImageAlpha(progress);
    }

    private void SetImageAlpha(float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
