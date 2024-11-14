using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Inputs", menuName = "Player", order = 1)]
public class SOInputs : ScriptableObject
{
    public PlayerInputActions inputActions;

    public void Init() => inputActions = new PlayerInputActions();
    public void Destroy()
    {
        inputActions.Dispose();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
#endif
    }
}
