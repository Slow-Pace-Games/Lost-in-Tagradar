using System.Linq;
using UnityEngine;

[System.Serializable]
public class PrefabsHotBar
{
    [SerializeField] private PrefabsType prefabType;
    [SerializeField] private GameObject prefab;

    public PrefabsType PrefabType { get => prefabType; set => prefabType = value; }
    public GameObject Prefab { get => prefab; set => prefab = value; }
}

public enum PrefabsType
{
    Convey,
    Building,
}
public class PlayerBuildance : MonoBehaviour
{
    private enum ConveyBuildState
    {
        PrevisualisationStartPoint,
        PrevisualisationEndPoint,
        None,
    }

    [SerializeField] public PrefabsHotBar[] prefabsHotBar;

    [HideInInspector]
    public bool HUBPlaced = false;
    [HideInInspector]
    public bool RCPlaced = false;
    private GameObject currentObjectInstantiate;
    private int currentIndex;
    private IBuildable machineCode;
    private Convey conveyCode;
    private bool isSnapped = true;
    PrefabsType type;

    [Header("Container")]
    [SerializeField] private Transform conveyContainer;
    [SerializeField] private Transform machineContainer;

    [Header("Convey Param")]
    [SerializeField] private float maxLengthSpline = 50f;
    private ConveyBuildState conveyBuildState = ConveyBuildState.None;
    private Attachement currentAttachement;
    private Attachement.DirectionIO possibleDirection;
    private bool needAnyDirection = false;

    [Header("Player Param")]
    [SerializeField] private float rotationMultiplier;
    private Vector3 rotationPrefab = Vector3.zero;

    [Header("Other")]
    private GameObject buildingWithoutHotBar;
    private PrefabsType typeWithoutHotBar;
    private HotBarSelection hotBar;
    [HideInInspector]
    public bool onDestructionMode = false;

    private SOBuildingData conveyBuildingData;
    private SOItems basiliteRodItem;

    #region Init
    private void Start()
    {
        hotBar = GetComponent<HotBarSelection>();
        InitInput();
        InitConveyData();
    }

    private void InitInput()
    {
        PlayerInputManager.Instance.BuildAction(ClickAction, PlayerInputManager.ActionType.Add);
        PlayerInputManager.Instance.DestructionMode(ToggleDestruct, PlayerInputManager.ActionType.Add);
    }

    private void InitConveyData()
    {
        SODatabase database = Resources.Load<SODatabase>("DataBase");
        conveyBuildingData = database.AllBuildingData.Where(building => building.name.ToLower() == "convey").FirstOrDefault();
        basiliteRodItem = database.AllItems.Where(item => item.NameItem.ToLower() == "basilite rod").FirstOrDefault();
    }

    #endregion

    #region Build
    private void ClickAction()
    {
        if (currentObjectInstantiate != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Ground")))
            {
                if (machineCode != null)
                {
                    PlaceAMachine(hit.transform.tag);
                }
                else if (conveyCode != null)
                {
                    if (conveyCode.IsBuildable(conveyBuildState == ConveyBuildState.PrevisualisationStartPoint ? false : true))
                    {
                        if (conveyBuildState == ConveyBuildState.PrevisualisationStartPoint)
                        {
                            conveyBuildState = ConveyBuildState.PrevisualisationEndPoint;
                            if (currentAttachement != null)
                            {
                                currentAttachement.IsFixed = true;
                            }
                            currentAttachement = null;
                        }
                        else
                        {
                            conveyCode.SetStateBuild();
                            conveyCode = null;
                            currentObjectInstantiate = null;
                            conveyBuildState = ConveyBuildState.None;
                            if (currentAttachement != null)
                            {
                                currentAttachement.IsFixed = true;
                            }
                            currentAttachement = null;

                            Player.Instance.RemoveItem(basiliteRodItem, 1);

                            if (currentIndex != -1)
                                ChangePrefabs(currentIndex);
                            else
                                PlaceBuildingWithoutHotbar(buildingWithoutHotBar, typeWithoutHotBar);


                        }
                    }
                }
            }
        }
    }
    public void CancelBuild()
    {
        machineCode = null;

        rotationPrefab = Vector3.zero;

        conveyCode = null;
        conveyBuildState = ConveyBuildState.None;
        if (currentAttachement != null)
        {
            currentAttachement.IsFixed = true;
        }
        currentAttachement = null;

        Destroy(currentObjectInstantiate);
        currentObjectInstantiate = null;
    }
    #endregion

    #region Destruct
    private void ToggleDestruct()
    {
        onDestructionMode = !onDestructionMode;

        if (onDestructionMode) // passe en destruction
        {
            Loid.Instance.UpdateTuto(PlayerAction.Destruction);
            Player.Instance.HotBarSelection.currentState = HotBarSelection.BuildingState.Destruction;
            PlayerUi.Instance.SwapInDestructionMode();
            Player.Instance.HotBarSelection.CloseHudBuild();
            PlayerInputManager.Instance.EnableAction();
            PlayerInputManager.Instance.DisablePauseMenu();
            ElecBallsManager.Instance.ShowElecBalls();
        }
        else // passe en autre chose
        {
            PlayerInputManager.Instance.EnablePauseMenu();
            Player.Instance.HotBarSelection.currentState = HotBarSelection.BuildingState.Exploration;
            PlayerUi.Instance.SwapInExplorationMode();
            RessourcesRayManager.instance.OnDeselectInvoke();
            Player.Instance.HotBarSelection.CloseHudBuild();
            ElecBallsManager.Instance.HideElecBalls();
        }
    }
    #endregion

    #region Preview
    public void UpdateBuild()
    {
        PlacePreviewBuilding();
        Rotate();
    }
    private void Rotate()
    {
        Vector2 delta = PlayerInputManager.Instance.GetMouseScrollValue();
        rotationPrefab.y += delta.normalized.y * rotationMultiplier;
    }
    private bool RayCastMachine(Ray ray, RaycastHit hit)
    {
        if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Deposit")))
        {
            currentObjectInstantiate.SetActive(true);
            Drill drill;
            if (currentObjectInstantiate.TryGetComponent(out drill))
            {
                PreviewExtractor(hit, ref drill);
                return true;
            }
            else if (machineCode != null)
            {
                PreviewMachine(hit);
                return true;
            }
        }
        return false;
    }
    private bool RayCastConvey(Ray ray, RaycastHit hit)
    {
        if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("IO")))
        {
            if (conveyCode != null)
            {
                PreviewConvey(hit);
                return true;
            }
        }
        return false;
    }
    private void PlacePreviewBuilding()
    {
        if (currentObjectInstantiate != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (RayCastMachine(ray, hit))//if success
            {
                return;
            }

            if (RayCastConvey(ray, hit))//if success
            {
                return;
            }

            if (currentObjectInstantiate.TryGetComponent(out IBuildable machine))
                machine.ResetCollisionCounter();
            currentObjectInstantiate.SetActive(false);//player not aim the ground
        }
    }
    #endregion

    #region Hotbar
    public void ButtonHotbar(int index)
    {
        ChangePrefabs(index);
    }
    private void ChangePrefabs(int index)
    {
        if (currentObjectInstantiate != null)
        {
            Destroy(currentObjectInstantiate);
        }

        if (index == -1 || prefabsHotBar[index].Prefab == null)
        {
            currentObjectInstantiate = null;
            return;
        }

        currentIndex = index;
        GameObject go = Instantiate(prefabsHotBar[index].Prefab);
        currentObjectInstantiate = go;

        type = prefabsHotBar[index].PrefabType;
        if (type == PrefabsType.Convey)
        {
            machineCode = null;
            conveyBuildState = ConveyBuildState.PrevisualisationStartPoint;

            go.transform.SetParent(conveyContainer);
            conveyCode = go.GetComponent<Convey>();

            conveyCode.InitConveyor();
        }
        else
        {
            conveyCode = null;
            conveyBuildState = ConveyBuildState.None;

            go.transform.SetParent(machineContainer);
            machineCode = go.GetComponent<IBuildable>();
            if (machineCode == null)
            {
                machineCode = go.GetComponentInChildren<IBuildable>();
            }
            HUB hub;
            if (currentObjectInstantiate.TryGetComponent(out hub))
            {
                hub.hasAnInstance = HUBPlaced;
            }
            (machineCode as Building).SetArrowActive(true);
            if ((machineCode as IColliderIO) != null)
            {
                (machineCode as IColliderIO).DeactivateIO();
            }
        }
    }

    public string GetNameOfCurrentObjectInstantiate()
    {
        if (currentObjectInstantiate != null)
        {
            string name = currentObjectInstantiate.name;
            string[] names = name.Split('(');
            return names[0];
        }
        else
        {
            return " ";
        }
    }

    public SOBuildingData GetBuildingData()
    {
        if (currentObjectInstantiate != null && conveyCode == null)
        {
            Building building = currentObjectInstantiate.GetComponent<Building>();
            if (building == null)
            {
                building = currentObjectInstantiate.GetComponentInChildren<Building>();
            }
            //return currentObjectInstantiate.GetComponent<Building>().BuildingData;
            return building.BuildingData;
        }
        else if (currentObjectInstantiate != null && conveyCode != null)
        {
            return conveyBuildingData;
        }
        else
        {
            return null;
        }
    }


    public void ChangeBuildingInHotbar(int index, PrefabsType type, GameObject prefabsBuilding, Sprite icon)
    {
        prefabsHotBar[index].PrefabType = type;
        prefabsHotBar[index].Prefab = prefabsBuilding;
        PlayerUi.Instance.AddItemInHotBar(icon, index);
    }

    public void PlaceBuildingWithoutHotbar(GameObject prefabBuilding, PrefabsType type)
    {
        if (currentObjectInstantiate != null)
        {
            Destroy(currentObjectInstantiate);
        }
        GameObject go = Instantiate(prefabBuilding);
        currentObjectInstantiate = go;
        buildingWithoutHotBar = prefabBuilding;
        hotBar.isBuilding = true;
        typeWithoutHotBar = type;
        if (type == PrefabsType.Convey)
        {
            machineCode = null;
            conveyBuildState = ConveyBuildState.PrevisualisationStartPoint;

            go.transform.SetParent(conveyContainer);
            conveyCode = go.GetComponent<Convey>();

            conveyCode.InitConveyor();
        }
        else
        {
            conveyCode = null;
            conveyBuildState = ConveyBuildState.None;

            go.transform.SetParent(machineContainer);
            machineCode = go.GetComponentInChildren<IBuildable>();
            HUB hub;
            if (currentObjectInstantiate.TryGetComponent(out hub))
            {
                hub.hasAnInstance = HUBPlaced;
            }
            (machineCode as Building).SetArrowActive(true);
            if ((machineCode as IColliderIO) != null)
            {
                (machineCode as IColliderIO).DeactivateIO();
            }
        }
        hotBar.ActivateBuild();
        currentIndex = -1;
    }
    #endregion

    #region Machine
    private void PlaceAMachine(string tag)
    {
        if (tag == "Ground")
        {
            if (machineCode.CanBeBuilt(isSnapped))
            {
                machineCode.Build();
                ElecBallsManager.Instance.ShowElecBalls();
                if ((machineCode as IColliderIO) != null)
                {
                    (machineCode as IColliderIO).ActivateIO();
                }

                UniqueBuilding unique;
                if (currentObjectInstantiate.TryGetComponent(out unique))
                {
                    if (unique as HUB)
                    {
                        HUBPlaced = true;
                    }
                    else
                    {
                        RCPlaced = true;
                    }
                }
                machineCode = null;
                currentObjectInstantiate = null;
                hotBar.UpdateQuantityInInventory();
                if (currentIndex != -1)
                    ChangePrefabs(currentIndex);
                else
                    PlaceBuildingWithoutHotbar(buildingWithoutHotBar, typeWithoutHotBar);
            }
        }
    }

    private void PreviewMachine(RaycastHit hit)
    {
        if (hit.transform.CompareTag("Ground") && !hit.transform.CompareTag("Deposit"))
        {
            currentObjectInstantiate.transform.position = hit.point;
            machineCode.Rotate(Quaternion.Euler(rotationPrefab));
            isSnapped = true;
            machineCode.CanBeBuilt(isSnapped);
        }
    }

    private void PreviewExtractor(RaycastHit hit, ref Drill drill)
    {
        if (hit.transform.CompareTag("Deposit"))
        {
            currentObjectInstantiate.transform.position = hit.transform.position;
            drill.Rotate(Quaternion.Euler(rotationPrefab));
            isSnapped = true;
            drill.CanBeBuilt(isSnapped);
        }
        else if (hit.transform.CompareTag("Ground"))
        {
            currentObjectInstantiate.transform.position = hit.point;
            drill.Rotate(Quaternion.Euler(rotationPrefab));
            isSnapped = false;
            drill.CanBeBuilt(isSnapped);
        }
    }
    #endregion

    #region Conveyor
    private void PreviewConvey(RaycastHit hit)
    {
        if (conveyCode.GetSplineLenght(hit.point) > maxLengthSpline && conveyBuildState == ConveyBuildState.PrevisualisationEndPoint)
        {
            conveyCode.UpdateBasePos();
            return;
        }

        //if on ground
        if (hit.transform.CompareTag("Ground"))
        {
            PreviewConveyOnGround(hit);
        }
        //if find Input/output
        else if (hit.transform.CompareTag("Input") || hit.transform.CompareTag("Output"))
        {
            if (hit.transform.TryGetComponent<Attachement>(out Attachement attach))
            {
                PreviewConveyConnect(attach);
            }
        }

        conveyCode.IsBuildable(conveyBuildState == ConveyBuildState.PrevisualisationStartPoint ? false : true);
    }

    private void PreviewConveyOnGround(RaycastHit hit)
    {
        if (conveyBuildState == ConveyBuildState.PrevisualisationStartPoint)
        {
            currentObjectInstantiate.transform.position = hit.point;
            conveyCode.UpdateStartSpline(Vector3.zero, Quaternion.Euler(rotationPrefab), false, Vector3.forward);

            needAnyDirection = true;

            ResetCurrentAttachement();
        }
        else
        {
            conveyCode.UpdateEndSpline(hit.point, Quaternion.Euler(rotationPrefab), false);
            ResetCurrentAttachement();
        }
    }

    private void PreviewConveyConnect(Attachement attach)
    {
        if ((attach.Convey == conveyCode && !attach.IsFixed) || attach.Convey == null)
        {
            if (conveyCode.CheckAttachement(attach))
            {
                return;
            }

            if (conveyBuildState == ConveyBuildState.PrevisualisationStartPoint)
            {
                PreviewStartPointConnect(attach);
            }
            else if (attach.Direction == possibleDirection || needAnyDirection)
            {
                conveyCode.UpdateEndSpline(attach.TransformIO.position, attach.TransformIO.rotation, true);
            }

            currentAttachement = attach;
            attach.Convey = conveyCode;

            SetAttachementBuilding(attach);
        }
    }

    private void PreviewStartPointConnect(Attachement attach)
    {
        if (attach.Direction == Attachement.DirectionIO.Output)
        {
            PreviewStartOutputPoint(attach);
            return;
        }

        if (attach.Direction == Attachement.DirectionIO.Input)
        {
            PreviewStartInputPoint(attach);
            return;
        }
    }

    private void PreviewStartOutputPoint(Attachement attach)
    {
        possibleDirection = (attach.Direction == Attachement.DirectionIO.Input) ? Attachement.DirectionIO.Output :
                                                                   Attachement.DirectionIO.Input;

        needAnyDirection = false;

        currentObjectInstantiate.transform.position = attach.TransformIO.position;
        conveyCode.UpdateStartSpline(attach.TransformIO.position, attach.TransformIO.rotation, true, attach.TransformIO.forward);
        rotationPrefab = attach.TransformIO.rotation.eulerAngles;
    }

    private void PreviewStartInputPoint(Attachement attach)
    {
        possibleDirection = (attach.Direction == Attachement.DirectionIO.Input) ? Attachement.DirectionIO.Output :
                                                           Attachement.DirectionIO.Input;

        needAnyDirection = false;

        Quaternion newRotation = attach.TransformIO.rotation;
        newRotation.eulerAngles = new Vector3(0f, newRotation.eulerAngles.y - 180f, 0f);

        currentObjectInstantiate.transform.position = attach.TransformIO.position;
        conveyCode.UpdateStartSpline(attach.TransformIO.position, newRotation, true, -attach.TransformIO.forward, true);
        rotationPrefab = newRotation.eulerAngles;
    }

    private void SetAttachementBuilding(Attachement attach)
    {
        if (attach.IsAConveyor)
        {
            if (conveyBuildState == ConveyBuildState.PrevisualisationStartPoint)
            {
                conveyCode.SetAttachement(attach.GetComponentInParent<Convey>(), true);
            }
            else
            {
                conveyCode.SetAttachement(attach.GetComponentInParent<Convey>(), false);
            }
        }
        else
        {
            if (conveyBuildState == ConveyBuildState.PrevisualisationStartPoint)
            {
                conveyCode.SetAttachementBuiding(attach, true);
            }
            else
            {
                conveyCode.SetAttachementBuiding(attach, false);
            }
        }
    }

    private void ResetCurrentAttachement()
    {
        if (currentAttachement != null)
        {
            if (currentAttachement.IsAConveyor)
            {
                if (conveyBuildState == ConveyBuildState.PrevisualisationStartPoint)
                {
                    conveyCode.SetAttachement(null, true);
                }
                else
                {
                    conveyCode.SetAttachement(null, false);
                }
            }
            else
            {
                if (conveyBuildState == ConveyBuildState.PrevisualisationStartPoint)
                {
                    conveyCode.SetAttachementBuiding(null, true);
                }
                else
                {
                    conveyCode.SetAttachementBuiding(null, false);
                }
            }

            currentAttachement.Convey = null;
            currentAttachement = null;
        }

    }
    #endregion
}