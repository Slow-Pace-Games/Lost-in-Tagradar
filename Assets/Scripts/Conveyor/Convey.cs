using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Convey : MonoBehaviour
{
    #region Class
    [System.Serializable]
    private class SplineRender
    {
        [Range(0.1f, 2f)] public float extrusionInterval;

        public MeshFilter baize;
        public MeshRenderer baizeRD;

        [Space]
        public MeshFilter border;
        public MeshRenderer borderRD;
    }
    [System.Serializable]
    private class ConveyMaterial
    {
        public Material[] buildable = new Material[2];
        public Material baize;
        public Material border;
        public Material baseConvey;

        public enum BuildMaterial
        {
            Buildable,
            NotBuildable,
        }
    }
    #endregion

    #region Enum
    public enum State
    {
        Previsualisation,
        Build,
    }
    private enum Base
    {
        Start,
        End,
    }
    #endregion

    [Header("Ref component")]
    [SerializeField] private SplineItem splineItem;
    [SerializeField] private SplineMeshCollider splineMeshCollider;
    [SerializeField] private AnimateConvey animateConvey;

    [Header("Spline Render")]
    [SerializeField] private SplineRender splineRender;
    [SerializeField] private ConveyMaterial conveyMaterial;

    [Header("Spline Parameter")]
    [SerializeField] private State state = State.Previsualisation;
    [SerializeField] private bool needToBeReversed = false;
    [SerializeField] private bool isAttach = false;
    private float yOffsetPosition = 1f;
    private Spline spline;

    [Header("Base")]
    [SerializeField] private Transform containerBase;
    [SerializeField] private GameObject prefabBase;
    [SerializeField, Range(0.0f, 3.0f)] private float offsetPositionBase;
    [Header("   Start")]
    [SerializeField] private GameObject baseStart;
    [SerializeField] private Attachement attachementStart;
    [Header("   End")]
    [SerializeField] private GameObject baseEnd;
    [SerializeField] private Attachement attachementEnd;

    [Header("Attachement")]
    [SerializeField] private Attachement buildingAtStart;
    [SerializeField] private Attachement buildingAtEnd;

    #region Spline Shape
    public void SetStateBuild()
    {
        state = State.Build;

        UpdateBasePos();
        if (needToBeReversed)
        {
            SplineUtilities.ReverseFlowSplineWithoutTangent(spline);
            SplineMesh.BuildMesh(spline, splineRender.extrusionInterval, splineRender.baize, splineRender.border);
        }

        Loid.Instance.UpdateTuto(PlayerAction.PlaceConvey);
        CanMergeConvey();
        SetDefMaterials();
        animateConvey.SetBuild();
        splineMeshCollider.SetSplineBuild();
        splineItem.UpdateStep();
        ActivateCollisionBase();
    }
    public void InitConveyor()
    {
        spline = GetComponent<SplineContainer>().Spline;//prend la ref a la spline
        animateConvey.Init();

        Vector3 position = new Vector3(0f, yOffsetPosition, 0f);

        SplineUtilities.AddSplinePoint(spline, position, new Vector3(0f, 0f, 2f));
        SplineUtilities.AddSplinePoint(spline, position, new Vector3(0f, 0f, 2f));

        ShowBase(position, Quaternion.identity, Base.Start);
        ShowBase(position, Quaternion.identity, Base.End);
    }
    public void UpdateStartSpline(Vector3 position, Quaternion rotation, bool isAttach, Vector3 forwardVector, bool inversedFlow = false)
    {
        needToBeReversed = inversedFlow;
        this.isAttach = isAttach;

        Vector3 tangent = SplineUtilities.GetTangentAtSplinePoint(spline, 0);
        Vector3 forward = Vector3.Normalize(tangent);
        Vector3 newTangent = Matrix4x4.Rotate(rotation).MultiplyVector(Vector3.forward);

        SplineUtilities.SetTangentsAtSplinePoint(spline, 0, newTangent);
        SplineUtilities.SetTangentsAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline), newTangent);

        if (isAttach)
        {
            Vector3 positionAhead = forwardVector * 2f;

            SplineUtilities.SetPositionAtSplinePoint(spline, 0, Vector3.zero);
            SplineUtilities.SetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline), positionAhead);

            HideBase(Base.Start);
            ShowBase(SplineUtilities.GetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline)), rotation, Base.End);
        }
        else
        {
            Vector3 positionYOffset = position + new Vector3(0f, yOffsetPosition, 0f);
            Vector3 positionAheadYOffset = positionYOffset + forward * 2f;

            SplineUtilities.SetPositionAtSplinePoint(spline, 0, positionYOffset);
            SplineUtilities.SetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline), positionAheadYOffset);

            ShowBase(SplineUtilities.GetPositionAtSplinePoint(spline, 0), rotation, Base.Start);
            ShowBase(SplineUtilities.GetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline)), rotation, Base.End);
        }

        SplineMesh.BuildMesh(spline, splineRender.extrusionInterval, splineRender.baize, splineRender.border);
        splineMeshCollider.UpdateColliders(splineRender.baize.sharedMesh);
    }
    public void UpdateEndSpline(Vector3 position, Quaternion rotation, bool isAttach)
    {
        this.isAttach = isAttach;

        if (needToBeReversed && isAttach)
        {
            rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y - 180f, 0f);
        }

        Vector3 newTangent = Matrix4x4.Rotate(rotation).MultiplyVector(Vector3.forward);
        Vector3 newEndPoint;

        if (isAttach)
        {
            newEndPoint = position - transform.position;
            HideBase(Base.End);
        }
        else
        {
            newEndPoint = position - transform.position + new Vector3(0f, yOffsetPosition, 0f);
            ShowBase(SplineUtilities.GetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline)), rotation, Base.End);
        }

        if (!SplineUtilities.IsSamePointAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline), newEndPoint, newTangent))
        {
            SplineUtilities.SetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline), newEndPoint);
            SplineUtilities.SetTangentsAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline), newTangent);

            SplineShape.UpdateSplineShape(spline);

            SplineMesh.BuildMesh(spline, splineRender.extrusionInterval, splineRender.baize, splineRender.border);
            splineMeshCollider.UpdateColliders(splineRender.baize.sharedMesh, true);
        }
    }
    #endregion

    #region Spline Buildable
    public bool IsBuildable(bool isEnd)
    {
        if (animateConvey.IsCollide && !isAttach)
        {
            UpdateMaterials(ConveyMaterial.BuildMaterial.NotBuildable);
            return false;
        }

        if (isEnd)
        {
            if (spline.GetLength() < 7f)
            {
                UpdateMaterials(ConveyMaterial.BuildMaterial.NotBuildable);
                return false;
            }

            if (IsAngleToSharp())
            {
                UpdateMaterials(ConveyMaterial.BuildMaterial.NotBuildable);
                return false;
            }
        }

        UpdateMaterials(ConveyMaterial.BuildMaterial.Buildable);
        return true;
    }
    private void UpdateMaterials(ConveyMaterial.BuildMaterial isBuildable)
    {
        Material material = conveyMaterial.buildable[(int)isBuildable];

        splineRender.baizeRD.material = material;
        splineRender.borderRD.material = material;

        UpdateMaterialsBases(material);
    }
    private void SetDefMaterials()
    {
        splineRender.baizeRD.material = conveyMaterial.baize;
        splineRender.borderRD.material = conveyMaterial.border;

        UpdateMaterialsBases(conveyMaterial.baseConvey);
    }
    private bool IsAngleToSharp()
    {
        float step = 0.05f;
        float delta = step;

        SplineUtility.Evaluate(spline, 0f, out float3 positionA, out float3 tangentA, out float3 upVectorA);
        Vector3 forwardA = ((Vector3)tangentA).normalized;

        while (delta < 1f)
        {
            SplineUtility.Evaluate(spline, delta, out float3 positionB, out float3 tangentB, out float3 upVectorB);
            Vector3 forwardB = ((Vector3)tangentB).normalized;

            if (Vector3.Angle(forwardA, forwardB) > 35f)
            {
                return true;
            }

            forwardA = forwardB;
            delta += step;

            if (delta > 1 && delta != 1f + step)
            {
                delta = 1f;
            }
        }

        return false;
    }
    public float GetSplineLenght(Vector3 position)
    {
        return Mathf.Abs(Vector3.Distance(SplineUtilities.GetWorldPositionAtSplinePoint(spline, transform.position, 0), position));
    }
    #endregion

    #region Spline Item
    public bool CanAddItemOnConvey()
    {
        if (state == State.Previsualisation)
        {
            return false;
        }

        return splineItem.CanAddItemOnConvey();
    }
    public void AddItemOnConvey(SOItems item)
    {
        splineItem.AddItemOnConvey(item);
    }
    public SOItems GetItemOnConvey()
    {
        return splineItem.GetItemOnConvey();
    }
    public bool IsSameItemOnConvey(SOItems item)
    {
        return splineItem.IsSameItemOnConvey(item);
    }
    public bool CanGetItemOnConvey()
    {
        if (state == State.Previsualisation)
        {
            return false;
        }

        return splineItem.CanGetItemOnConvey();
    }
    #endregion

    #region Spline Merger
    public void CanMergeConvey()
    {
        SplineIsReversed();

        if (TryMergeDoubleConvey())
        {
            return;
        }

        if (CanMergeConveyAtEnd())
        {
            return;
        }

        if (CanMergeConveyAtBegining())
        {
            return;
        }
    }
    public void SetAttachement(Convey convey, bool isAtBegin)
    {
        if (isAtBegin)
        {
            attachementStart.Convey = convey;
        }
        else
        {
            attachementEnd.Convey = convey;
        }
    }
    public void SetAttachementBuiding(Attachement attach, bool isAtBegin)
    {
        if (isAtBegin)
        {
            buildingAtStart = attach;
        }
        else
        {
            buildingAtEnd = attach;
        }
    }
    private bool CanMergeConveyAtEnd()
    {
        //if someone plug at the end of the spline
        if (attachementEnd.Convey != null)
        {
            if (attachementEnd.Convey == this)
            {
                attachementEnd.Convey = null;
                return false;
            }
            else if (attachementEnd.Convey.state != State.Previsualisation)
            {
                if (attachementEnd.Convey.buildingAtEnd != null)
                {
                    attachementEnd.Convey.buildingAtEnd.Convey = this;
                    buildingAtEnd = attachementEnd.Convey.buildingAtEnd;
                }

                MergeConveyEnd(attachementEnd.Convey);
                return true;
            }
        }
        return false;
    }
    private bool CanMergeConveyAtBegining()
    {
        //if someone plug at the begining of the spline
        if (attachementStart.Convey != null)
        {
            if (attachementStart.Convey == this)
            {
                attachementStart.Convey = null;
                return false;
            }
            else if (attachementStart.Convey.state != State.Previsualisation)
            {
                if (attachementStart.Convey.buildingAtStart != null)
                {
                    attachementStart.Convey.buildingAtStart.Convey = this;
                    buildingAtStart = attachementStart.Convey.buildingAtStart;
                }

                MergeConveyStart(attachementStart.Convey);
                return true;
            }
        }
        return false;
    }
    private bool TryMergeDoubleConvey()
    {
        if (attachementStart.Convey != null && attachementEnd.Convey != null)
        {
            bool isMe = false;
            if (attachementStart.Convey == this)
            {
                attachementStart.Convey = null;
                isMe = true;
            }

            if (attachementEnd.Convey == this)
            {
                attachementEnd.Convey = null;
                isMe = true;
            }

            if (!isMe)
            {
                DoubleMergerConvey(attachementStart.Convey, attachementEnd.Convey);
                return true;
            }
        }
        return false;
    }
    private void MergeConveyStart(Convey attachConvey)
    {
        //merge spline
        spline.Knots = SplineUtilities.Merge2SplineAtStart(spline.Knots.ToList(), attachConvey.spline.Knots.ToList(), transform.position, attachConvey.transform.position);
        SplineMesh.BuildMesh(spline, splineRender.extrusionInterval, splineRender.baize, splineRender.border);

        //merge base
        int nbBase = attachConvey.containerBase.childCount;
        for (int i = 0; i < nbBase; i++)
        {
            GameObject newBase = attachConvey.containerBase.GetChild(0).gameObject;
            newBase.transform.SetParent(containerBase);//change parent
            newBase.transform.SetSiblingIndex(i);//change parent
            newBase.name = "Base " + i.ToString();
            newBase.GetComponent<Attachement>().DesactivateCollider();//desactivate collider merger

            if (i == 0)
            {
                //destroy the old version
                Destroy(baseStart);

                //add the new version
                baseStart = newBase;
                baseStart.name = "BaseStart";
                baseStart.transform.SetAsFirstSibling();
            }
        }

        MergeItem(attachConvey);

        //merge code attachement
        attachementStart = baseStart.GetComponent<Attachement>();
        attachementStart.ReactivateCollider();

        buildingAtStart = attachConvey.buildingAtStart;
        if (buildingAtStart != null)
        {
            buildingAtStart.Convey = this;
        }

        //destroy old spline
        Destroy(attachConvey.gameObject);
    }
    private void MergeConveyEnd(Convey attachConvey)
    {
        spline.Knots = SplineUtilities.Merge2SplineAtEnd(spline.Knots.ToList(), attachConvey.spline.Knots.ToList(), transform.position, attachConvey.transform.position);
        SplineMesh.BuildMesh(spline, splineRender.extrusionInterval, splineRender.baize, splineRender.border);

        //merge base
        int nbBase = attachConvey.containerBase.childCount;
        for (int i = 0; i < nbBase; i++)
        {
            GameObject newBase = attachConvey.containerBase.GetChild(0).gameObject;
            newBase.transform.SetParent(containerBase);//change parent
            newBase.transform.SetSiblingIndex(i);//change parent
            newBase.name = "Base " + i.ToString();
            newBase.GetComponent<Attachement>().DesactivateCollider();//desactivate collider merger

            if (i == nbBase - 1)
            {
                Destroy(baseEnd);//destroy the old version

                baseEnd = newBase;//add the new version
                baseEnd.name = "BaseEnd";
                baseEnd.transform.SetAsLastSibling();
                baseStart.transform.SetAsFirstSibling();
            }
        }

        MergeItem(attachConvey);

        //merge code attachement
        attachementEnd = baseEnd.GetComponent<Attachement>();
        attachementEnd.ReactivateCollider();

        buildingAtEnd = attachConvey.buildingAtEnd;
        if (buildingAtEnd != null)
        {
            buildingAtEnd.Convey = this;
        }

        //destroy old spline
        Destroy(attachConvey.gameObject);
    }
    private void MergeItem(Convey attachConvey)
    {
        attachConvey.splineItem.ClearItems();
    }
    private void DoubleMergerConvey(Convey startAttachConvey, Convey endAttachConvey)
    {
        MergeConveyStart(startAttachConvey);
        MergeConveyEnd(endAttachConvey);
    }
    private void SplineIsReversed()
    {
        if (needToBeReversed)
        {
            if (attachementStart.IsAConveyor || attachementEnd.IsAConveyor)
            {
                Convey refTemp = attachementStart.Convey;
                attachementStart.Convey = attachementEnd.Convey;
                attachementEnd.Convey = refTemp;

                InversedBase();
            }
        }
    }
    #endregion

    #region Spline Base
    private void ShowBase(Vector3 position, Quaternion rotation, Base basePos)
    {
        if (basePos == Base.Start)
        {
            baseStart.transform.localPosition = position;
            baseStart.transform.localRotation = rotation;
            if (!baseStart.activeSelf)
            {
                baseStart.SetActive(true);
                return;
            }
        }
        else
        {
            baseEnd.transform.localPosition = position;
            baseEnd.transform.localRotation = rotation;
            if (!baseEnd.activeSelf)
            {
                baseEnd.SetActive(true);
                return;
            }
        }
    }
    private void HideBase(Base basePos)
    {
        if (basePos == Base.Start)
        {
            if (baseStart.activeSelf)
            {
                baseStart.SetActive(false);
            }
            return;
        }

        if (baseEnd.activeSelf)
        {
            baseEnd.SetActive(false);
        }
    }
    //return true if the attachment is mine
    public bool CheckAttachement(Attachement attach)
    {
        if (attach == attachementStart || attach == attachementEnd)
        {
            return true;
        }
        return false;
    }
    private void ActivateCollisionBase()
    {
        if (baseStart.activeSelf == true)
        {
            baseStart.GetComponent<Collider>().enabled = true;
        }
        if (baseEnd.activeSelf == true)
        {
            baseEnd.GetComponent<Collider>().enabled = true;
        }
    }
    private void InversedBase()
    {
        Transform refTemp = baseEnd.transform;

        baseStart.transform.position = baseEnd.transform.position;
        baseStart.transform.rotation = baseEnd.transform.rotation;

        baseEnd.transform.position = refTemp.position;
        baseEnd.transform.rotation = refTemp.rotation;

        bool refActiveTemp = baseStart.activeSelf;
        baseStart.SetActive(baseEnd.activeSelf);
        baseEnd.SetActive(refActiveTemp);

        bool refActiveColliderTemp = baseStart.GetComponent<BoxCollider>();
        baseStart.GetComponent<BoxCollider>().enabled = baseEnd.GetComponent<BoxCollider>().enabled;
        baseEnd.GetComponent<BoxCollider>().enabled = refActiveColliderTemp;

        Attachement temp = buildingAtStart;
        buildingAtStart = buildingAtEnd;
        buildingAtEnd = temp;

        InversedForward();
    }
    private void InversedForward()
    {
        Quaternion newRotation = attachementEnd.TransformIO.rotation;
        newRotation.eulerAngles = new Vector3(0f, newRotation.eulerAngles.y - 180f, 0f);

        attachementEnd.TransformIO.rotation = newRotation;

        newRotation = attachementStart.TransformIO.rotation;
        newRotation.eulerAngles = new Vector3(0f, newRotation.eulerAngles.y - 180f, 0f);

        attachementStart.TransformIO.rotation = newRotation;
    }
    private void UpdateMaterialsBases(Material material)
    {
        baseStart.GetComponentInChildren<MeshRenderer>().material = material;
        baseEnd.GetComponentInChildren<MeshRenderer>().material = material;
    }
    public void UpdateBasePos()
    {
        baseStart.transform.localPosition = SplineUtilities.GetPositionAtSplinePoint(spline, 0);
        baseEnd.transform.localPosition = SplineUtilities.GetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline));
    }
    #endregion

    #region Save/Load
    public List<BezierKnot> GetAllKnots()
    {
        return spline.Knots.ToList();
    }
    public List<Pose> GetAllBase()
    {
        List<Pose> list = new List<Pose>();
        for (int i = 0; i < containerBase.childCount; i++)
        {
            Transform child = containerBase.GetChild(i);
            Pose currentPose = new Pose(child.position, child.rotation);
            list.Add(currentPose);
        }
        return list;
    }
    public List<ItemOnConvey> GetAllItems()
    {
        return splineItem.GetAllItems();
    }

    public void BuildConvey(List<BezierKnot> conveyPoint, List<Pose> basePose, List<ItemOnConvey> items)
    {
        SaveSystem.Instance.OnLoadFinish += () => StartCoroutine(FindBuildingToConnect());

        spline = GetComponent<SplineContainer>().Spline;
        spline.Knots = conveyPoint;

        splineItem.SetAllItems(items);

        LoadBaseConvey(basePose);
        SetConveyBuildOnLoad();
    }

    private Vector3 overlapDim = new Vector3(2f, 2f, 2f);
    private IEnumerator FindBuildingToConnect()
    {
        yield return new WaitForSeconds(0.3f);

        buildingAtStart = OverlapConnect(attachementStart.TransformIO.position);
        buildingAtEnd = OverlapConnect(attachementEnd.TransformIO.position);
    }
    private Attachement OverlapConnect(Vector3 center)
    {
        Collider[] allCollider = Physics.OverlapBox(center, overlapDim);

        foreach (Collider collider in allCollider)
        {
            if (collider.TryGetComponent<IAttachement>(out IAttachement iAttachement))
            {
                List<Attachement> attachements = new List<Attachement>();
                if (iAttachement.GetInputAttachements() != null)
                    attachements.AddRange(iAttachement.GetInputAttachements());//get the input of the machine
                if (iAttachement.GetOutputAttachements() != null)
                    attachements.AddRange(iAttachement.GetOutputAttachements());//get the output of the machine

                for (int i = 0; i < attachements.Count; i++)
                {
                    Attachement current = attachements[i];
                    if (Vector3.Distance(current.TransformIO.position, center) <= 0.5f)
                    {
                        current.Convey = this;
                        current.IsFixed = true;
                        return current;
                    }
                }
            }
        }

        return null;
    }

    private void LoadBaseConvey(List<Pose> basePose)
    {
        //destroy exist
        int countBase = containerBase.childCount;
        for (int i = 0; i < countBase; i++)
        {
            Destroy(containerBase.GetChild(i).gameObject);
        }

        //create new
        for (int i = 0; i < basePose.Count; i++)
        {
            CreateBase(basePose[i]);
        }

        //get first and last
        baseStart = containerBase.GetChild(2).gameObject;
        attachementStart = baseStart.GetComponent<Attachement>();
        attachementStart.Init(Attachement.DirectionIO.Input, true);

        baseEnd = containerBase.GetChild(3).gameObject;
        attachementEnd = baseEnd.GetComponent<Attachement>();
        attachementEnd.Init(Attachement.DirectionIO.Output, true);
    }
    private void CreateBase(Pose pose) => Instantiate(prefabBase, pose.position, pose.rotation, containerBase);
    private void SetConveyBuildOnLoad()
    {
        state = State.Build;

        SplineMesh.BuildMesh(spline, splineRender.extrusionInterval, splineRender.baize, splineRender.border);

        animateConvey.Init();
        animateConvey.SetBuild();

        splineMeshCollider.SetSplineBuild();

        splineItem.UpdateStep();

        UpdateBasePos();
        ActivateCollisionBase();
    }
    private void OnDrawGizmos()
    {
        Vector3 center;
        if (attachementStart != null)
        {
            center = attachementStart.TransformIO.position;
            Gizmos.color = Color.red;
            Gizmos.DrawCube(center, overlapDim);
        }

        if (attachementEnd != null)
        {
            center = attachementEnd.TransformIO.position;
            Gizmos.color = Color.red;
            Gizmos.DrawCube(center, overlapDim);
        }
    }
    #endregion

    #region IDestructible
    public void Destruct()
    {
        if (buildingAtStart != null)
        {
            buildingAtStart.Convey = null;
            buildingAtStart.IsFixed = false;
        }

        if (buildingAtEnd != null)
        {
            buildingAtEnd.Convey = null;
            buildingAtEnd.IsFixed = false;
        }

        splineItem.ClearItems();
        animateConvey.Destruction();

        Destroy(gameObject);
    }
    #endregion

    #region Editor
    [ContextMenu("Generate Spline Mesh")]
    private void GenerateSplineMesh()
    {
        Spline target = GetComponent<SplineContainer>().Spline;
        SplineMesh.BuildMesh(target, splineRender.extrusionInterval, splineRender.baize, splineRender.border);
        animateConvey.Init();
        animateConvey.SetBuild();
    }
    #endregion
}