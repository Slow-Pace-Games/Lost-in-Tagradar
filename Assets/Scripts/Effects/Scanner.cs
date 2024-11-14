using UnityEngine;

public class Scanner : MonoBehaviour
{
    private float currentTime = 0f;
    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        currentTime += TimeScale.deltaTime;
        GetComponent<SphereCollider>().radius = ps.main.startSize.constant * Player.Instance.Remap(currentTime, 0f, ps.main.startLifetime.constant, 0f, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Deposit" && other.GetComponent<RessourceExploitable>()?.sORessource == Player.Instance.GetSelectedResource())
        {
            Player.Instance.SpawnPing(other);
        }
    }

    private void OnDestroy()
    {
        Player.Instance.SetSelectedResource(null);
    }
}