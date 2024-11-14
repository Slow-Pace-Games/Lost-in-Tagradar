using UnityEngine;

public class NeutralMonster : MeleeEnemy, IDamageable
{

    [Header("Attack")]
    [SerializeField] private Animator animator;
    private float timerAttack = 0f;
    private float maxTimerAttack = 3f;
    private int dammage = 5;
    private float timerAnger =0f;
    [SerializeField] private float timerMaxAnger = 15f;
    private float timerPicore = 0f;
    private float maxTimerPicore = 5f;
    private float timerDelayPicore = 0f;
    private float maxTimerDelayPicore = 5f;
    private bool isPicoring = false;
    [SerializeField] Material material;
    public float emissionIntensity = 1.0f; // Desired emission intensity
    public Color emissionColor = Color.white;

    new void Update()
    {
        if (HP > 0)
        {
            if (Spawner.UniversalAnger)
            {
                Attack();
                timerAnger += TimeScale.deltaTime;
                emissionIntensity = 100f;
                emissionColor =  Color.white;
                Color finalColor = emissionColor * emissionIntensity; // HDRP uses an exponential scale for intensity
                material.SetColor("_EmissiveColor", finalColor);
                if (timerAnger >= timerMaxAnger)
                {
                    timerAnger = 0f;
                    Spawner.UniversalAnger = false;
                    timerChangeDirection = maxTimerChangeDirection - 2f;
                }
            }
            else
            {
                emissionIntensity = 50f;
                emissionColor = Color.magenta;
                Color finalColor = emissionColor * emissionIntensity; // HDRP uses an exponential scale for intensity
                material.SetColor("_EmissiveColor", finalColor);
                if (!isPicoring)
                {
                    agent.speed = 3.5f;
                    base.Update();
                }
                UpdatePicore();
            }

            if (agent.remainingDistance < 1.5f)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
            UpdateInvunerability();

            animator.SetFloat("Velocity", agent.velocity.magnitude);
        }
    }

    public void Hit(int damage)
    {
        if (timerInvulnerability <= 0f)
        {
           HP -= damage;
            timerInvulnerability = timerMaxInvulnerability;
            Spawner.UniversalAnger = true;
            timerAnger = 0f;
            if (HP < 0)
            {
                agent.isStopped = true;
                animator.StopPlayback();
                animator.SetTrigger("Death");
                ItemManager.Instance.DropItem(mobDrop, 1, transform.position, Quaternion.identity);
                Spawner.StartSpawnCoroutine();
                Destroy(gameObject,2f);
            }
        }
    }
    private void UpdatePicore()
    {
        timerDelayPicore += TimeScale.deltaTime;
       
        if (timerDelayPicore > maxTimerDelayPicore)
        {
            agent.isStopped = true;
            animator.SetBool("Picore", true);
            timerPicore += TimeScale.deltaTime;
            isPicoring =true;
            if (timerPicore > maxTimerPicore)
            {
                isPicoring = false;
                agent.isStopped = false;
                timerDelayPicore = 0;
                timerPicore = 0;
                maxTimerPicore = Random.Range(1f, 10f);
                maxTimerDelayPicore = Random.Range(1f, 10f);
                animator.SetBool("Picore", false);
            }
        }
    }

   
    private void Attack()
    {
        Vector3 playerPos = Player.Instance.GetPosition();
        agent.SetDestination(playerPos);
        agent.speed = 8f;

        Vector3 vectorPlayerMonster = playerPos - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, vectorPlayerMonster);

        if (agent.remainingDistance < 2f && timerAttack <= 0 && Vector3.Distance(transform.position, playerPos) < 3f && dotProduct > 1f && dotProduct < 3f)
        {
            Player.Instance.HitPlayer(dammage);
            animator.SetTrigger("Attack");
            timerAttack = maxTimerAttack;
        }
        else
        {
            timerAttack -= TimeScale.deltaTime;
        }
    }

}
