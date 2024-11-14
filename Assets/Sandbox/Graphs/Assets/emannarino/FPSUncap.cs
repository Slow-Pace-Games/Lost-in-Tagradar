using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSUncap : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 300;
    }

}
