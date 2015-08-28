using UnityEngine;
using System.Collections;

public class OrganelleReachedScript : MonoBehaviour {
	public GameObject Organelle;
    public GameObject prevOrg;
    private OrganelleReachedScript a;
  //  public bool soundplayed;
float sfxvolume=1.0f;
    AudioSource sfx;
	void Start () {
        if (prevOrg != null)
        {
            a = prevOrg.GetComponent<OrganelleReachedScript>();
        }

	}
    void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        sfx = Organelle.GetComponent<AudioSource>();
        sfx.clip = Organelle.GetComponent<AudioSource>().clip;
        if(a==null)
        {
            if(prevOrg.GetComponent<AudioSource>().isPlaying)
            {
                fadeOut();
            }
            sfx.Play();
        }
        else if (a.isAudio())//!soundplayed)
        {
            a.fadeOut();
            sfx.Play();
        }
        else
        {

            sfx.Play();
            //  soundplayed = true;
        }
    }
      /*  else
        {
            a.fadeOut();
              if (audio1Volume <= 0.1) {
         if(track2Playing == false)
         {
           track2Playing = true;
           audio.clip = track2;
           audio.Play();
         }
         
         fadeIn();
     }
 }
        }*/
    void fadeOut()
    {
        if (sfxvolume > 0.1)
        {
            sfxvolume -= (float)(0.5 * Time.deltaTime);
            sfx.volume = sfxvolume;
        }
      //  return sfx.volume;

    }
    bool isAudio()
    {
        return sfx.isPlaying;
    }
	}

	// Update is called once per frame
	

  /*  bool getSoundPlayed()
    {
        return soundplayed;
    }*/

