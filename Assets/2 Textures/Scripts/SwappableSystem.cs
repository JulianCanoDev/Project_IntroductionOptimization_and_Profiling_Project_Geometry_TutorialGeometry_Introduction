using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwappableSystem : MonoBehaviour
{
    private static SwappableSystem s_Instance;

    public Text InfoText;

    public string Mode0Name;
    public string Mode1Name;
    
    private List<SwappableModel> m_Models = new List<SwappableModel>();

    private int m_CurrentMode = 0;
    
    private void Awake()
    {
        s_Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void SwapAll()
    {
        m_CurrentMode = 1 - m_CurrentMode;
        foreach (var model in m_Models)
        {
            model.SwapModel();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        InfoText.text = "Current Mode : " + (m_CurrentMode == 0 ? Mode0Name : Mode1Name);
    }

    public static void RegisterModel(SwappableModel model)
    {
        s_Instance.m_Models.Add(model);
    }
}
