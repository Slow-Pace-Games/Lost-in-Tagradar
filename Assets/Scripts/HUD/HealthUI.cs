using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum HealthType
{
    Damage,
    Heal,
}

public class HealthUI : MonoBehaviour
{
    #region Class
    private static class HealthColor
    {
        public static Color Hit = new Color(160, 0, 0);
        public static Color Heal = new Color(10, 100, 25);
        public static Color Normal = Color.white;
    }
    private class HealtBar
    {
        public Image bar;
        public bool isEmpty;
        public bool isFull;
        public int healthInBar;

        public float startPosX;
        public float currentPosX { get => bar.transform.localPosition.x; }
        public float currentPosY { get => bar.transform.localPosition.y; }
        public Vector2 SetBarPos { set => bar.transform.localPosition = value; }

        public void BarColor(Color barColor) => bar.color = barColor;
        public void ResetBar(int healthInBar)
        {
            isEmpty = false;
            isFull = true;
            this.healthInBar = healthInBar;
            SetBarPos = new Vector2(startPosX, currentPosY);
            bar.enabled = true;
        }
        public HealtBar(bool isEmpty, bool isFull, int healthInBar, Image bar)
        {
            this.isEmpty = isEmpty;
            this.isFull = isFull;
            this.healthInBar = healthInBar;

            this.bar = bar;
            if (bar == null)
                Debug.LogWarning("No bar set in " + this);
            else
                startPosX = bar.transform.localPosition.x;
        }
    }
    #endregion

    private HealtBar[] healtBars = new HealtBar[10];

    private int previousHealth;
    private float timeMultiplier = 10f;

    private bool crtFinishBar = false;
    private bool crtStart = false;

    #region Init
    private void Start()
    {
        previousHealth = Player.Instance.GetCurrentHealthPoint();
        InitHealthBars();
    }
    private void InitHealthBars()
    {
        int healtPerBar = Player.Instance.GetMaxHealthPoint() / healtBars.Length;
        for (int i = 0; i < healtBars.Length; i++)
        {
            healtBars[i] = new HealtBar(false, true, healtPerBar, transform.GetChild(i).GetComponent<Image>());
        }
    }
    #endregion

    #region Bar Display
    private void DisableEmptyBar(int indexBar)
    {
        HealtBar selectHealthBar = healtBars[indexBar];

        selectHealthBar.isEmpty = true;
        selectHealthBar.bar.enabled = false;
        selectHealthBar.isFull = false;
        selectHealthBar.healthInBar = 0;
    }
    private void ColorHitAllBar()
    {
        for (int i = 0; i < healtBars.Length; i++)
        {
            healtBars[i].BarColor(HealthColor.Hit);
        }
    }
    private void ColorNormalAllBar()
    {
        for (int i = 0; i < healtBars.Length; i++)
        {
            healtBars[i].BarColor(HealthColor.Normal);
        }
    }
    private void ColorHealAllBar()
    {
        for (int i = 0; i < healtBars.Length; i++)
        {
            healtBars[i].BarColor(Color.green);
        }
    }
    #endregion

    #region Coroutine
    private IEnumerator DecreaseHealthBar(int indexBar, int damageToInflict)
    {
        float timer = 0f;

        float currentPosX = healtBars[indexBar].currentPosX;
        while (timer <= 1f)
        {
            healtBars[indexBar].SetBarPos = new Vector2(Mathf.Lerp(currentPosX, currentPosX - damageToInflict, timer), healtBars[indexBar].currentPosY);

            float time = TimeScale.deltaTime * timeMultiplier;
            timer += time;
            if (timer > 1f && timer != 1f + time)
                timer = 1f;

            yield return null;
        }

        healtBars[indexBar].healthInBar -= damageToInflict;

        healtBars[indexBar].isFull = false;
        healtBars[indexBar].isEmpty = false;

        if (healtBars[indexBar].healthInBar == 0)
            DisableEmptyBar(indexBar);

        crtFinishBar = true;
        crtStart = false;
    }
    private IEnumerator HealHealthBar(int indexBar, int healToInflict)
    {
        float timer = 0f;
        float currentPosX = healtBars[indexBar].currentPosX;
        healtBars[indexBar].bar.enabled = true;

        while (timer <= 1f)
        {
            healtBars[indexBar].SetBarPos = new Vector2(Mathf.Lerp(currentPosX, currentPosX + healToInflict, timer), healtBars[indexBar].currentPosY);
            float time = TimeScale.deltaTime * timeMultiplier;
            timer += time;
            if (timer > 1f && timer != 1f + time)
                timer = 1f;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        healtBars[indexBar].healthInBar += healToInflict;
        healtBars[indexBar].isEmpty = false;

        if (healtBars[indexBar].healthInBar == 10)
            healtBars[indexBar].ResetBar(10);

        crtFinishBar = true;
        crtStart = false;
    }
    private IEnumerator DamageHealthBar()
    {
        int nbDamage = previousHealth - Player.Instance.GetCurrentHealthPoint();

        ColorHitAllBar();

        int indexBar;
        if (!crtStart)
        {
            crtStart = true;
            indexBar = FindIndexHealthBarHit();

            int nbDamageTake = StartDecreaseHealthBar(indexBar, nbDamage);

            nbDamage -= nbDamageTake;
            previousHealth -= nbDamageTake;
        }

        while (nbDamage > 0)
        {
            if (crtFinishBar)
            {
                crtStart = true;
                indexBar = FindIndexHealthBarHit();
                int nbDamageTake = StartDecreaseHealthBar(indexBar, nbDamage);

                nbDamage -= nbDamageTake;
                previousHealth -= nbDamageTake;
            }

            yield return null;
        }

        while (!crtFinishBar)
        {
            yield return null;
        }

        previousHealth = Player.Instance.GetCurrentHealthPoint();
        ColorNormalAllBar();
    }
    private IEnumerator HealHealthBar()
    {
        int nbHeal = Player.Instance.GetCurrentHealthPoint() - previousHealth;

        ColorHealAllBar();

        int indexBar;
        if (!crtStart)
        {
            crtStart = true;
            indexBar = FindIndexHealthBarHeal();
            int nbHealTake = StartHealHealthBar(indexBar, nbHeal);

            nbHeal -= nbHealTake;
            previousHealth += nbHealTake;
        }

        while (nbHeal > 0)
        {
            if (crtFinishBar)
            {
                crtStart = true;
                indexBar = FindIndexHealthBarHeal();
                int nbHealTake = StartHealHealthBar(indexBar, nbHeal);

                nbHeal -= nbHealTake;
                previousHealth += nbHealTake;
            }

            yield return null;
        }

        while (!crtFinishBar)
        {
            yield return null;
        }

        previousHealth = Player.Instance.GetCurrentHealthPoint();
        ColorNormalAllBar();
    }
    public void UpdateHealthBar(HealthType healthType)
    {
        if (healthType == HealthType.Damage)
        {
            ColorNormalAllBar();
            StartCoroutine(DamageHealthBar());
        }
        else
        {
            ColorNormalAllBar();
            StopCoroutine(HealHealthBar());
            StartCoroutine(HealHealthBar());
        }
    }
    private int StartDecreaseHealthBar(int indexBar, int nbDamage)
    {
        int damageToInflict = Mathf.Clamp(nbDamage, 1, healtBars[indexBar].healthInBar);
        StartCoroutine(DecreaseHealthBar(indexBar, damageToInflict));
        crtFinishBar = false;
        return damageToInflict;
    }
    private int StartHealHealthBar(int indexBar, int nbHeal)
    {
        if (indexBar == -1)
        {
            return 0;
        }

        int healToInflict = Mathf.Clamp(nbHeal, 1, 10 - healtBars[indexBar].healthInBar);

        StartCoroutine(HealHealthBar(indexBar, healToInflict));
        crtFinishBar = false;

        return healToInflict;
    }
    private int FindIndexHealthBarHit()
    {
        for (int i = healtBars.Length - 1; i >= 0; i--)
        {
            if (!healtBars[i].isEmpty)
                return i;
        }

        return -1;
    }
    private int FindIndexHealthBarHeal()
    {
        for (int i = 0; i < healtBars.Length; i++)
        {
            if (!healtBars[i].isFull)
                return i;
        }

        return -1;
    }
    #endregion

    #region Restart
    public void ResetHealthBar()
    {
        for (int i = 0; i < healtBars.Length; i++)
        {
            healtBars[i].ResetBar(Player.Instance.GetMaxHealthPoint() / healtBars.Length);
        }
        previousHealth = Player.Instance.GetMaxHealthPoint();
        StopAllCoroutines();
    }
    public void Stop()
    {
        StopCoroutine(DamageHealthBar());
    }
    #endregion
}