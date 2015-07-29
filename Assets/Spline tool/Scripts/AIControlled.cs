// Copyright (c) 2014 Raed Abdullah 

using UnityEngine;
using System.Collections;

/// <summary>
/// this script can make the object move on the way you want in code
/// that means that if there is more than 1 way 
/// you can put the controll type in the followSpline component to AI controlled 
/// the this ChooseSubway function in this script will be called
/// and you will need to edit the i variable 0 : the first way you drew 
/// 1 : the second way 
/// 2 : the third way 
/// ....
/// ...
/// ..
/// note : this function will be only called if there is more than 1 way so it is safe to put i = 1
/// but if you want to put it i = 2
/// be sure there is more than 2 ways using the nextPos list 
/// like this if(nextPos.Length > 2) i = 2
/// </summary>
public class AIControlled : MonoBehaviour 
{
    public void ChooseSubway(ref int i, Vector3[] nextPos)
    {
        // you can put here your fancy code and calculate positions and what ever you want 
        // but i will assign it as simple as possible O.o 
        // this nextPos variable holds a list of positions
        // for the half way point between this object and the next point 
        // the reason that it's the half way through not the next point position exactly
        // is to get better results if you want to get a direction (because its a curve)
        //
        // you can get how many ways you have to choose from simply by nextPos.Length
        i = 1;
    }
}
