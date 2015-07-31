using UnityEngine;
using System.Collections;

public class OraganelleReachedScript : MonoBehaviour {
	public GameObject Organelle;
	public GameObject OrganelleText;
    AudioSource sfx;
	// Use this for initialization
	void Start () {
		OrganelleText.SetActive (false);
	}

	void OnTriggerEnter (Collider other) {
		Debug.Log ("Entered");
        sfx= Organelle.GetComponent<AudioSource>();
        sfx.clip = Organelle.GetComponent<AudioSource>().clip;
        sfx.Play();
        OrganelleText.SetActive(true);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
