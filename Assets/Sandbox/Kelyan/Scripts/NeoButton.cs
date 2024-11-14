using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NeoButton : MonoBehaviour
{    
    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
    }
}
