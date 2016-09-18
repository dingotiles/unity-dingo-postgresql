using UnityEngine;
using System.Collections;
using thelab.mvc;

public class DatabaseHighlight : View<DingoApplication>
{
	public Material baseMaterial;
	public Material highlightMaterial;
	public bool highlight;
	bool isHighlighted;
	shaderGlow glow;
	DatabaseLayerColor[] layers;
	Material currentMaterial;

	public DatabaseHighlight replica = null;
	public DatabaseSphereView sphere { get { return p_sphere = base.Assert<DatabaseSphereView> (p_sphere); } }
	DatabaseSphereView p_sphere;

	public TileSlotView tileSlot { get { return p_tileSlot = AssertParent<TileSlotView> (p_tileSlot); } }
	TileSlotView p_tileSlot;

	void Awake()
	{
		layers = GetComponentsInChildren<DatabaseLayerColor> ();
		glow = GetComponentInChildren<shaderGlow> ();
	}

	public void SetHighlight(bool doHighlight)
	{
		// TODO - 'replica' might be 'leader' for RouterView/TileSlotViews
		if (doHighlight && replica != null) {
			sphere.outboundTarget = replica.sphere.gameObject;
		}
		if (currentMaterial != null && doHighlight == isHighlighted) {
			return;
		}
		// Set material/color
		for (int i = 0; i < layers.Length; i++) {
			if (doHighlight) {
				currentMaterial = highlightMaterial;
			} else {
				currentMaterial = baseMaterial;
			}
			layers [i].SetMaterial (currentMaterial);
		}

		sphere.visible = doHighlight;
		if (replica != null) {
			sphere.outboundTarget = replica.sphere.gameObject;
		}

		// Set glow on/off
		if (doHighlight) {
			glow.lightOn ();
			isHighlighted = true;
		} else if (isHighlighted) {
			glow.lightOff ();
			isHighlighted = false;
		}
		isHighlighted = doHighlight;
		highlight = doHighlight;
	}

}