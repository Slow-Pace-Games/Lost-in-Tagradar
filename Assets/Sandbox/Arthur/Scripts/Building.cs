using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Building : MonoBehaviour, IBuildable, IDestructible
{
    [Header("Render")]
    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected Material[] materialPreview = new Material[3];
    [SerializeField] protected VisualEffect effect;
    [SerializeField] protected ParticleSystem particle;
    protected VisualEffect instantiatedEffect;
    protected bool isEffectRunning = false;
    protected float effectMaxTimer = 1.8f;
    protected float effectTimer = 0;

    [Header("Data")]
    [SerializeField] protected SOBuildingData buildingData;
    [SerializeField] protected bool isAlimented = false;
    [SerializeField] protected SODatabase database;

    [Header("Debug")]
    [SerializeField] protected bool isPlaced = false;
    protected int collisionCounter = 0;

    [Header("Arrow")]
    [SerializeField] public Transform[] arrows;

    [Header("Sound")]
    [SerializeField] protected AudioSource audioSource1;
    [SerializeField] protected AudioSource audioSource2;

    [Header("Collisions")]
    [SerializeField] BoxCollider bCollider;


    //Accessors
    public bool IsPlaced { get => isPlaced; set => isPlaced = value; }
    public bool IsAlimented { get => isAlimented; set => isAlimented = value; }
    public SOBuildingData BuildingData { get => buildingData; }

    protected virtual void ActivateMesh(bool activate)
    {
        meshRenderer.enabled = activate;
    }

    public void SetArrowActive(bool activate)
    {
        foreach (Transform arrow in arrows)
        {
            arrow.gameObject.SetActive(activate);
        }
    }

    #region IBuildable

    public virtual void LoadBuild(List<int> outputIDs = null, List<int> inputIDs = null, List<int> outputStacks = null, List<int> inputStacks = null)
    {
        gameObject.GetComponent<NavMeshObstacle>().enabled = true;

        meshRenderer.material = materialPreview[2];
        IsPlaced = true;

        instantiatedEffect = Instantiate(effect, transform.position, transform.rotation);
        instantiatedEffect.Play();

        ActivateMesh(false);

        SetArrowActive(false);
    }

    public virtual void Build()
    {
        gameObject.GetComponent<NavMeshObstacle>().enabled = true;

        meshRenderer.material = materialPreview[2];
        IsPlaced = true;

        for (int i = 0; i < buildingData.costs.Count; i++)
        {
            Player.Instance.RemoveItem(buildingData.costs[i].item, buildingData.costs[i].value);
        }

        instantiatedEffect = Instantiate(effect, transform.position, transform.rotation);
        instantiatedEffect.Play();

        ActivateMesh(false);
        SetArrowActive(false);
    }



    public virtual bool CanBeBuilt(bool isSnapped)
    {
        for (int i = 0; i < buildingData.costs.Count; i++)
        {
            if (Player.Instance.GetItemAmount(buildingData.costs[i].item) < buildingData.costs[i].value) //Check in inventory if the component are there in the adequate quantity
            {
                meshRenderer.material = materialPreview[1];
                return false;
            }
        }

        if (collisionCounter == 0 && isSnapped)
        {
            meshRenderer.material = materialPreview[0];
            return true;
        }

        meshRenderer.material = materialPreview[1];
        return false;
    }

    public virtual void ResetCollisionCounter()
    {
        collisionCounter = 0;
    }

    public void Rotate(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    #endregion

    #region IDestructible
    public virtual void Destruct()
    {
        isPlaced = false;
        for (int i = 0; i < buildingData.costs.Count; i++)
        {
            Player.Instance.AddItem(buildingData.costs[i].item, buildingData.costs[i].value);
        }
        Destroy(gameObject);
    }
    #endregion

    #region Collision
    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<HealthComponent>(out HealthComponent garbage))
        {
            bCollider.isTrigger = true;
        }
        collisionCounter++;
    }
    public virtual void OnCollisionExit(Collision collision)
    {
        collisionCounter--;
    }

    public virtual void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Deposit"))
        {
            collisionCounter--;
        }

        HealthComponent garbage;
        if (collider.gameObject.TryGetComponent<HealthComponent>(out garbage))
        {
            bCollider.isTrigger = false;
            collisionCounter--;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Deposit"))
        {
            collisionCounter++;
        }

        HealthComponent garbage;
        if (collider.gameObject.TryGetComponent<HealthComponent>(out garbage))
        {
            collisionCounter++;
        }
    }
    #endregion

	#region Audio
    protected void PlaySound(AudioSource source, AudioClip clip, bool repeat)
    {
        if (source != null)
        {
            source.clip = clip;
            source.loop = repeat;
            source.Play();
        }
        else
        {
            Debug.LogWarning("Audio source machine null");
        }
    }
    protected void PlaySound(AudioSource source, AudioClip clip, int nbReapeat)
    {
        if (source != null)
        {
            source.clip = clip;
            StartCoroutine(RepeatSounds(source, nbReapeat));
        }
        else
        {
            Debug.LogWarning("Audio source machine null");
        }
    }

    protected void PlaySoundPuller(AudioSource source, List<AudioClip> puller, int nbReapeat) => StartCoroutine(PullerSound(source, puller, nbReapeat));
    protected void StopSound(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            source.clip = null;
            source.loop = false;
        }
    }
    private IEnumerator RepeatSounds(AudioSource source, int nbReapeat)
    {
        float clipLenght = source.clip.length;
        source.loop = true;
        source.Play();

        while (nbReapeat > 0)
        {
            yield return new WaitForSeconds(clipLenght);
            nbReapeat--;
        }

        source.Stop();
        source.clip = null;
        source.loop = false;
    }
    private IEnumerator PullerSound(AudioSource source, List<AudioClip> puller, int nbReapeat)
    {
        while (true)
        {
            source.clip = puller[UnityEngine.Random.Range(0, puller.Count)];
            yield return StartCoroutine(RepeatSounds(source, nbReapeat));
        }
    }
	#endregion
}