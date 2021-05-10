using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTimeout : MonoBehaviour {

    public uint timeout = 5;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, timeout);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
