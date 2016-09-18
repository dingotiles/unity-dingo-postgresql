using UnityEngine;
using System.Collections;
using thelab.mvc;


public class CursorMovement : Controller<DingoApplication> {
	LayerMask tileSlotMask;
	float camRayLength = 100f;

	void Awake()
	{
		tileSlotMask = LayerMask.GetMask ("Tile");
	}

	void Update()
	{
		ShowCursor ();
	}

	void ShowCursor()
	{
		Ray cameraRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit tileSlotHit;
		if (Physics.Raycast (cameraRay, out tileSlotHit, camRayLength, tileSlotMask)) {
			TileSlotView tileSlot = tileSlotHit.collider.gameObject.GetComponentInParent<TileSlotView> ();
			if (tileSlot != null) {
				tileSlot.EnableCursor (true);
			}
		}
	}
}
