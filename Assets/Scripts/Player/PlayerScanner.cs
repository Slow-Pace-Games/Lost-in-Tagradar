using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScanner : MonoBehaviour
{
    [SerializeField] List<SORessource> scannableResources;
    [SerializeField] List<Sprite> scanButtonsSprites;
    [SerializeField] GameObject scannerParticlePrefab;
    [SerializeField] GameObject scanPingPrefab;
    [SerializeField] GameObject scannableResourceHUDPrefab;
    [SerializeField] Transform particleContainer;
    [SerializeField] Transform scannerHUD;
    [SerializeField] Transform pingContainer;
    [SerializeField] AudioSource audioScanner;

    [HideInInspector]
    private SORessource selectedResource;

    public SORessource SelectedResource { get => selectedResource; set => selectedResource = value; }

    void Start()
    {
        scannerHUD.GetChild(0).GetComponent<HUDScannerRessource>().resource = scannableResources[0]; // Basilite

        for (int i = 0; i < scannableResources.Count; i++)
        {
            if (scannableResources[i].item.IsDiscover)
            {
                AddScannableResource(i);
            }
        }

        PlayerInputManager.Instance.SelectRessourceScan(PlayerUi.Instance.OpenScanUi, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.StartScanAction(StartScanner, PlayerInputManager.ActionType.Add);
    }

    private void StartScanner()
    {
        PlayerUi.Instance.CloseScanUi();
        if (particleContainer.childCount == 0 && selectedResource != null)
        {
            Loid.Instance.UpdateTuto(PlayerAction.Scan);
            audioScanner.Play();
            GameObject go = Instantiate(scannerParticlePrefab, transform.position, Quaternion.identity, particleContainer);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            ps.Play();
        }
    }

    public void SpawnPing(Collider _other)
    {
        GameObject go = Instantiate(scanPingPrefab, pingContainer);
        go.transform.GetComponent<ScanPing>().InitPing(_other.transform, Player.Instance.transform, selectedResource.item.Sprite);
        Destroy(go, 10f);
    }

    public void AddScannableResource(int _index)
    {
        if (_index == 0) // Basilite
        {
            scannerHUD.GetChild(0).GetComponent<Image>().sprite = scanButtonsSprites[0];
            return;
        }
        GameObject go = Instantiate(scannableResourceHUDPrefab, scannerHUD);
        go.GetComponent<HUDScannerRessource>().resource = scannableResources[_index];
        go.GetComponent<Image>().sprite = scanButtonsSprites[_index];
    }
}