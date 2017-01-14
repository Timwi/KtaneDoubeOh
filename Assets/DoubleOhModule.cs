using System;
using System.Collections.Generic;
using System.Linq;
using DoubleOh;
using UnityEngine;
using Rnd = UnityEngine.Random;

/// <summary>
/// On the Subject of Double-Oh
/// Created by Timwi
/// </summary>
public class DoubleOhModule : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;

    void Start()
    {
        Debug.Log("[Double-Oh] Started");
    }

    void ActivateModule()
    {
        Debug.Log("[Double-Oh] Activated");
    }
}
