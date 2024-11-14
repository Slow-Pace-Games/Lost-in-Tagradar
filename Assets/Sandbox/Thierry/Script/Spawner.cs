using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] int maxMonsterSpawn;
    [SerializeField] List<GameObject> monster;
    [SerializeField] private bool canHaveALeader;
     private bool universalAnger;

    [HideInInspector]
    public bool CanHaveALeader { get => canHaveALeader; }
    [HideInInspector]
    public bool UniversalAnger { get => universalAnger; set => universalAnger = value; }

    private Transform leaderTransform;

    private void Start()    
    {

        for (int i = 0; i < maxMonsterSpawn; i++)
        {
            StartCoroutine(Spawn(0f));
        }
    }

    //il faudrait que t'ai un manager qui connait tous tes spawners
    //et une fct pour tuer tous tes mobs pour le cheat engine (autre que faire un Find je te vois venir)

    //ton update est pas conne mais on peut opti en mettant ta fonction de spawn dans une coroutine
    //et lorsque tu a plus de mob a faire spawn (que tes arrivé sur ton max) tu stop ta coroutine
    //et lorsque tu detect que un de tes mobs est mort tu relance ta coroutine comme sa plus d'update en continue sur chaque spawner donc plus optimisé
    //pour savoir quand un mob est mort tu peux le faire sur le set du nbMonsterSpawn

    //pour ta fonction de spawn soit tu garde la ref du transform de ton leader soit tu garde une ref sur tous tes mobs que ta fait spawn (le code MeleeEnemy) et avec Linq tu trouve un male via le isLeader
    //pour recup son transform tu peux le faire via la ref du code et tu le donne au mob que tu viens de faire spawn
    //ta fct ne fait spawn qu'un seul leader du coup j'ai fait un ptit fix avec ma mini opti a toi de voir si tu veux faire spwan plus de leader ou pas si oui dans ce cas là
    //le commentaire d'au dessus devrait être une piste

    //mon fix était surtout la pour enlever ta boucle for dans le spawn qui avait aucun sens

    //le public fait un peux mal au coeur mais en vrai c ok même si, si tu le met en private et que tu fait un set tu pourra mettre le truc que j'ai parlé plus haut et en plus de sa
    //il n'apparaitra pas dans l'inspecteur même si sa pourrait être interressant de le voir pour du débug (il faudrat faire un start pour le remettre a 0 et éviter des possible pb)

    //NDD : mon fix a possiblement pété ton spawn aucune idée j'ai pas vérif mais si tu le reprends avec ce que j'ai dit au dessus sa irait ptet mieux thx

    private IEnumerator Spawn(float timerRespawn)
    {
        while (timerRespawn > 0)
        {
            timerRespawn -= Time.deltaTime;
            yield return null;
        }
        Vector3 spawnPosition = transform.position + new Vector3(Random.Range(0, 11), 0, Random.Range(0, 11));
        int rand = Random.Range(0, monster.Count);
        GameObject newMonster = Instantiate(monster[rand], spawnPosition, Quaternion.identity, transform);
        newMonster.GetComponent<MeleeEnemy>().Spawner = this;
        if (canHaveALeader)
        {
            if (leaderTransform == null)
            {
                newMonster.GetComponent<MeleeEnemy>().SetLeader(true);
                leaderTransform = newMonster.transform;
            }
            else
            {
                newMonster.GetComponent<MeleeEnemy>().SetLeaderTransform(leaderTransform);
            }
        }
       yield return null;
    }
    public void StartSpawnCoroutine()
    {
        StartCoroutine(Spawn(120f));

    }
}