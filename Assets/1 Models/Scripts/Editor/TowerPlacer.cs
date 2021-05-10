using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TowerPlacer : EditorWindow
{
    private int m_CountX, m_CountY;
    private float m_Spacing = 1.0f;
    
    private GameObject[] m_Prefabs = new GameObject[0];
    
    [MenuItem("Tools/PlacePrefabInGrid")]
    static void Open()
    {
        var o = GetWindow<TowerPlacer>();
        o.Show();
    }

    private void OnGUI()
    {
        m_CountX = EditorGUILayout.IntField("Count X", m_CountX);
        m_CountY = EditorGUILayout.IntField("Count Y", m_CountY);
        m_Spacing = EditorGUILayout.FloatField("Spacing", m_Spacing);

        if (m_Prefabs != null)
        {
            for (int i = 0; i < m_Prefabs.Length; ++i)
            {
                m_Prefabs[i] = EditorGUILayout.ObjectField("Prefabs", m_Prefabs[i], typeof(GameObject), false) as GameObject;
            }
        }
        
        if(GUILayout.Button("Add"))
            ArrayUtility.Add(ref m_Prefabs, null);

        if(GUILayout.Button("Populate"))
            Populate();    
    }

    void Populate()
    {
        if (m_Prefabs == null || m_Prefabs.Length == 0)
            return;

        int currentPrefab = 0;
        
        for (int y = 0; y < m_CountY; ++y)
        {
            float py = -m_CountY * 0.5f * m_Spacing + y * m_Spacing;
            
            for (int x = 0; x < m_CountX; ++x)
            {
                float px = -m_CountX * 0.5f * m_Spacing + x * m_Spacing;

                var inst = PrefabUtility.InstantiatePrefab(m_Prefabs[currentPrefab], Selection.activeTransform) as GameObject;
                inst.transform.localPosition = new Vector3(px, 0, py);

                currentPrefab += 1;

                if (currentPrefab >= m_Prefabs.Length)
                    currentPrefab = 0;
            }
        }
        
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
