using System.Collections;
using UnityEngine;

public class Taser : MonoBehaviour
{
    private BoxCollider taser;
    [SerializeField] int TaserDamage = 40;
    [SerializeField] private SOItems taserItem;
    private ParticleSystem taserParticle;

    private float cooldownTaser = 0f;
    private float cooldownTaserMax = 0.8f;
    private float timerTaserIsActive = 0f;
    private float timerMaxTaserIsActive = 0.5f;
    public SOItems TaserItem { get => taserItem; }

    [SerializeField] AudioSource audioTaser;
    private void Start()
    {
        taserParticle = GetComponentInChildren<ParticleSystem>();
        taser = GetComponent<BoxCollider>();
        taser.enabled = false;
    }

    private void Update()
    {
        if (cooldownTaser > 0f)
        {
            cooldownTaser -= TimeScale.deltaTime;
        }

        if (timerTaserIsActive > 0f)
        {
            timerTaserIsActive -= TimeScale.deltaTime;
            taser.enabled = timerTaserIsActive <= 0f ? false : true;
        }
    }

    public void Attack()
    {
        if (cooldownTaser <= 0f)
        {
            if(!audioTaser.isPlaying)
            StartCoroutine(StartDammage());
            Player.Instance.SetTriggerAnimator("IsUsingWeapon");
        }
    }
    private IEnumerator StartDammage()
    { 
        yield return new WaitForSeconds(0.35f);
        taserParticle.Play();
        audioTaser.Play();
        cooldownTaser = cooldownTaserMax;
        timerTaserIsActive = timerMaxTaserIsActive;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {
            other.GetComponent<IDamageable>().Hit(TaserDamage);
        }
    }
    private void OnEnable()
    {
        PlayerInputManager.Instance.AttackAction(Attack, PlayerInputManager.ActionType.Add);
    }
    private void OnDisable()
    {
        PlayerInputManager.Instance.AttackAction(Attack, PlayerInputManager.ActionType.Remove);
    }
}
