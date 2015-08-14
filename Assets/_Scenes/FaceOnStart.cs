using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Transform))]

public class FaceOnStart : MonoBehaviour {
    Transform button;
    Camera cam;
    // Use this for initialization
	void Start () {
	cam.transform.LookAt(button, Vector3.up);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
