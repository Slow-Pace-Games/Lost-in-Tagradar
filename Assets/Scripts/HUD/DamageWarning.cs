using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageWarning : MonoBehaviour
{
    [SerializeField] HealthComponent healthPlayer;
    [SerializeField] AnimationCurve easingCurve;

    private Image effectImage;

    private float timerEffect;
    private float maxTimerEffect;

    private bool coroutineIsRunning = false;

    private void Start()
    {
        effectImage = GetComponent<Image>();
        effectImage.enabled = false;
        healthPlayer.OnHealthChange += OnHealthChange;

        maxTimerEffect = 1f;
        timerEffect = 0f;
    }

    public void OnHealthChange(HealthType type)
    {
        if (type == HealthType.Damage && !coroutineIsRunning)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateWarning());
        }

    }

    private void OnDestroy()
    {
        if (healthPlayer != null)
        {
            healthPlayer.OnHealthChange -= OnHealthChange;
        }
    }

    private IEnumerator UpdateWarning()
    {
        coroutineIsRunning = true;
        effectImage.enabled = true;
        while (timerEffect < maxTimerEffect)
        {
            timerEffect += Time.deltaTime;
            float alpha = easingCurve.Evaluate(timerEffect);
            if (alpha < 0f)
            {
                alpha = 0f;
            }
            Color newColor = new Color(effectImage.color.r, effectImage.color.g, effectImage.color.b, alpha);
            effectImage.color = newColor;

            yield return null;
        }
        effectImage.enabled = false;
        coroutineIsRunning = false;
        timerEffect = 0f;
        yield return null;
    }
}
