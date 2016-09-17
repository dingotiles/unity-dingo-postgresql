using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Hexsphere))]
public class HexplanetEditor : Editor {

	Hexsphere planet;

	void OnEnable(){
		planet = (Hexsphere)target;
	}

	public override void OnInspectorGUI(){
		DrawDefaultInspector ();

		EditorGUILayout.LabelField ("Planet ID", planet.planetID.ToString());
		//mainPlanet.detailLevel = EditorGUILayout.IntSlider ("Detail Level", mainPlanet.detailLevel, 1, 4);
		EditorGUILayout.LabelField ("Tile Count", planet.TileCount.ToString());

		EditorGUI.BeginDisabledGroup (planet.tilesGenerated);
		//Generate Planet
		if(GUILayout.Button("Generate Tiles") && !planet.tilesGenerated){
			planet.BuildPlanet();
		}
		EditorGUI.EndDisabledGroup ();

		EditorGUI.BeginDisabledGroup (!planet.tilesGenerated);
		//Random region generation
		if (GUILayout.Button ("Generate Random Regions") && planet.tilesGenerated) {
			planet.generateRandomRegions();
		}
		//Delete tiles
		if(GUILayout.Button("Delete Tiles") && planet.tilesGenerated){
			planet.deleteTiles();
			//Reset the scale slider to 1 when deleting
			planet.setWorldScale(1f);
			EditorGUILayout.Slider ("Planet Scale", 1f, 1f, 10f);
		}
		EditorGUI.EndDisabledGroup ();

		EditorGUI.BeginDisabledGroup (Application.isPlaying);
		//Scale slider
		planet.setWorldScale (EditorGUILayout.Slider ("Planet Scale", planet.planetScale, 1f, 10f));
		EditorGUI.EndDisabledGroup ();
		//Ensure that the hexplanet's lists arent destroyed when playmode is entered
		if (GUI.changed) {
			EditorUtility.SetDirty(planet);  
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
