using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabInstantiator : MonoBehaviour
{
    public GameObject MultiMaterialPrefab;
    public GameObject SingleMaterialPrefab;

    public int InstanceCount = 200;

    private Renderer[] m_MultiMatRenderer;
    private Renderer[] m_SingleMatRenderer;

    private MeshFilter[] m_MultiMatMeshFilter;
    private MeshFilter[] m_SingleMatMeshFilter;
    
    private List<Renderer[]> m_RunningInstance;
    private List<MeshFilter[]> m_RunningInstanceFilter;
    private bool m_UsingMultiMat = true;
    
    // Start is called before the first frame update
    void Start()
    {
        m_MultiMatRenderer = MultiMaterialPrefab.GetComponentsInChildren<Renderer>();
        m_SingleMatRenderer = SingleMaterialPrefab.GetComponentsInChildren<Renderer>();

        m_MultiMatMeshFilter = MultiMaterialPrefab.GetComponentsInChildren<MeshFilter>();
        m_SingleMatMeshFilter = SingleMaterialPrefab.GetComponentsInChildren<MeshFilter>();
        
        m_RunningInstance = new List<Renderer[]>();
        m_RunningInstanceFilter = new List<MeshFilter[]>();

        for (int i = 0; i < InstanceCount; ++i)
        {
            var newInstance = Instantiate(MultiMaterialPrefab);
            newInstance.transform.position = new Vector3(
                Random.Range(-100.0f, 100.0f),
                0,
                Random.Range(-100.0f, 100.0f));
            
            m_RunningInstance.Add(newInstance.GetComponentsInChildren<Renderer>());
            m_RunningInstanceFilter.Add(newInstance.GetComponentsInChildren<MeshFilter>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < m_RunningInstance.Count; ++i)
            {
                var renderers = m_RunningInstance[i];
                var filters = m_RunningInstanceFilter[i];
                
                for (int j = 0; j < renderers.Length; ++j)
                {
                    filters[j].mesh = m_UsingMultiMat
                        ? m_SingleMatMeshFilter[j].sharedMesh
                        : m_MultiMatMeshFilter[j].sharedMesh;
                    
                    renderers[j].materials = m_UsingMultiMat
                        ? m_SingleMatRenderer[j].sharedMaterials
                        : m_MultiMatRenderer[j].sharedMaterials;
                }
            }

            m_UsingMultiMat = !m_UsingMultiMat;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label($"Press spacebar to switch between multi material and single material prefabs. Using multi material : {m_UsingMultiMat} ");
    }
}
