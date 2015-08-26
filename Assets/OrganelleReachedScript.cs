﻿using UnityEngine;
using System.Collections;

public class OrganelleReachedScript : MonoBehaviour {
	public GameObject Organelle;
//	public GameObject OrganelleText;
    bool soundplayed;
    AudioSource sfx;
	// Use this for initialization
	void Start () {
	//	OrganelleText.SetActive (false);
        soundplayed = false;
	}

	void OnTriggerEnter (Collider other) {
		Debug.Log (other.gameObject.tag);
        sfx= Organelle.GetComponent<AudioSource>();
        sfx.clip = Organelle.GetComponent<AudioSource>().clip;
        if (!soundplayed)
        {
            sfx.Play();
            soundplayed = true;
        }

    //    OrganelleText.SetActive(true);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
