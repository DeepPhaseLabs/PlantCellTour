using UnityEngine;
using System.Collections;

public class Checkpoint1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter (Collider other) {
		Debug.Log ("Detected");
	}

	// Update is called once per frame
	void Update () {
	
	}
}
