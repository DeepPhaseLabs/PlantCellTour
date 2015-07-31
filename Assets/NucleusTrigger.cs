using UnityEngine;
using System.Collections;

public class NucleusTrigger : MonoBehaviour {
	public GameObject Nucleus;


	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter (Collider other) {
		Debug.Log ("Entered");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
