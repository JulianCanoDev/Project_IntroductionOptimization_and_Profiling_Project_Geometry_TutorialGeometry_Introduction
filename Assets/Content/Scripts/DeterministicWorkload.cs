using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeterministicWorkload : MonoBehaviour {

    public int seed = 0;

	// Use this for initialization
	void Awake () {
        Random.InitState(seed);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
