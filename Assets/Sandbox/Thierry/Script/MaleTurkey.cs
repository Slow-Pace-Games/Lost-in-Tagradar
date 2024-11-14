using UnityEngine;

public class MaleTurkey : MeleeEnemy, IDamageable
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    private float timerShakeHead = 0;
    private float maxTimerShakeHead = 3f;

    [Header("Player Ref")]
    private Vector3 playerPos;
    private Vector3 vectorPlayerMonster;
    [SerializeField] private float rangeDetection = 15f;

    [Header("Attack")]
    private bool isAttacking = false;
    private float timerAttack = 0f;
    private float maxTimerAttack = 5f;
    private int dammage = 10;
    private float timerLoseAggro = 5f;

    [Header("Audio")]
    [SerializeField] AudioSource attack;

    protected override void Update()
    {
        
        if (HP > 0)
        {
            playerPos = Player.Instance.GetPosition();
            vectorPlayerMonster = playerPos - transform.position;
            if (isAttacking)
            {
                Attack();
                if (vectorPlayerMonster.magnitude > rangeDetection)
                {
                    timerLoseAggro -= TimeScale.deltaTime;
                }
                if (timerLoseAggro < 0)
                {
                    isAttacking = false;
                    timerLoseAggro = 5f;
                }
            }
            else
            {
                base.Update();
                if (vectorPlayerMonster.magnitude < rangeDetection)
                {
                    isAttacking = true;
                }
            }


            if (agent.remainingDistance < 2f)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }

            UpdateInvunerability();
            UpdateHeadShake();
            animator.SetFloat("Velocity", agent.velocity.magnitude);
        }
    }

    private void Attack()
    {
        agent.speed = runSpeed;
        agent.SetDestination(playerPos);

        float dotProduct = Vector3.Dot(transform.forward, vectorPlayerMonster);

        if (agent.remainingDistance < 2f && timerAttack <= 0 && Vector3.Distance(transform.position, playerPos) < 3f && dotProduct > 1f && dotProduct < 3f )
        {
            attack.Play();
            animator.SetTrigger("Attack");
            timerAttack = maxTimerAttack;
        }
        else
        {
            timerAttack -= TimeScale.deltaTime;
        }
    }
    public void SetDamage()
    {
        float dotProduct = Vector3.Dot(transform.forward, vectorPlayerMonster);
        if (Vector3.Distance(transform.position, playerPos) < 3f && dotProduct > 1f && dotProduct < 3f)
        {
            Player.Instance.HitPlayer(dammage);
        }
    }

    private void UpdateHeadShake()
    {
        timerShakeHead += TimeScale.deltaTime;
        if (timerShakeHead > maxTimerShakeHead)
        {
            timerShakeHead = 0f;
            maxTimerShakeHead = Random.Range(1f, 5f);
            animator.SetBool("Shake", true);
        }
        else
        {
            animator.SetBool("Shake", false);
        }
    }


    public void Hit(int damage)
    {

        if (timerInvulnerability <= 0f)
        {
            HP -= damage;
            timerInvulnerability = timerMaxInvulnerability;

            if (HP <= 0)
            {
                agent.isStopped = true;
                animator.StopPlayback();
                animator.Play("Death");
                ItemManager.Instance.DropItem(mobDrop, 1, transform.position, Quaternion.identity);
                Spawner.StartSpawnCoroutine();
                Destroy(gameObject,1.5f);
            }
        }
    }

}