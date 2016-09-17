using UnityEngine;
using System.Collections;

public class DatabaseLayerColor : MonoBehaviour {
	MeshRenderer meshRenderer;

	void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer> ();
	}

	public void SetMaterial(Material material)
	{
		meshRenderer.sharedMaterial = material;
	}
}
