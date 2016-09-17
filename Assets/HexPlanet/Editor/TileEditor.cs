using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Tile))]
[CanEditMultipleObjects]
public class TileEditor : Editor {

	public Tile tile;
	public GameObject objectToPlace;

	private static string placeObjToolTip = "Place an object on this tile.  If the object is a prefab and not in the scene, it will be instantiated and then placed.  If it does exist in the scene, it will be moved to this tile.";

	void OnEnable(){
		tile = (Tile)target;
	}

	public override void OnInspectorGUI(){
		DrawDefaultInspector ();

		//Place object interface
		EditorGUILayout.BeginHorizontal ();
		objectToPlace = (GameObject)EditorGUILayout.ObjectField (new GUIContent("Object to place", placeObjToolTip), objectToPlace, typeof(GameObject), true); 
		if (GUILayout.Button ("Place Object") && objectToPlace != null) {
			//If its a prefab, spawn it then place it
			if(EditorUtility.IsPersistent(objectToPlace)){
				GameObject o = Instantiate(objectToPlace) as GameObject;
				tile.placeObject(o);
			}
			//If it is a scene object, move its current instance to the tile
			else if(objectToPlace.activeInHierarchy){
				tile.placeObject(objectToPlace);
			}
			//Clear the object slot
			objectToPlace = null;
		}
		EditorGUILayout.EndHorizontal ();
	}
}
