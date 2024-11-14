using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

//alors première chose t'a vu le nom de ta class Convoyer et pas Conveyor :)

//data a save
//point de la spline (position,tangent in,tangent out)
//pied des convey (position,rotation)
//rank (enum -> int)
//et pour les connections je le fais moi même via une fct

public class ConvoyerContainer : SaveContainer
{
    //List convertie des enfants du gameobject (container)
    [SerializeField] private List<ConvoyerSaveable> convoyers = new List<ConvoyerSaveable>();
    [SerializeField] GameObject prefabBase;
    [SerializeField] SODatabase soDataBase;

    protected override void Convert()
    {
        convoyers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            ConvoyerSaveable convoyerSaveable = new ConvoyerSaveable();

            Convey convey = go.GetComponent<Convey>();
            List<BezierKnot> points = convey.GetAllKnots();
            List<Pose> baseTransforms = convey.GetAllBase();
            List<ItemOnConvey> itemsOnConvey = convey.GetAllItems();

            convoyerSaveable.name = go.name;

            convoyerSaveable.transform = Serialize.ConvertSerialized(go.transform);

            convoyerSaveable.transformKnots = new PointSpline[points.Count];
            for (int j = 0; j < points.Count; j++)
            {
                convoyerSaveable.transformKnots[j] = new PointSpline();

                convoyerSaveable.transformKnots[j].position = Serialize.ConvertVector3Serialized(points[j].Position);
                convoyerSaveable.transformKnots[j].rotation = Serialize.ConvertQuaternionSerialized(points[j].Rotation);

                convoyerSaveable.transformKnots[j].tangeanteIn = Serialize.ConvertVector3Serialized(points[j].TangentIn);
                convoyerSaveable.transformKnots[j].tangeanteOut = Serialize.ConvertVector3Serialized(points[j].TangentOut);
            }

            convoyerSaveable.transformBases = new TransformSerialized[baseTransforms.Count];

            for (int j = 0; j < baseTransforms.Count; j++)
            {
                convoyerSaveable.transformBases[j] = Serialize.ConvertSerialized(baseTransforms[j]);
            }

            for (int j = 0; j < itemsOnConvey.Count; j++)
            {
                ItemsConveySaveable itemsConveySaveable = new ItemsConveySaveable();
                itemsConveySaveable.name = Serialize.ConvertItemOnConvey(itemsOnConvey[j]).name;
                itemsConveySaveable.time = itemsOnConvey[j].time;
                itemsConveySaveable.isMoving = itemsOnConvey[j].isMoving;

                itemsConveySaveable.itemID = Serialize.ConvertItemOnConvey(itemsOnConvey[j]).itemID;
                itemsConveySaveable.transform = Serialize.ConvertItemOnConvey(itemsOnConvey[j]).transform;

                convoyerSaveable.itemsConveySaveables.Add(itemsConveySaveable);
            }


            convoyers.Add(convoyerSaveable);
        }

        SaveSystem.Instance.ClassContainer.convoyersList = convoyers;
    }

    protected override void Load()
    {
        convoyers = SaveSystem.Instance.ClassContainer.convoyersList;

        for (int i = 0; i < convoyers.Count; i++)
        {
            ConvoyerSaveable current = convoyers[i];
            GameObject go = Instantiate(prefabBase, transform);
            Convey convey = go.GetComponent<Convey>();
            List<BezierKnot> points = new List<BezierKnot>();
            List<Pose> baseTransforms = new List<Pose>();
            List<ItemOnConvey> items = new List<ItemOnConvey>();

            go.name = current.name;

            go.transform.position = Serialize.UnConvert(current.transform, go.transform).position;
            go.transform.rotation = Serialize.UnConvert(current.transform, go.transform).rotation;

            // Bases
            for (int j = 0; j < current.transformBases.Length; j++)
            {
                baseTransforms.Add(Serialize.UnConvertPose(current.transformBases[j]));
            }

            // Knots
            for (int j = 0; j < current.transformKnots.Length; j++)
            {
                points.Add(Serialize.UnConvertPose(current.transformKnots[j]));
            }

            // Items
            for (int j = 0; j < current.itemsConveySaveables.Count; j++)
            {
                items.Add(Serialize.UnConvertItemOnConvey(current.itemsConveySaveables[j]));
                items[j].item = soDataBase.AllItems.Where(item => item.id == current.itemsConveySaveables[j].itemID).FirstOrDefault(item => item); // comme la sql
            }


            convey.BuildConvey(points, baseTransforms, items);
        }
    }
}


[System.Serializable]
public class ConvoyerSaveable
{
    public string name;

    public TransformSerialized transform;
    public PointSpline[] transformKnots;
    public TransformSerialized[] transformBases;

    public List<ItemsConveySaveable> itemsConveySaveables = new List<ItemsConveySaveable>();
}

public class PointSpline
{
    public Vector3Serialized position = new Vector3Serialized();
    public QuaternionSerialized rotation = new QuaternionSerialized();

    public Vector3Serialized tangeanteOut = new Vector3Serialized();
    public Vector3Serialized tangeanteIn = new Vector3Serialized();
}

[System.Serializable]
public class ItemsConveySaveable
{
    public string name;
    public float time;
    public bool isMoving;

    public int itemID;
    public Matrix4x4Serialized transform;
}