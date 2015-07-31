using UnityEngine;
using System.Collections;

public class NucleusTrigger : MonoBehaviour {
	public GameObject Organelle;
	public GameObject OrganelleText;

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter (Collider other) {
		Debug.Log ("Entered");
		OrganelleText.SetActive (false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
