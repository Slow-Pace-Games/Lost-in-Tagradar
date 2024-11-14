using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Brush : EditorWindow
{
    #region Enum
    private enum InstanceType
    {
        GPU,
        CPU,
    }
    private enum RayStart
    {
        Mouse,
        Camera,
    }
    private enum InstanceNumber
    {
        Single,
        Multiple,
    }
    private enum PrefabOffsetType
    {
        Position,
        Rotaition,
        Scale,
    }
    private enum Vector3Type
    {
        X,
        Y,
        Z,
    }
    public enum OffsetType
    {
        Exact,
        Random,
    }
    #endregion

    #region Class
    [System.Serializable]
    public class PrefabOffset
    {
        public OffsetData[] transformOffsets = new OffsetData[3];

        public PrefabOffset()
        {
            transformOffsets[0] = new OffsetData("Position Offset");
            transformOffsets[1] = new OffsetData("Rotation Offset");
            transformOffsets[2] = new OffsetData("Scale Offset");
        }
    }
    [System.Serializable]
    public class OffsetData
    {
        public bool isEnable = false;
        public string name;
        public Vector3Offset[] vector3Offsets = new Vector3Offset[3];

        public OffsetData(string name)
        {
            vector3Offsets[0] = new Vector3Offset("X");
            vector3Offsets[1] = new Vector3Offset("Y");
            vector3Offsets[2] = new Vector3Offset("Z");

            this.name = name;
        }
    }
    [System.Serializable]
    public class Vector3Offset
    {
        public string name;

        public float offsetValue = 0f;

        public float offsetFrom = 0f;
        public float offsetTo = 0f;

        public OffsetType offsetType = OffsetType.Exact;

        public Vector3Offset(string name)
        {
            this.name = name;
        }
    }
    #endregion

    [MenuItem("Brush/Brush")]
    public static void OpenBrush()
    {
        Brush window = GetWindow<Brush>();
        window.TryLoadBrushProfiles();
    }

    private bool TryLoadBrushProfiles()
    {
        if (brushProfiles == null && AssetDatabase.LoadAssetAtPath("Assets/Resources/BrushProfiles.asset", typeof(SOBrushProfiles)) != null)
        {
            brushProfiles = AssetDatabase.LoadAssetAtPath("Assets/Resources/BrushProfiles.asset", typeof(SOBrushProfiles)) as SOBrushProfiles;
            return true;
        }
        return false;
    }
    private void CreateBrushProfiles()
    {
        SOBrushProfiles brushProfiles = ScriptableObject.CreateInstance<SOBrushProfiles>();
        this.brushProfiles = brushProfiles;

        AssetDatabase.CreateAsset(brushProfiles, "Assets/Resources/BrushProfiles.asset");

        AssetDatabase.Refresh();
        EditorUtility.SetDirty(this.brushProfiles);
        AssetDatabase.SaveAssets();
    }

    public float radius = 5f;
    public int spawnCount = 5;

    public bool showOriginCircle = true;
    public bool showPreviewPosition = true;
    public bool showPreviewNormal = true;
    public bool showFoldoutPrefabs = false;

    public PrefabOffset prefabOffset = new PrefabOffset();

    public GameObject spawnPrefabs;

    private SODataEnvironment environment;
    public Mesh spawnMesh;
    public Material spawnMaterial;

    private InstanceType instanceType = InstanceType.CPU;
    private RayStart rayStart = RayStart.Mouse;
    private InstanceNumber instanceNumber = InstanceNumber.Multiple;

    private SerializedObject so;
    private SerializedProperty propRadius;
    private SerializedProperty propSpawnCount;
    private SerializedProperty propSpawnMesh;
    private SerializedProperty propSpawnMaterial;
    private SerializedProperty propShowOriginCircle;
    private SerializedProperty propShowPreviewPosition;
    private SerializedProperty propShowPreviewNormal;

    private Vector2 scrollPos;
    private Vector2[] rndPoints;
    private Pose[] spawnPoses;

    private SOBrushProfiles brushProfiles;

    #region GUI
    private void OnGUI()
    {
        using (GUILayout.ScrollViewScope scrollScope = new GUILayout.ScrollViewScope(scrollPos))
        {
            scrollPos = scrollScope.scrollPosition;

            so.Update();

            EnumParametersGUI();
            PreviewParametersGUI();
            so.ApplyModifiedProperties();

            so.Update();
            OffsetPrefabGUI();
            CircleParametersGUI();
            if (so.ApplyModifiedProperties())
            {
                GenerateRandomPoints();
                SceneView.RepaintAll();
            }

            so.Update();
            SpawnParameterGUI();
            GenerateRandomPointsButton();
            BrushProfilesGUI();
            ButtonCpuToGpu();
            so.ApplyModifiedProperties();

            LooseFocus();
        }
    }

    private void EnumParametersGUI()
    {
        //enum
        instanceType = (InstanceType)EditorGUILayout.EnumPopup(instanceType);
        rayStart = (RayStart)EditorGUILayout.EnumPopup(rayStart);
        InstanceNumber last = instanceNumber;
        instanceNumber = (InstanceNumber)EditorGUILayout.EnumPopup(instanceNumber);
        if (instanceNumber != last)
            GenerateRandomPoints();
    }
    private void PreviewParametersGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(propShowOriginCircle, false);
        EditorGUILayout.PropertyField(propShowPreviewPosition, false);
        EditorGUILayout.PropertyField(propShowPreviewNormal, false);
    }
    private void CircleParametersGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(propRadius, false);
        propRadius.floatValue = Mathf.Max(1f, propRadius.floatValue);
        EditorGUILayout.PropertyField(propSpawnCount, false);
        propSpawnCount.intValue = Mathf.Max(1, propSpawnCount.intValue);
    }
    private void OffsetPrefabGUI()
    {
        GUILayout.Space(10f);
        using (new GUILayout.VerticalScope(GUI.skin.box))
        {
            for (int i = 0; i < 3; i++)
            {
                OffsetData offsetDatas = prefabOffset.transformOffsets[i];
                using (new GUILayout.HorizontalScope())
                {
                    offsetDatas.isEnable = EditorGUILayout.Toggle(offsetDatas.isEnable, GUILayout.MaxWidth(25f));
                    if (!prefabOffset.transformOffsets[0].isEnable)
                        GUI.color = Color.gray;

                    EditorGUILayout.LabelField(offsetDatas.name);
                    GUI.color = Color.white;
                }

                if (offsetDatas.isEnable)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector3Offset vector3Offset = offsetDatas.vector3Offsets[j];
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("", GUILayout.MaxWidth(50f), GUILayout.MinWidth(50f));
                            EditorGUILayout.LabelField(vector3Offset.name, GUILayout.MaxWidth(20f), GUILayout.MinWidth(20f));
                            if (vector3Offset.offsetType == OffsetType.Exact)
                            {
                                EditorGUILayout.LabelField("is", GUILayout.MaxWidth(30f));
                                vector3Offset.offsetValue = EditorGUILayout.FloatField(vector3Offset.offsetValue, GUILayout.MaxWidth(100f), GUILayout.MinWidth(100f));
                            }
                            else
                            {
                                EditorGUILayout.LabelField("from", GUILayout.MaxWidth(30f));
                                vector3Offset.offsetFrom = EditorGUILayout.FloatField(vector3Offset.offsetFrom, GUILayout.MaxWidth(100f), GUILayout.MinWidth(100f));
                                EditorGUILayout.LabelField("to", GUILayout.MaxWidth(30f));
                                vector3Offset.offsetTo = EditorGUILayout.FloatField(vector3Offset.offsetTo, GUILayout.MaxWidth(100f), GUILayout.MinWidth(100f));
                            }
                            vector3Offset.offsetType = (OffsetType)EditorGUILayout.EnumPopup(vector3Offset.offsetType, GUILayout.MaxWidth(100f), GUILayout.MinWidth(100f));
                        }
                    }
                }
            }
        }
    }
    private void SpawnParameterGUI()
    {
        //spawn param
        GUILayout.Space(10);
        if (instanceType == InstanceType.GPU)
        {
            environment = EditorGUILayout.ObjectField(environment, typeof(SODataEnvironment), false) as SODataEnvironment;
            EditorGUILayout.PropertyField(propSpawnMesh, false);
            EditorGUILayout.PropertyField(propSpawnMaterial, false);
        }
        else
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Spawn prefab", GUILayout.MaxWidth(85f));
                spawnPrefabs = EditorGUILayout.ObjectField(spawnPrefabs, typeof(GameObject), false) as GameObject;
            }
        }
    }
    private void GenerateRandomPointsButton()
    {
        GUILayout.Space(10);
        if (GUILayout.Button("Regenerate random points"))
        {
            GenerateRandomPoints();
            SceneView.RepaintAll();
        }
    }
    private void BrushProfilesGUI()
    {
        //load brush profiles
        GUILayout.Space(10);
        if (brushProfiles == null)
        {
            if (GUILayout.Button("Refresh brush profiles"))
            {
                TryLoadBrushProfiles();
            }

            if (GUILayout.Button("Create brush profiles"))
            {
                if (!TryLoadBrushProfiles())
                {
                    CreateBrushProfiles();
                }
            }
        }

        //create new brush profiles
        else
        {
            if (GUILayout.Button("Generate a new profile"))
            {
                GenerateNewProfiles();
            }
        }
    }
    private void ButtonCpuToGpu()
    {
        GUI.backgroundColor = new Color(0.5f,0f, 0.5f, 1f);
        if (GUILayout.Button("CPU to GPU"))
        {
            TransformToCpu();
        }
        GUI.backgroundColor = Color.white;
    }

    private void TransformToCpu()
    {
        GameObject[] selection = Selection.gameObjects;

        for(int i =0; i < selection.Length; i++) 
        {
            MeshFilter[] kid = selection[i].GetComponentsInChildren<MeshFilter>();
            environment = Resources.Load("DataEnvironmentGame") as SODataEnvironment;
            for(int j =0; j < kid.Length; j++)
            {
                Matrix4x4 matrix = Matrix4x4.TRS(kid[j].transform.position, kid[j].transform.rotation, kid[j].transform.parent.parent.localScale);
                environment.AddNewItem(new GPURenderer(kid[j].sharedMesh, kid[j].GetComponent<MeshRenderer>().sharedMaterial), matrix);
            }
            DestroyImmediate(selection[i].gameObject);
        }

    }
    private void GenerateNewProfiles()
    {
        if (instanceType == InstanceType.CPU)
        {
            brushProfiles.cpuProfiles.Add(new CPUProfile(radius, spawnCount, spawnPrefabs));
        }
    }
    private void LooseFocus()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GUI.FocusControl(null);
            Repaint();
        }
    }
    #endregion

    #region Enable
    private void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
        CreateSerializedObject();
        GenerateRandomPoints();
    }
    private void CreateSerializedObject()
    {
        so = new SerializedObject(this);

        propRadius = so.FindProperty("radius");
        propSpawnCount = so.FindProperty("spawnCount");

        propShowOriginCircle = so.FindProperty("showOriginCircle");
        propShowPreviewPosition = so.FindProperty("showPreviewPosition");
        propShowPreviewNormal = so.FindProperty("showPreviewNormal");

        propSpawnMesh = so.FindProperty("spawnMesh");
        propSpawnMaterial = so.FindProperty("spawnMaterial");
    }
    #endregion

    private void OnDisable() => SceneView.duringSceneGui -= DuringSceneGUI;

    private void DuringSceneGUI(SceneView sceneView)
    {
        Handles.zTest = CompareFunction.LessEqual;

        if (Event.current.type == EventType.MouseMove)
            SceneView.RepaintAll();

        bool holdingAlt = (Event.current.modifiers & EventModifiers.Alt) != 0;
        if (Event.current.type == EventType.ScrollWheel && holdingAlt)
        {
            float dirScroll = Mathf.Sign(Event.current.delta.y);

            so.Update();
            propRadius.floatValue *= 1f + dirScroll * 0.1f;
            so.ApplyModifiedProperties();
            Repaint();

            Event.current.Use();
        }

        Transform camTf = sceneView.camera.transform;

        Ray ray;
        if (rayStart == RayStart.Mouse)
            ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        else
            ray = new Ray(camTf.position, camTf.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //tangent space
            Vector3 hitNormal = hit.normal;
            Vector3 hitTangent = Vector3.Cross(hitNormal, camTf.forward).normalized;
            Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent).normalized;

            if (showOriginCircle)
            {
                Handles.color = Color.red;
                Handles.DrawAAPolyLine(6, hit.point, hit.point + hitTangent);
                Handles.color = Color.green;
                Handles.DrawAAPolyLine(6, hit.point, hit.point + hitBitangent);
                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(6, hit.point, hit.point + hitNormal);
            }

            Ray GetTangentRay(Vector2 worldPos)
            {
                Vector3 rayOrigin = hit.point + (hitTangent * worldPos.x + hitBitangent * worldPos.y) * radius;
                rayOrigin += hit.normal * 2;// offset
                Vector3 rayDirection = -hitNormal;

                return new Ray(rayOrigin, rayDirection);
            }

            void DrawCircle()
            {
                const int circleDetail = 256;
                const float TAU = 6.28318530718f;
                Vector3[] ringPoints = new Vector3[circleDetail + 1];
                for (int i = 0; i < circleDetail + 1; i++)
                {
                    float t = i / (float)(circleDetail);
                    float angRad = t * TAU;
                    Vector2 dir = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
                    Ray circleRay = GetTangentRay(dir);
                    if (Physics.Raycast(circleRay, out RaycastHit circleHit))
                    {
                        ringPoints[i] = circleHit.point;
                    }
                    else
                    {
                        ringPoints[i] = circleRay.origin;
                    }
                }

                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(ringPoints);
            }
            DrawCircle();

            //draw random points
            Handles.color = Color.white;
            spawnPoses = new Pose[rndPoints.Length];
            int index = 0;
            foreach (Vector2 randomPoint in rndPoints)
            {
                Ray randomPointRay = GetTangentRay(randomPoint);

                if (Physics.Raycast(randomPointRay, out RaycastHit randomPointHit))
                {
                    if (showPreviewPosition)
                        DrawSphere(randomPointHit.point);
                    if (showPreviewNormal)
                        Handles.DrawAAPolyLine(randomPointHit.point, randomPointHit.point + randomPointHit.normal);


                    spawnPoses[index] = new Pose();
                    spawnPoses[index].position = randomPointHit.point;
                    spawnPoses[index].rotation = Quaternion.LookRotation(randomPointHit.normal) * Quaternion.Euler(new Vector3(90f, 0f, 0f));
                }

                index++;
            }
        }

        RandomPointsEvent();
        EventSpawnObject();
    }

    private void GenerateRandomPoints()
    {
        if (instanceNumber == InstanceNumber.Single)
        {
            rndPoints = new Vector2[1]
            {
                Vector2.zero,
            };

            return;
        }

        rndPoints = new Vector2[spawnCount];
        for (int i = 0; i < rndPoints.Length; i++)
        {
            rndPoints[i] = Random.insideUnitCircle;
        }
    }
    private void DrawSphere(Vector3 pos)
    {
        Handles.SphereHandleCap(-1, pos, Quaternion.identity, 0.35f, EventType.Repaint);
    }
    private bool TrySpawnObject()
    {
        if (instanceType == InstanceType.CPU)
        {
            if (spawnPrefabs == null)
                return false;

            foreach (Pose spawnPose in spawnPoses)
            {
                GameObject currentSpawnPrefab = PrefabUtility.InstantiatePrefab(spawnPrefabs) as GameObject;
                Undo.RegisterCreatedObjectUndo(currentSpawnPrefab, "Undo Spawn prefab");

                currentSpawnPrefab.transform.position = spawnPose.position + PositionOffset();
                currentSpawnPrefab.transform.rotation = spawnPose.rotation * RotationOffset();
                currentSpawnPrefab.transform.localScale = ScaleOffset();
            }

            return true;
        }
        else
        {
            if (environment == null || spawnMesh == null || spawnMaterial == null)
                return false;

            List<Matrix4x4> matrixTransform = new List<Matrix4x4>();

            foreach (Pose spawnPose in spawnPoses)
            {
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetTRS(spawnPose.position + PositionOffset(), spawnPose.rotation * RotationOffset(), ScaleOffset());
                matrixTransform.Add(matrix);
            }

            Undo.RecordObject(environment, "Undo Spawn GPU Mesh");

            environment.AddNewItems(new GPURenderer(spawnMesh, spawnMaterial), matrixTransform);

            AssetDatabase.Refresh();
            EditorUtility.SetDirty(environment);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }
    }

    private Vector3 PositionOffset()
    {
        OffsetData positionOffsetData = prefabOffset.transformOffsets[(int)PrefabOffsetType.Position];
        if (positionOffsetData.isEnable)
        {
            return Vector3OffsetResult(positionOffsetData);
        }

        return Vector3.zero;
    }
    private Quaternion RotationOffset()
    {
        OffsetData rotationOffsetData = prefabOffset.transformOffsets[(int)PrefabOffsetType.Rotaition];
        if (rotationOffsetData.isEnable)
        {
            return Quaternion.Euler(Vector3OffsetResult(rotationOffsetData));
        }

        return Quaternion.identity;
    }
    private Vector3 ScaleOffset()
    {
        OffsetData positionOffsetData = prefabOffset.transformOffsets[(int)PrefabOffsetType.Scale];
        if (positionOffsetData.isEnable)
        {
            return Vector3OffsetResult(positionOffsetData);
        }

        return Vector3.one;
    }
    private Vector3 Vector3OffsetResult(OffsetData offsetData)
    {
        Vector3 offsetPos = Vector3.zero;
        Vector3Offset valueOffset = offsetData.vector3Offsets[(int)Vector3Type.X];
        if (valueOffset.offsetType == OffsetType.Exact)
        {
            offsetPos.x = valueOffset.offsetValue;
        }
        else
        {
            offsetPos.x = Random.Range(valueOffset.offsetFrom, valueOffset.offsetTo);
        }

        valueOffset = offsetData.vector3Offsets[(int)Vector3Type.Y];
        if (valueOffset.offsetType == OffsetType.Exact)
        {
            offsetPos.y = valueOffset.offsetValue;
        }
        else
        {
            offsetPos.y = Random.Range(valueOffset.offsetFrom, valueOffset.offsetTo);
        }

        valueOffset = offsetData.vector3Offsets[(int)Vector3Type.Z];
        if (valueOffset.offsetType == OffsetType.Exact)
        {
            offsetPos.z = valueOffset.offsetValue;
        }
        else
        {
            offsetPos.z = Random.Range(valueOffset.offsetFrom, valueOffset.offsetTo);
        }

        return offsetPos;
    }

    private void EventSpawnObject()
    {
        if (Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.KeyDown)
        {
            if (TrySpawnObject())
            {
                GenerateRandomPoints();
            }
        }
    }
    private void RandomPointsEvent()
    {
        bool holdingAlt = (Event.current.modifiers & EventModifiers.Shift) != 0;
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && holdingAlt)
        {
            GenerateRandomPoints();
            SceneView.RepaintAll();
            Event.current.Use();
        }
    }
}