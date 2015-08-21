using UnityEngine;
using System.Collections;

public class LoadLevelOnTrigger : MonoBehaviour
{

    public string levelName;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Application.LoadLevel(levelName);
        }
    }
}