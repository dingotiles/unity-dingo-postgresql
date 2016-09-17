using UnityEngine;
using System.Collections;

public class RotatorViewHelper : MonoBehaviour {
	public float x = 15;
	public float y = 30;
	public float z = 45;

	void Update () 
	{
		transform.Rotate (new Vector3 (x, y, z) * Time.deltaTime);
	}
}