using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Hexsphere : MonoBehaviour {

	public static List<Hexsphere> planetInstances = new List<Hexsphere> ();
	public static int Planet_ID;
	public static float unitScale;

	[HideInInspector]
	public int planetID;
	[Tooltip("Should this planet build itself when the game enters Play Mode?")]
	public bool generateOnPlay;
	//[Tooltip("Set this to true if you dont want to build the tiles as separate gameobjects.  Will just build a single mesh with no pathfinding features.")]
	//public bool dontBuildSeparateTiles;
	[Tooltip("The reference to this planet's navigation manager.")]
	public NavigationManager navManager;

	[HideInInspector]
	public int TileCount;
	[Range(1, 4)]
	public int detailLevel;
	[Tooltip("The number of colors used to color the sphere when using random region generation.  The colors themselves are explicitly defined in the Tile class.")]
	public int numColors;

	//The scale multiplier for the entire planet
	[HideInInspector]
	public float planetScale;
	
	public Material hexMat;
	public Material pentMat;
	
	private float radialScale;
	private float maxEdgeLength;
	private float maxTileRadius;
	//Worldspace radius of the planet
	private float worldRadius;

	[SerializeField, HideInInspector]
	private List<Vector3> vectors;// = new List<Vector3>();
	[SerializeField, HideInInspector]
	private List<int> indices;// = new List<int>();
	[SerializeField, HideInInspector]
	private List<GameObject> TileObjects;// = new List<GameObject>();
	[SerializeField, HideInInspector]
	private List<Tile> tiles;// = new List<Tile>();

	[HideInInspector]
	public bool tilesGenerated;

	void Start(){
		planetID = Planet_ID;
		Planet_ID++;
		planetInstances.Add (this);

		if (generateOnPlay && !tilesGenerated) {
			//Build the HexSphere
			BuildPlanet ();
			//Assign tile attributes
			MapBuilder ();
		}
		navManager.setWorldTiles (tiles);
	}

	
	public void BuildPlanet(){
		vectors = new List<Vector3>();
		indices = new List<int>();
		TileObjects = new List<GameObject>();
		tiles = new List<Tile>();

		if (detailLevel < 1) {
			detailLevel = 1;
		}
		
		//Mesh generation freezes up for detail levels greater than 4
		if (detailLevel > 4) {
			detailLevel = 4;
		}
		
		//radialScale = detailLevel;
		unitScale = detailLevel;
		
		#region Generate Icosahedron Vertices
		//HEX VERTICES
		Geometry.Icosahedron(vectors, indices);
		//subdivide
		for (int i = 0; i < detailLevel; i++)
			Geometry.Subdivide(vectors, indices, true);
		
		/// normalize vectors to "inflate" the icosahedron into a sphere.
		for (int i = 0; i < vectors.Count; i++){
			//You can also multiply this by some amount to change the build size
			vectors[i]=Vector3.Normalize(vectors[i]) * detailLevel;
		}

		#endregion
		
		List<Vector3> centers = getTriangleIncenter(vectors, indices);
		
		maxEdgeLength = getMaxEdgeDistance (centers);
		maxTileRadius = getMaxTileRadius (centers, vectors);
		
		generateSubMeshes (centers, vectors);
		TileCount = TileObjects.Count;

		//Useful for establishing the world's size but commenting out to avoid unused field warnings.
		//worldRadius = Vector3.Magnitude (centers [0]);

		//find each tiles neighbors
		for (int i = 0; i < TileObjects.Count; i++) {
			TileObjects[i].GetComponent<Tile>().FindNeighbors();
		}
		tilesGenerated = true;

		//Assign tiles to navManager
		navManager.setWorldTiles (tiles);
	}
	
	private void removeTileColliders(){
		foreach (GameObject t in TileObjects) {
			Destroy(t.GetComponent<Collider>());
		}
	}
	
	
	private void generateSubMeshes(List<Vector3> centers, List<Vector3> vertices){
		//Generate the hex/pent mesh for each vertex in the main mesh by associating it to its surrounding triangle centers
		for(int i = 0; i < vertices.Count; i++){
			GameObject tile = new GameObject ("Tile " + i);
			Mesh submesh = new Mesh ();
			tile.AddComponent<MeshFilter> ();

			tile.transform.parent = this.transform;
			tile.transform.localPosition = vertices[i];
			tile.transform.up = vertices[i];

			List<Vector3> submeshVs = new List<Vector3>();
			for(int j = 0; j < centers.Count; j++){
				if(Vector3.Distance(vertices[i], centers[j]) <= maxTileRadius){
					submeshVs.Add(centers[j]);
				}
			}
			
			//If its a pentagon
			if(submeshVs.Count == 5){
				
				bool[] isUsed = new bool[5];
				List<int> orderedIndices = new List<int>();
				Vector3 current = submeshVs[0];
				orderedIndices.Add(0);
				isUsed[0] = true;
				//starting at the first point in submeshVs, find a point on the perimeter of the tile that is within one edgelength from point current, then add its index to the list
				while(orderedIndices.Count < 5){
					foreach(Vector3 c in submeshVs){
						if(Vector3.Distance(c, current) <= maxEdgeLength && Vector3.Distance(c, current) >= 0.001f && !isUsed[submeshVs.IndexOf(c)]){
							//triangles[h + j] = submeshVs.IndexOf(c);
							orderedIndices.Add(submeshVs.IndexOf(c));
							isUsed[submeshVs.IndexOf(c)] = true;
							current = c;
							break;
						}
					}
				}
				int[] triangles = new int[9];
				triangles[0] = 0;
				triangles[1] = orderedIndices[1];
				triangles[2] = orderedIndices[2];
				
				triangles[3] = orderedIndices[2];
				triangles[4] = orderedIndices[3];
				triangles[5] = orderedIndices[0];
				
				triangles[6] = orderedIndices[3];
				triangles[7] = orderedIndices[4];
				triangles[8] = orderedIndices[0];

				//Convert the vertices to the tile's local space
				for(int k = 0; k < submeshVs.Count; k++){
					submeshVs[k] = tile.transform.InverseTransformPoint(submeshVs[k]);
				}
				Vector3[] subVsArray = submeshVs.ToArray();
				submesh.vertices = subVsArray;
				submesh.triangles = triangles;
				Vector2[] uvs = new Vector2[submeshVs.Count];
				
				uvs[orderedIndices[0]] = new Vector2(0f, 0.625f);
				uvs[orderedIndices[1]] = new Vector2(0.5f, 1f);
				uvs[orderedIndices[2]] = new Vector2(1f, 0.625f);
				uvs[orderedIndices[3]] = new Vector2(0.8f, 0.0162f);
				uvs[orderedIndices[4]] = new Vector2(.1875f, 0.0162f);
				
				submesh.uv = uvs;
		
				tile.AddComponent<MeshRenderer>();
				//Single material
				tile.GetComponent<Renderer>().sharedMaterial = pentMat;
				
				
			}
			//If its a hexagon
			else if(submeshVs.Count == 6){
				bool[] isUsed = new bool[6];
				List<int> orderedIndices = new List<int>();
				Vector3 current = submeshVs[0];
				orderedIndices.Add(0);
				isUsed[0] = true;
				//starting at the first point in submeshVs, find a point on the perimeter of the tile that is within one edgelength from point current, then add its index to the list
				while(orderedIndices.Count < 6){
					foreach(Vector3 c in submeshVs){
						if(Vector3.Distance(c, current) <= maxEdgeLength && Vector3.Distance(c, current) >= 0.001f && !isUsed[submeshVs.IndexOf(c)]){
							orderedIndices.Add(submeshVs.IndexOf(c));
							isUsed[submeshVs.IndexOf(c)] = true;
							current = c;
							break;
						}
					}
				}
				int[] triangles = new int[12];
				triangles[0] = 0;
				triangles[1] = orderedIndices[1];
				triangles[2] = orderedIndices[2];
				
				triangles[3] = orderedIndices[2];
				triangles[4] = orderedIndices[3];
				triangles[5] = 0;
				
				triangles[6] = orderedIndices[3];
				triangles[7] = orderedIndices[4];
				triangles[8] = 0;
				
				triangles[9] = orderedIndices[4];
				triangles[10] = orderedIndices[5];
				triangles[11] = 0;

				//Convert the vertices to the tile's local space
				for(int k = 0; k < submeshVs.Count; k++){
					submeshVs[k] = tile.transform.InverseTransformPoint(submeshVs[k]);
				}
				Vector3[] subVsArray = submeshVs.ToArray();
				submesh.vertices = subVsArray;
				submesh.triangles = triangles;
				
				Vector2[] uvs = new Vector2[6];
				//UV Coords based on geometry of hexagon
				uvs[orderedIndices[0]] = new Vector2(0.0543f, 0.2702f);
				uvs[orderedIndices[1]] = new Vector2(0.0543f, 0.7272f);
				uvs[orderedIndices[2]] = new Vector2(0.5f, 1f);
				uvs[orderedIndices[3]] = new Vector2(0.946f, 0.7272f);
				uvs[orderedIndices[4]] = new Vector2(0.946f, 0.2702f);
				uvs[orderedIndices[5]] = new Vector2(0.5f, 0f);
				
				submesh.uv = uvs;

				tile.AddComponent<MeshRenderer>();
				//Single material
				tile.GetComponent<Renderer>().sharedMaterial = hexMat;
				
			}
			
			//Assign mesh
			tile.GetComponent<MeshFilter>().mesh = submesh;
			submesh.RecalculateBounds();
			submesh.RecalculateNormals();
			tile.AddComponent<Tile>();
			
			//Fix any upsidedown tiles by checking their normal vector
			if((tile.transform.TransformDirection(submesh.normals[0]) + vertices[i]).sqrMagnitude < vertices[i].sqrMagnitude){
				submesh.triangles = submesh.triangles.Reverse().ToArray();
				submesh.RecalculateBounds();
				submesh.RecalculateNormals();
			}
			//Initialize tile attributes
			tile.AddComponent<MeshCollider>();

			Tile t = tile.GetComponent<Tile>();
			t.Initialize(vertices[i] + transform.position);
			t.parentPlanet = this;
			t.setTileRadius(maxTileRadius);
			tiles.Add(t);

			tile.isStatic = true;
			TileObjects.Add(tile);
		}
	}
	
	
	
	void MapBuilder(){
		//Put your map building logic in here


	}

	public void generateRandomRegions(){
		//Randomly assign colors
		for (int i = 0; i < tiles.Count; i++) {
			int col = UnityEngine.Random.Range(1, numColors + 1);
			tiles[i].setColor(col);
			//Just an example in which purple tiles are non navigable 
			if(col == 2){
				tiles[i].navigable = false;
			}
			else{
				tiles[i].navigable = true;
			}
		}
	}

	private float getMaxTileRadius(List<Vector3> centers, List<Vector3> vertices){
		float delta = 1.5f;
		Vector3 v = Vector3.zero;
		if (detailLevel != 0) {
			v = vertices [12];
		}
		else{
			v = vertices [0];
		}
		
		float minDistance = Mathf.Infinity;
		foreach (Vector3 c in centers) {
			
			float dist = Vector3.Distance(v, c);
			
			if (dist < minDistance){
				minDistance = dist;
			}
			
		}
		minDistance = minDistance * (delta);
		
		return minDistance;
	}
	
	private float getMaxEdgeDistance(List<Vector3> centers) {
		//Returns the approximate distance between adjacent triangle centers
		
		//delta is the approximate variation in edge lengths, as not all edges are the same length
		float delta = 1.4f;
		Vector3 point = centers [0];
		// scan all vertices to find nearest
		float minDistance = Mathf.Infinity;
		foreach (Vector3 n in centers) {
			if(!point.Equals(n)){
				float dist = Vector3.Distance(point, n);
				
				if (dist < minDistance){
					minDistance = dist;
				}
			}
		}
		
		minDistance = minDistance * (delta);
		
		return minDistance;
	}
	
	private List<Vector3> getTriangleIncenter(List<Vector3> vertices, List<int> triangles){
		List<Vector3> centers = new List<Vector3> ();
		for (int i = 0; i < triangles.Count - 2; i += 3) {
			Vector3 A = vertices[triangles[i]];
			Vector3 B = vertices[triangles[i + 1]];
			Vector3 C = vertices[triangles[i + 2]];
			
			float a = Vector3.Distance(C, B);
			float b = Vector3.Distance(A, C);
			float c = Vector3.Distance(A, B);
			
			float P = a + b + c;
			
			Vector3 abc = new Vector3(a, b, c);
			
			float x = Vector3.Dot (abc, new Vector3(A.x, B.x, C.x)) / P;
			float y = Vector3.Dot (abc, new Vector3(A.y, B.y, C.y)) / P;
			float z = Vector3.Dot (abc, new Vector3(A.z, B.z, C.z)) / P;
			
			Vector3 center = new Vector3(x, y, z);
			centers.Add(center);
		}
		return centers;
	}

	public void setWorldScale(float scale){
		transform.localScale = Vector3.one * scale;
		planetScale = scale;
	}

	//Destroys all tiles and resets the 
	public void deleteTiles(){
		foreach(GameObject t in TileObjects){
			DestroyImmediate(t);
		}

		indices.Clear ();
		vectors.Clear ();
		tiles.Clear ();
		TileObjects.Clear ();
		tilesGenerated = false;
		TileCount = 0;
	}

}
