using UnityEngine;

public class InputCreator : MonoBehaviour
{
    public SOInputs input;
    public SODatabase data;
    private void Awake()
    {
        input.Init();
        //data.InitId();
    }
}