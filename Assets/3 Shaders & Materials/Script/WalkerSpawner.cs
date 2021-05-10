using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WalkerSpawner : MonoBehaviour
{
    public GameObject Prefab;
    public float Distance;
    public float Speed;
    public int Count;
    public float MinTime;
    public float MaxTime;

    private float m_NextSpawn;
    private List<GameObject> m_Instances = new List<GameObject>();
    
    private void Start()
    {
        m_NextSpawn = -1;
    }

    private void Update()
    {
        if (m_Instances.Count < Count)
        {
            m_NextSpawn -= Time.deltaTime;

            if (m_NextSpawn <= 0)
            {
                m_NextSpawn = Random.Range(MinTime, MaxTime);

                var obj = Instantiate(Prefab);
                obj.transform.position = transform.position;
                obj.transform.forward = transform.forward;

                var animator = obj.GetComponentInChildren<Animator>();
                animator.SetFloat("Speed", 1.0f);
                
                m_Instances.Add(obj);
            }
        }
        
        for (int i = 0; i < m_Instances.Count; ++i)
        {
            m_Instances[i].transform.position =
                m_Instances[i].transform.position + transform.forward * Speed * Time.deltaTime;

            if (Vector3.Distance(m_Instances[i].transform.position, transform.position) > Distance)
            {
                m_Instances[i].transform.position = transform.position;
            }
        }
    }
}
