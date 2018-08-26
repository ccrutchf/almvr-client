using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFacePlayer : MonoBehaviour {

    private GameObject mainCamera;

	// Use this for initialization
	void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
        transform.forward = transform.position - mainCamera.transform.position;
	}
}
