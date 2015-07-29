// Copyright (c) 2014 Raed Abdullah
// this script will work only for Arrow prefab located under Assets/Spline tool/Arrow.prefab
// *************************************note**************************************
// this won't work if you have Edit > project settings > phyiscs > Raycast hit triggers = false

using UnityEngine;

public class Arrow : MonoBehaviour 
{
    public delegate void VoidDelegate(int selectedWay);

    // function to call when the arrow is pressed 
    // assigned out of this script
    public VoidDelegate CallBack;
    public int subWay;

    Color mainColor = Color.yellow;
    Color hoverColor = Color.green;
    Material myMat;

    void Start()
    {
        // creating a new instance of a material
        myMat = new Material(transform.GetChild(0).GetComponent<Renderer>().material);
        // setting the material to the arrow material 
        transform.GetChild(0).GetComponent<Renderer>().material = myMat;
        myMat.color = mainColor;
    }

    void Update()
    {
        // simple raycasting form camera to the arrow
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        myMat.color = mainColor;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform == transform)
            {
                myMat.color = hoverColor;
                // calling the function inside the moving script
                if (Input.GetMouseButtonUp(0)) CallBack(subWay);
            }
        }
    }
}
