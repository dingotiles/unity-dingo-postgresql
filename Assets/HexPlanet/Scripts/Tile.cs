using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Tile : MonoBehaviour {

	public static float planetScale;
	private static int ID = 0;
	//private static float tileRadius = 0.8f;

	[Tooltip("The instance of the hexsphere which constructed this tile")]
	public Hexsphere parentPlanet;
	
	public List<Tile> neighborTiles;

	//Tile Attributes
	[Tooltip("Whether or not navigation will consider this tile as a valid to move over")]
	public bool navigable = true;
	[Tooltip("The cost of moving across this tile in terms of pathfinding weight.  Pathfinding will prioritize the lowest cost path.")]
	[Range(1, 100)]
	public int pathCost = 1;

	//The center of this tile in worldspace as calculated by the planet.  Is slightly offset downwards from the transforms position in the local Y.
	public Vector3 center {
		get{ return tileRenderer.bounds.center; }
	}

	//The position of this tile as reported by the renderer in world space.  More strict than the above center.
	public Vector3 centerRenderer {
		get{ return tileRenderer.bounds.center; }
	}

	private int color;
	private int id;

	[HideInInspector]
	public Renderer tileRenderer;

	private float maxTileRadius;
	private const float tileAlpha = .75f;
	public static Color[] colors = new Color[]{new Color(1f, 1f, 1f, 0f), new Color(1f, 0.235f, 0f, tileAlpha), new Color(0.51f, 0.137f, 0.725f, tileAlpha), new Color(0.294f, 0.725f, 0f, tileAlpha), new Color(1f, .5f, 0f, tileAlpha)};

	//Used to specify which tile is currently selected so that any tile can query the selected tile or assign themselves as selected.
	private static Tile selectedTile;
	//The center of the tile in worldspace as assigned by the hexsphere during generation.  Not affected by the scale of the planet.
//	[SerializeField, HideInInspector]
//	private Vector3 centerUnscaled;

	void Awake(){
		tileRenderer = GetComponent<Renderer> ();
	}

	public void Initialize(Vector3 coordinates){
		tileRenderer = GetComponent<Renderer> ();
//		centerUnscaled = coordinates;
		id = ID;
		ID++;
	}

	void OnMouseEnter(){
		Pointer.instance.setPointer (PointerStatus.TILE, transform);
		
	}
	void OnMouseExit(){
		Pointer.instance.unsetPointer ();
	}
	
	void OnMouseDown(){
		//Demo function
		pathfindingDrawDemo ();
	}

	/// <summary>
	/// Just a simple demo function that allows you to click on two tiles and draw the shortest path between them.
	/// </summary>
	public void pathfindingDrawDemo(){
		if (selectedTile == null) {
			selectedTile = this;
		}
		else if(selectedTile != this){
			Stack<Tile> path = new Stack<Tile>();
			if(parentPlanet.navManager.findPath(selectedTile, this, out path)){
				parentPlanet.navManager.drawPath(path);
				selectedTile = null;
			}
		}
	}

	//NEW FIND NEIGHBORS
	public void FindNeighbors(){
		//Extend a sphere around this tile to find all adjacent tiles within the spheres radius
		Collider[] neighbors = Physics.OverlapSphere (center, maxTileRadius);
		//OverlapSphere detects the current tile so we must omit this tile from the list
		neighborTiles = new List<Tile> ();
		int j = 0;
		for(int i = 0; i < neighbors.Length; i++){
			if(neighbors[i] != this.GetComponent<Collider>() && neighbors[i].gameObject.GetComponent<Tile>() != null){
				neighborTiles.Add(neighbors[i].gameObject.GetComponent<Tile>());
				j++;
			}
		}
	}

	public void placeObject(GameObject obj){
		obj.transform.position = center;
		obj.transform.up = transform.up;
	}

	public void setColor(int col){
		color = col;
		Material tempMaterial = new Material(GetComponent<Renderer>().sharedMaterial);
		tempMaterial.color = colors[color];
		GetComponent<Renderer>().sharedMaterial = tempMaterial;
	}
	
	public void setTileRadius(float r){
		this.maxTileRadius = r;
	}

	public int getID(){
		return id;
	}

}
