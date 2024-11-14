using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public static class Serialize
{
    static public TransformSerialized ConvertSerialized(Transform _transform)
    {
        TransformSerialized t = new TransformSerialized();

        t.vector3Serialized = ConvertVector3Serialized(_transform.position);
        t.quaternionSerialized = ConvertQuaternionSerialized(_transform.rotation);

        return t;
    }
    static public TransformSerialized ConvertSerialized(Pose _pose)
    {
        TransformSerialized t = new TransformSerialized();

        t.vector3Serialized = ConvertVector3Serialized(_pose.position);
        t.quaternionSerialized = ConvertQuaternionSerialized(_pose.rotation);

        return t;
    }

    static public Vector3Serialized ConvertVector3Serialized(Vector3 _vector3)
    {
        Vector3Serialized t = new Vector3Serialized();

        t.Xpos = _vector3.x;
        t.Ypos = _vector3.y;
        t.Zpos = _vector3.z;

        return t;
    }

    static public QuaternionSerialized ConvertQuaternionSerialized(Quaternion _quaternion)
    {
        QuaternionSerialized t = new QuaternionSerialized();

        t.Xrot = _quaternion.x;
        t.Yrot = _quaternion.y;
        t.Zrot = _quaternion.z;
        t.Wrot = _quaternion.w;

        return t;
    }

    static public Matrix4x4Serialized ConvertMatrixSerialized(Matrix4x4 _matrix4x4)
    {
        Matrix4x4Serialized t = new Matrix4x4Serialized();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                t.matrix[i, j] = _matrix4x4[i, j];
            }
        }

        return t;
    }


    static public ItemSaveable ConvertSOItem(Item _item)
    {
        ItemSaveable tempItem = new ItemSaveable();
        if (_item != null)
        {
            if (_item.itemType == null)
            {
                tempItem.id = -1;
                tempItem.number = 0;
                return tempItem;
            }
            tempItem.id = _item.itemType.id;
            tempItem.number = _item.stacks;
        }
        else
        {
            tempItem.id = -1;
            tempItem.number = 0;
        }

        Debug.Log(tempItem.id);

        return tempItem;
    }

    static public ItemsConveySaveable ConvertItemOnConvey(ItemOnConvey _itemOnConvey)
    {
        ItemsConveySaveable itemsConveySaveable = new ItemsConveySaveable();

        if (_itemOnConvey != null)
        {
            itemsConveySaveable.name = _itemOnConvey.item.name;
            itemsConveySaveable.itemID = _itemOnConvey.item.id;
            itemsConveySaveable.transform = ConvertMatrixSerialized(_itemOnConvey.transform);
        }

        return itemsConveySaveable;
    }

    static public Matrix4x4 UnConvertMatrixSerialized(Matrix4x4Serialized _matrix4x4)
    {
        Matrix4x4 t = new Matrix4x4();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                t[i, j] = _matrix4x4.matrix[i, j];
            }
        }

        return t;
    }

    static public Transform UnConvert(TransformSerialized _serialized, Transform t)
    {
        t.position = new Vector3(_serialized.vector3Serialized.Xpos, _serialized.vector3Serialized.Ypos, _serialized.vector3Serialized.Zpos);
        t.rotation = new Quaternion(_serialized.quaternionSerialized.Xrot, _serialized.quaternionSerialized.Yrot, _serialized.quaternionSerialized.Zrot, _serialized.quaternionSerialized.Wrot);

        return t;
    }
    static public Pose UnConvertPose(TransformSerialized _serialized)
    {
        Pose tempPose = new Pose();

        tempPose.position = new Vector3(_serialized.vector3Serialized.Xpos, _serialized.vector3Serialized.Ypos, _serialized.vector3Serialized.Zpos);
        tempPose.rotation = new Quaternion(_serialized.quaternionSerialized.Xrot, _serialized.quaternionSerialized.Yrot, _serialized.quaternionSerialized.Zrot, _serialized.quaternionSerialized.Wrot);

        return tempPose;
    }
    static public BezierKnot UnConvertPose(PointSpline _serialized)
    {
        BezierKnot knot = new BezierKnot();

        knot.Position = new Vector3(_serialized.position.Xpos, _serialized.position.Ypos, _serialized.position.Zpos);
        knot.Rotation = new Quaternion(_serialized.rotation.Xrot, _serialized.rotation.Yrot, _serialized.rotation.Zrot, _serialized.rotation.Wrot);

        knot.TangentIn = new Vector3(_serialized.tangeanteIn.Xpos, _serialized.tangeanteIn.Ypos, _serialized.tangeanteIn.Zpos);
        knot.TangentOut = new Vector3(_serialized.tangeanteOut.Xpos, _serialized.tangeanteOut.Ypos, _serialized.tangeanteOut.Zpos);
        return knot;
    }

    static public ItemOnConvey UnConvertItemOnConvey(ItemsConveySaveable _itemsConveySaveable)
    {
        ItemOnConvey itemsOnConvey = new ItemOnConvey();

        itemsOnConvey.time = _itemsConveySaveable.time;
        itemsOnConvey.isMoving = _itemsConveySaveable.isMoving;

        itemsOnConvey.transform = UnConvertMatrixSerialized(_itemsConveySaveable.transform);

        return itemsOnConvey;
    }
}

[System.Serializable]
public class TransformSerialized
{
    public Vector3Serialized vector3Serialized = new Vector3Serialized();
    public QuaternionSerialized quaternionSerialized = new QuaternionSerialized();
}

public class Vector3Serialized
{
    public float Xpos;
    public float Ypos;
    public float Zpos;
}

public class QuaternionSerialized
{
    public float Xrot;
    public float Yrot;
    public float Zrot;
    public float Wrot;
}

public class Matrix4x4Serialized
{
    public float[,] matrix = new float[4, 4];
}

public class ClassContainer
{
    public int tutoProgress;
    public bool[] shipProgress = new bool[4];
    public List<ResourceSaveable> resourcesList = new List<ResourceSaveable>();
    public List<MachineSaveable> machinesList = new List<MachineSaveable>();
    public List<ConvoyerSaveable> convoyersList = new List<ConvoyerSaveable>();
    public List<PlayerSaveable> playersList = new List<PlayerSaveable>();
    public SODatabase.DatabaseSaver database = new SODatabase.DatabaseSaver(); 
}

public class InfoContainer
{
    public SaveInfoSaveable saveInfo = new SaveInfoSaveable();
}


[System.Serializable]
public class SettingsToSave
{
    public GraphicsSettingsSaveable graphics = new GraphicsSettingsSaveable();
    public PlayerSettingsSaveable playerSetSaveable = new PlayerSettingsSaveable();
    public AudioSettingsSaveable audio = new AudioSettingsSaveable();
}