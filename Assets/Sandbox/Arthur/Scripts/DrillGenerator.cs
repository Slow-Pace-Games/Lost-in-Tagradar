using System.Collections;
using UnityEngine;

public class DrillGenerator : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    GameObject machineGO;
    [SerializeField] Material Buildable;
    [SerializeField] Material NotBuildable;
    [SerializeField] Material Built;
    GameObject player;
    GameObject MachineContainer;
    IBuildable machine;
    bool constructMod = true;
    private Quaternion currentRotation = Quaternion.identity;

    IEnumerator PlaceTheMachine()
    {
        while (constructMod)
        {
            yield return null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Move the machine on the plane
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.tag == "Ground")
                {
                    machineGO.transform.position = new Vector3(hit.point.x, machineGO.transform.position.y, hit.point.z);
                    Quaternion playerRota = player.transform.rotation;
                    // Ajuste la rotation de la machine pour compenser la rotation du joueur
                    Quaternion targetRotation = Quaternion.Euler(0f, playerRota.eulerAngles.y, 0f) * Quaternion.Euler(0f, currentRotation.y, 0f);
                    machineGO.transform.rotation = targetRotation;
                }
            }

            // Check to see if the machine can be built and build it
            //if (machineGO.GetComponent<IBuildable>().CanBeBuilt())
            //{
            //    if (machineGO.GetComponentInChildren<MeshRenderer>().sharedMaterial != Buildable)
            //    {
            //        machineGO.GetComponentInChildren<MeshRenderer>().sharedMaterial = Buildable;
            //    }

            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        machine.Build();
            //        machineGO.GetComponent<Drill>().IsPlaced = true;
            //        machineGO.GetComponentInChildren<MeshRenderer>().sharedMaterial = Built;
            //        NewDrillInConstructMod(hit.point);
            //    }
            //}
            //else
            //{
            //    if (machineGO.GetComponentInChildren<MeshRenderer>().sharedMaterial != NotBuildable)
            //    {
            //        machineGO.GetComponentInChildren<MeshRenderer>().sharedMaterial = NotBuildable;
            //    }
            //}

            //Rotation of the machine
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y + 10f, currentRotation.eulerAngles.z);
                machineGO.GetComponent<IBuildable>().Rotate(currentRotation);
            }

            //Deselect the machine that is currently displayed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(machineGO);
                constructMod = false;
            }
        }
    }

    private void Start()
    {
        MachineContainer = new GameObject("MachineContainer");
        player = GameObject.Find("Player");
        machine = prefab.GetComponent<IBuildable>();
        NewDrillInConstructMod(Vector3.zero);
        StartCoroutine(PlaceTheMachine());
    }

    void NewDrillInConstructMod(Vector3 _drillPos)
    {
        Vector3 drillPos = _drillPos;
        drillPos.y = 0.0f;
        machineGO = Instantiate(prefab, drillPos, Quaternion.identity, MachineContainer.transform);
    }
}
