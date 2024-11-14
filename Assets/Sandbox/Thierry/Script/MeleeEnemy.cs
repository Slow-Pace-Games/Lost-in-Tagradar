using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Timer action")]
    protected float timerChangeDirection;
    protected float maxTimerChangeDirection;
    private float timerChill;
    private float maxTimerChill;    
    private float timerGoChill;
    private float maxTimerGoChill;
    protected float timerMaxInvulnerability;
    protected float timerInvulnerability = 0f;

    [Header("Navigaton")]
    protected NavMeshAgent agent;
    private Vector3 nextDirection;
    private Vector3 destinationGap;
    protected float walkSpeed = 2.5f;
    protected float runSpeed = 10f;

    [Header("Mob Param")]
    private Transform leader;
    private float maxDistance = 100f;
    public bool isChilling = false;
    private bool isLeader = false;
    [SerializeField] protected SOItems mobDrop;


    [Header("Comonent ref")]
    private Spawner spawner;
    public Spawner Spawner { set => spawner = value; get => spawner; }
    private AudioSource hit;
    protected int HP;
    protected int MaxHp = 100;

    private void Start()
    {
        maxTimerChangeDirection = Random.Range(15f, 30f);
        timerChangeDirection = maxTimerChangeDirection;
        maxTimerChill = 5f;
        timerChill = maxTimerChill;
        maxTimerGoChill = Random.Range(15f, 30f);
        timerGoChill = maxTimerGoChill;
        timerMaxInvulnerability = 0.5f;

        nextDirection = Vector3.zero;
        agent = GetComponent<NavMeshAgent>();
        destinationGap = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
        spawner = GetComponentInParent<Spawner>();
        hit = GetComponent<AudioSource>();
        HP = MaxHp;

        float random = Random.Range(-0.15f, 0.15f);
        transform.localScale = new Vector3(transform.localScale.x + random, transform.localScale.y + random, transform.localScale.z + random);
    }

    protected virtual void Update()
    {
        GetALeader();
        UpdateMobPosition();
    }

    private void GetALeader()
    {
        //malo a rien compris xDD
        //hum hum, comment dire que sa non, si jamais un des mobs spawn en premier sans avoir déjà fait de leader fait le wander un peux paummer qui réagit pas ou un truc du genre et soit avec le linq (voir ndd spawner)
        //soit avec une fonction du style AskForALeader sur le spawner qui te renvoie un leader ce sera mieux qu'un foreach pas opti du cul
        //avec le linq cette fonction pourrait s'enlever instant
        //et si tu veux faire spawn un leader en premier tu peux le faire direct dans le spawner plutôt que de redéfinir un leader dans le mob ce qui est très étrange
        //et avec plusieurs leader sa peux faire plusieurs groupe et donc un peux plus vivant
        if (spawner.CanHaveALeader)
        {
            if (leader == null)
            {
                MeleeEnemy[] monster = GetComponentInParent<Spawner>().GetComponentsInChildren<MeleeEnemy>();
                bool leaderIsFind = false;
                foreach (MeleeEnemy m in monster)
                {
                    if (m.isLeader)
                    {
                        SetLeaderTransform(m.transform);
                        leaderIsFind = true;
                    }
                }

                if (!leaderIsFind)
                {
                    isLeader = true;
                }
            }
        }
    }
    private void UpdateMobPosition()
    {
        if (isLeader)
        {
            if (!isChilling)
            {
                ChooseADirection();

                timerGoChill += TimeScale.deltaTime;
                if (timerGoChill >= maxTimerGoChill)
                {
                    timerGoChill = 0;
                    maxTimerGoChill = Random.Range(15f, 30f);
                    isChilling = true;
                }
            }
            else
            {
                Chill();
            }
        }
        else
        {
            if (spawner.CanHaveALeader)
            {
                agent.SetDestination(leader.position + destinationGap);
                agent.speed = walkSpeed;
            }
            else
            {
                ChooseADirection();
            }
        }
    }

    private void ChooseADirection()
    {
        if (timerChangeDirection >= maxTimerChangeDirection)
        {
            timerChangeDirection = 0f;
            maxTimerChangeDirection = Random.Range(15f, 30f);

            nextDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            nextDirection *= Random.Range(50f,75f);
            nextDirection += transform.position;
            agent.speed = walkSpeed;
            agent.SetDestination(nextDirection);

        }
        else
        {
            timerChangeDirection += Time.deltaTime;
        }

        if (Vector3.Distance(spawner.transform.position, transform.position) > maxDistance)
        {
            agent.SetDestination(spawner.transform.position);
        }
    }

    private void Chill()
    {
        if (timerChill > 0)
        {
            if (agent.remainingDistance > 0.5)
            {
                agent.SetDestination(transform.position);
            }

            timerChill -= TimeScale.deltaTime;
        }
        else
        {
            timerGoChill = 0f;
            maxTimerChill = Random.Range(1f,5f);
            timerChill = maxTimerChill;
            isChilling = false;
            timerChangeDirection = maxTimerChangeDirection;
        }
    }

    public void SetLeader(bool _isleader)
    {
        isLeader = _isleader;
    }
    public void SetLeaderTransform(Transform _transform)
    {
        leader = _transform;
    }



    protected void UpdateInvunerability()
    {
        if (timerInvulnerability > 0)
        {
            timerInvulnerability -= TimeScale.deltaTime;
        }
    }


}