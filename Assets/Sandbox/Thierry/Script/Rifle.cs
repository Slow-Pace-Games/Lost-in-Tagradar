using Cinemachine;
using System.Collections;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [SerializeField] private float rangeRifle = 50f;
    [SerializeField] private int dammageRifle = 50;
    [SerializeField] private TrailRenderer trailPrefab;
    [SerializeField] private Transform SpawnBullet;
    private GameObject particleContainer;
    [SerializeField] private SOItems rifleItem;
    [SerializeField] AudioSource audioRifle;

    private CinemachineVirtualCamera cam;
    private float cooldownRifle = 0f;
    private float cooldownRifleMax = 0.2f;

    public SOItems RifleItem { get => rifleItem; }

    private void Start()
    {
        cam = Player.Instance.GetComponentInChildren<CinemachineVirtualCamera>();
        particleContainer = GameObject.Find("BulletContainer");
    }

    void Update()
    {
        if (cooldownRifle > 0f)
        {
            cooldownRifle -= TimeScale.deltaTime;
        }
    }

    private void Shoot()
    {
        if (gameObject.activeInHierarchy == true)
        {
            cooldownRifle = cooldownRifleMax;
            TrailRenderer trail = Instantiate(trailPrefab, particleContainer.transform);
            audioRifle.Play();
            Player.Instance.SetTriggerAnimator("IsUsingWeapon");
            if (Physics.Raycast(cam.State.FinalPosition, cam.transform.forward, out RaycastHit hit, rangeRifle))
            {

                if (hit.transform.gameObject.GetComponent<IDamageable>() != null)
                {
                    hit.transform.gameObject.GetComponent<IDamageable>().Hit(dammageRifle);
                }
                StartCoroutine(SpawnTrail(trail, hit.point));
            }
            else
            {
                Vector3 frontOfPlayer = cam.State.FinalPosition + cam.transform.forward * 50f;
                StartCoroutine(SpawnTrail(trail, frontOfPlayer));
            }
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hit)
    {
        float time = 0;
        Vector3 startPosition = SpawnBullet.position;

        while (time < 1f)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        Destroy(trail.gameObject);
        yield return null;
    }
    private void OnEnable()
    {
        PlayerInputManager.Instance.AttackAction(Shoot, PlayerInputManager.ActionType.Add);
    }
    private void OnDisable()
    {
        PlayerInputManager.Instance.AttackAction(Shoot, PlayerInputManager.ActionType.Remove);
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        PlayerInputManager.Instance.AttackAction(Shoot, PlayerInputManager.ActionType.Remove);
        StopAllCoroutines();
    }
}
