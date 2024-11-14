using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickUiSound : MonoBehaviour
{
    [SerializeField] AudioSource source;

    public void PlayClickSound()
    {
        source.Play();
    }
}
