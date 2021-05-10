using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class LODChanger : MonoBehaviour
{
    public void ChangeLOD(float value)
    {
        QualitySettings.lodBias = 1.0f + value;
    }
}
