using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SwappableModel : MonoBehaviour
{
    public GameObject[] Objects;

    private int m_Current = 0;

    private void Start()
    {
        SwappableSystem.RegisterModel(this);
    }

    public void SwapModel()
    {
        Objects[m_Current].gameObject.SetActive(false);
        m_Current = 1 - m_Current;
        Objects[m_Current].gameObject.SetActive(true);
    }
}
