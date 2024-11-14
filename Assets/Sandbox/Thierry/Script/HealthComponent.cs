using System.Collections;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    public int currentHealthPoint;
    public int maxHealthPoint = 100;
    private bool invincible = false;
    public bool Invincible { set => invincible = value; }

    [Header("Ref Component")]
    [SerializeField] HealthUI healthBar;
    [SerializeField] PlayerSound hit;

    public delegate void HealthChangeEvent(HealthType healthType);
    public HealthChangeEvent OnHealthChange;

    [Header("Passive Regen")]
    [SerializeField] private float passiveRegenCooldown = 10f;
    [SerializeField] private float regenTickCooldown = 5f;
    [SerializeField] private int passiveRegenValue = 3;
    private Coroutine crtRegen;
    private bool hasArmor = false;
    public bool HasArmor { set => hasArmor = value; }
    public float RegenTickCooldown { set => regenTickCooldown = value; }

    private void Start()
    {
        if (maxHealthPoint == 0)
            maxHealthPoint = 100;

        currentHealthPoint = maxHealthPoint;

        if (healthBar != null)
            OnHealthChange += healthBar.UpdateHealthBar;
    }

    public void HitPlayer(int amount)
    {
        if (!invincible)
        {
            //death
            if (currentHealthPoint <= 0)
            {
                StopAllCoroutines();
                Player.Instance.Respawn();
                return;
            }

            //hit ui
            if (hasArmor)
            {
                currentHealthPoint -= amount / 2;
            }
            else
            {
                currentHealthPoint -= amount;
            }
            healthBar.Stop();
            OnHealthChange.Invoke(HealthType.Damage);

            //passive regen
            StartPassiveRegen();

            //audio
            hit.PlayRandomSoundInPlayerHit();
        }
    }

    public void FullRegen()
    {
        currentHealthPoint = maxHealthPoint;
    }

    private IEnumerator PassiveRegen()
    {
        yield return new WaitForSeconds(passiveRegenCooldown);

        while (currentHealthPoint != maxHealthPoint)
        {
            yield return new WaitForSeconds(regenTickCooldown);

            currentHealthPoint = Mathf.Min((currentHealthPoint + passiveRegenValue), maxHealthPoint);
            OnHealthChange.Invoke(HealthType.Heal);
        }
    }
    private void StartPassiveRegen()
    {
        if (crtRegen != null)
        {
            StopCoroutine(crtRegen);
        }

        crtRegen = StartCoroutine(PassiveRegen());
    }
}