using UnityEngine;
using System.Collections;

public class OrganelleReachedScript : MonoBehaviour {
	public GameObject Organelle;
    //public GameObject prevOrg;
    //private OrganelleReachedScript a;
     bool soundplayed=false;
//float sfxvolume=1.0f;
    AudioSource sfx;


	// audiosource array to stop all sounds  ( see method StopAllAudio below)
	private AudioSource[] allAudioSources;



	void Start () {
        //if (prevOrg != null)
        //{
        //    a = prevOrg.GetComponent<OrganelleReachedScript>();
        //}

	}
    void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        sfx = Organelle.GetComponent<AudioSource>();
        sfx.clip = Organelle.GetComponent<AudioSource>().clip;
        //if(a==null)
        //{
        //    if(prevOrg.GetComponent<AudioSource>().isPlaying)
        //    {
        //        fadeOut();
        //    }
        //    sfx.Play();
        //}
        //else if (a.isAudio())
        //{
        //    a.fadeOut();
        //    sfx.Play();
        //}
        //else
        if(!soundplayed)
        {
			StopAllAudio();
            sfx.Play();
            soundplayed = true;
        }
    }
    //void fadeOut()
    //{
    //    if (sfxvolume > 0.1)
    //    {
    //        sfxvolume -= (float)(0.5 * Time.deltaTime);
    //        sfx.volume = sfxvolume;
    //    }
    //}
    //bool isAudio()
    //{
    //    return sfx.isPlaying;
    //}


	
	void StopAllAudio() {
		allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		foreach( AudioSource audioS in allAudioSources) {
			audioS.Stop();
		}
	}


	}

	// Update is called once per frame
	

  /*  bool getSoundPlayed()
    {
        return soundplayed;
    }*/

