using UnityEngine;
using System.Collections;

public class OraganelleReachedScript : MonoBehaviour {
	public GameObject Organelle;
	public GameObject OrganelleText;

	// Use this for initialization
	void Start () {
		OrganelleText.SetActive (false);
	}

	void OnTriggerEnter (Collider other) {
		Debug.Log ("Entered");
		OrganelleText.SetActive (true);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
