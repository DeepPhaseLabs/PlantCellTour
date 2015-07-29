// Copyright (c) 2014 Raed Abdullah

using UnityEngine;
using System.Collections;

/// <summary>
/// changes the color of the object 
/// this behaviour is managed by FollowSpline component in the advanced settings (events)
/// </summary>
public class ChangeColor : MonoBehaviour 
{
    public void ChangeColorTo(int to)
    {
        Color c = new Color();
        switch (to)
        {
            case 1: c = Color.cyan; break;
            case 2: c = Color.green; break;
            case 3: c = Color.red; break;
            default: c = Color.white; break;
        }
        GetComponent<Renderer>().material.color = c;
    }
}
