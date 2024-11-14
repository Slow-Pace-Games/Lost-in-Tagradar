using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FleeAndPasiveMonster : MeleeEnemy, IDamageable
{
    bool isFleeing = false;
    float fleeDistanceMultipler = 50f;
    Vector3 destination;
    float timer = 15f;
    new void Update()
    {
        if (isFleeing)
        {
            Flee();
        }
        else
        {
            base.Update();
        }
        UpdateInvunerability();
    }
    public void Hit(int damage)
    {
        if (timerInvulnerability <= 0f)
        {
            HP -= damage;
            timerInvulnerability = timerMaxInvulnerability;
            isFleeing = true;
            if (HP < 0)
            {
                ItemManager.Instance.DropItem(mobDrop, 1, transform.position, Quaternion.identity);
                Spawner.StartSpawnCoroutine();
                Destroy(gameObject);
            }
        }
    }

    void Flee()
    {
        destination = transform.position + Player.Instance.transform.forward * fleeDistanceMultipler;
        agent.SetDestination(destination);
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            isFleeing = false;
            isChilling = true;
        }

    }
}
