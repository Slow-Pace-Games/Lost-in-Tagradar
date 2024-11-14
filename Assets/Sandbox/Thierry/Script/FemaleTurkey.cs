using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FemaleTurkey : MeleeEnemy, IDamageable
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    private float timerShakeHead = 0;
    private float maxTimerShakeHead = 3f;

    [Header("Le reste")]
    bool isFleeing = false;
    bool isOverRange = false;
    Vector3 destination;
    float timer = 30f;
    float timerMax = 30f;
    float fleeDistanceMultipler = 20f;
    private Vector3 vectorPlayerMonster;
    private Vector3 playerPos;
    private float timerLoseAggro = 2f;
    private float rangeDetection = 15f;

    new void Update()
    {
        if (HP > 0)
        {
            playerPos = Player.Instance.GetPosition();
            vectorPlayerMonster = playerPos - transform.position;
            if (isFleeing)
            {
                Flee();
                if (vectorPlayerMonster.magnitude > rangeDetection)
                {
                    timerLoseAggro -= TimeScale.deltaTime;
                }
                else
                {
                    destination = this.transform.position + (Player.Instance.GetForward() * fleeDistanceMultipler);
                    timer = timerMax;
                    isFleeing = true;
                }
                if (timerLoseAggro < 0)
                {
                    isOverRange = true;
                    timerLoseAggro = 5f;
                }
            }
            else
            {
                base.Update();
                if (vectorPlayerMonster.magnitude < rangeDetection)
                {
                    
                    destination = this.transform.position + (Player.Instance.GetForward() * fleeDistanceMultipler);
                    timer = timerMax;
                    isFleeing = true;
                }
            }
            if (isOverRange)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    isFleeing = false;
                    isChilling = true;
                    isOverRange = false;
                }
            }
            UpdateInvunerability();
            UpdateHeadShake();
            animator.SetFloat("Velocity", agent.velocity.magnitude);
        }
    }
    void Flee()
    {
        agent.speed = runSpeed;
        agent.SetDestination(destination);

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
                Destroy(gameObject, 1.5f);
            }
        }
    }
}
