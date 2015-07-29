using UnityEngine;
using System.Collections;

public class Choose : MonoBehaviour {

	public int someValue;

	public void ChooseSubway(ref int i, Vector3[] nextPos){
		if (someValue > 100)
			i = 1; 
		else i = 0;
	
}
}
